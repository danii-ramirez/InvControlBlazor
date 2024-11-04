using ClosedXML.Excel;
using InvControl.Server.Data;
using InvControl.Shared.Helpers;
using InvControl.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Security.Claims;

namespace InvControl.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StockController : ControllerBase
    {
        private readonly ILogger<StockController> _logger;
        private readonly string connectionString;

        public StockController(ILogger<StockController> logger, IConfiguration configuration)
        {
            _logger = logger;
            connectionString = configuration.GetConnectionString("InvControlDB");
        }

        [HttpPost("ajuste")]
        public IActionResult PostAjuste(StockAjuste stockAjuste)
        {
            SqlTransaction transaction = null;
            try
            {
                DA_Stock daS = new(connectionString);
                DA_TipoMovimiento daTM = new(connectionString);

                using (SqlConnection cnn = new(connectionString))
                {
                    cnn.Open();
                    transaction = cnn.BeginTransaction();

                    foreach (var s in stockAjuste.Detalle)
                    {
                        int cantidad;
                        if (stockAjuste.Tipo == Tipo.Entrada.ToString())
                        {
                            cantidad = (int)s.Cantidad;
                        }
                        else
                        {
                            cantidad = (int)s.Cantidad * -1;
                        }

                        daS.InsertarStock(s.IdSku, cantidad, DateTime.Now, transaction);

                        daS.InsertarStockMovimientos(stockAjuste.IdTipoMovimiento, s.IdSku, (int)s.Codigo, s.NombreSku, cantidad, stockAjuste.Observaciones,
                            DateTime.Now, int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)), transaction);
                    }

                    transaction.Commit();
                    cnn.Close();
                }

                return Ok();
            }
            catch (Exception ex)
            {
                if (transaction != null && transaction.Connection != null)
                    transaction.Rollback();
                _logger.LogError(ex, "{msg}", ex.Message);
                return StatusCode(500, ex);
            }
        }

        [HttpGet("consulta")]
        public IActionResult GetStock(string nombre, int? idMarca, bool? especial, int? cantidadMin, int? cantidadMax, DateTime? fechaMin, DateTime? fechaMax)
        {
            List<Stock> stock = new();

            using (DataTable dt = new DA_Stock(connectionString).ObtenerStock(nombre?.Trim(), idMarca, especial, cantidadMin, cantidadMax, fechaMin, fechaMax))
            {
                foreach (DataRow dr in dt.Rows)
                {
                    Stock s = new()
                    {
                        IdStock = (int)dr["IdStock"],
                        IdSku = (int)dr["IdSKU"],
                        Codigo = (int)dr["Codigo"],
                        Nombre = (string)dr["Nombre"],
                        Marca = (string)dr["Marca"],
                        Cantidad = (int)dr["Cantidad"]
                    };
                    stock.Add(s);
                }
            }

            return Ok(stock);
        }

        [HttpPost("consulta/ExportToExcel")]
        public IActionResult PostExportToExcel([FromBody] List<Stock> skus)
        {
            try
            {
                using var workbook = new XLWorkbook();
                var worksheet = workbook.Worksheets.Add("Datos");

                worksheet.Cell(1, 1).Value = "Código";
                worksheet.Cell(1, 2).Value = "Nombre";
                worksheet.Cell(1, 3).Value = "Marca";
                worksheet.Cell(1, 4).Value = "Stock";

                for (int i = 0; i < skus.Count; i++)
                {
                    worksheet.Cell(i + 2, 1).Value = skus[i].Codigo;
                    worksheet.Cell(i + 2, 2).Value = skus[i].Nombre;
                    worksheet.Cell(i + 2, 3).Value = skus[i].Marca;
                    worksheet.Cell(i + 2, 4).Value = skus[i].Cantidad;
                }

                using var stream = new MemoryStream();
                workbook.SaveAs(stream);
                stream.Seek(0, SeekOrigin.Begin);
                return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ExportedData.xlsx");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }

        [HttpGet("TipoMovimiento")]
        public IActionResult GetTiposMovimientos(int? idTipoMovimiento, string nombre, bool? soloLectura, bool? interno)
        {
            List<TipoMovimiento> tiposMovimientos = new();

            using (DataTable dt = new DA_TipoMovimiento(connectionString).ObtenerTipoMovimiento(idTipoMovimiento, nombre, soloLectura, interno))
            {
                foreach (DataRow dr in dt.Rows)
                {
                    TipoMovimiento tm = new()
                    {
                        IdTipoMovimiento = (int)dr["IdTipoMovimiento"],
                        Nombre = (string)dr["Nombre"],
                        Tipo = (string)dr["Tipo"],
                        SoloLectura = (bool)dr["SoloLectura"],
                        Interno = (bool)dr["Interno"]
                    };

                    tiposMovimientos.Add(tm);
                }
            }

            return Ok(tiposMovimientos);
        }

        [HttpPost("TipoMovimiento")]
        public IActionResult PostTipoMovimiento(TipoMovimiento tipoMovimiento)
        {
            SqlTransaction transaction = null;
            try
            {
                DA_TipoMovimiento daTM = new(connectionString);
                DA_Auditoria daAu = new(connectionString);

                if (daTM.ObtenerTipoMovimiento(null, tipoMovimiento.Nombre.Trim(), null, null).Rows.Count > 0)
                    ModelState.AddModelError(nameof(TipoMovimiento.Nombre), "Ya existe movimiento con el mismo nombre");

                if (ModelState.IsValid)
                {
                    using (SqlConnection cnn = new(connectionString))
                    {
                        cnn.Open();
                        transaction = cnn.BeginTransaction();

                        tipoMovimiento.IdTipoMovimiento = daTM.InsertarTipoMovimiento(tipoMovimiento.Nombre.Trim(), tipoMovimiento.Tipo, tipoMovimiento.SoloLectura, tipoMovimiento.Interno, transaction);

                        transaction.Commit();
                        cnn.Close();
                    }

                    return Ok(tipoMovimiento);
                }

                return BadRequest(ModelState);
            }
            catch (Exception ex)
            {
                if (transaction != null && transaction.Connection != null)
                    transaction.Rollback();
                _logger.LogError(ex, "{msg}", ex.Message);
                return StatusCode(500, ex);
            }
        }

        [HttpPut("TipoMovimiento")]
        public IActionResult PutTipoMovimiento(TipoMovimiento tipoMovimiento)
        {
            SqlTransaction transaction = null;
            try
            {
                DA_TipoMovimiento daTM = new(connectionString);
                DA_Auditoria daAu = new(connectionString);

                using (DataTable dt = daTM.ObtenerTipoMovimiento(null, tipoMovimiento.Nombre.Trim(), null, null))
                {
                    if (dt.Rows.Count > 0 && (int)dt.Rows[0]["IdTipoMovimiento"] != tipoMovimiento.IdTipoMovimiento)
                        ModelState.AddModelError(nameof(TipoMovimiento.Nombre), "Ya existe movimiento con el mismo nombre");
                }

                if (ModelState.IsValid)
                {
                    using (SqlConnection cnn = new(connectionString))
                    {
                        cnn.Open();
                        transaction = cnn.BeginTransaction();

                        daTM.ActualizarTipoMovimiento(tipoMovimiento.IdTipoMovimiento, tipoMovimiento.Nombre.Trim(), tipoMovimiento.Tipo, tipoMovimiento.SoloLectura, tipoMovimiento.Interno, transaction);

                        transaction.Commit();
                        cnn.Close();
                    }

                    return Ok(tipoMovimiento);
                }

                return BadRequest(ModelState);
            }
            catch (Exception ex)
            {
                if (transaction != null && transaction.Connection != null)
                    transaction.Rollback();
                _logger.LogError(ex, "{msg}", ex.Message);
                return StatusCode(500, ex);
            }
        }
    }
}
