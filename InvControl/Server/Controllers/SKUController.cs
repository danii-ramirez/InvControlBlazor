using ClosedXML.Excel;
using InvControl.Server.Data;
using InvControl.Server.Helpers;
using InvControl.Shared.DTO;
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
    public class SKUController : ControllerBase
    {
        private readonly ILogger<SKUController> _logger;
        private readonly string connectionString;

        public SKUController(ILogger<SKUController> logger, IConfiguration configuration)
        {
            _logger = logger;
            connectionString = configuration.GetConnectionString("InvControlDB");
        }

        [HttpGet]
        public IActionResult GetSKU(int? codigo, string nombre, bool? activo, int? idMarca)
        {
            List<SKU> lst = new();
            DA_SKU da = new(connectionString);
            using (DataTable dt = da.ObtenerSKU(null, codigo, nombre?.Trim(), activo, idMarca))
            {
                foreach (DataRow dr in dt.Rows)
                {
                    SKU s = new()
                    {
                        IdSKU = (int)dr["IdSKU"],
                        Codigo = (int)dr["Codigo"],
                        Nombre = (string)dr["Nombre"],
                        Activo = (bool)dr["Activo"],
                        Especial = (bool)dr["Especial"],
                        IdMarca = (int)dr["IdMarca"],
                        NombreMarca = (string)dr["DescripcionMarca"],
                        IdTipoContenedor = (int?)dr["IdTipoContenedor"],
                        NombreTipoContenedor = (string)dr["NombreTipoContenedor"],
                        UnidadesPorContenedor = (int?)dr["UnidadesPorContenedor"]
                    };
                    if (dr["Descripcion"] != DBNull.Value) s.Descripcion = (string)dr["Descripcion"];

                    lst.Add(s);
                }
            }

            return Ok(lst);
        }

        [HttpPost]
        public IActionResult PostSKU(SKU sku)
        {
            SqlTransaction transaction = null;
            try
            {
                DA_SKU daSKU = new(connectionString);
                DA_Stock daS = new(connectionString);
                DA_Auditoria daAu = new(connectionString);

                if (daSKU.ObtenerSKU(null, sku.Codigo, null, null, null).Rows.Count > 0)
                    ModelState.AddModelError(nameof(SKU.Codigo), "Ya existe un SKU con el mismo código");

                if (daSKU.ObtenerSKU(null, null, sku.Nombre.Trim(), null, null).Rows.Count > 0)
                    ModelState.AddModelError(nameof(SKU.Nombre), "Ya existe un SKU con el mismo nombre");

                if (ModelState.IsValid)
                {
                    using (SqlConnection cnn = new(connectionString))
                    {
                        cnn.Open();
                        transaction = cnn.BeginTransaction();

                        sku.IdSKU = daSKU.InsertarSKU((int)sku.Codigo!, sku.Nombre.Trim(), sku.Descripcion?.Trim()!, sku.Activo, sku.Especial, sku.IdMarca,
                             (int)sku.IdTipoContenedor!, (int)sku.UnidadesPorContenedor!, int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)), DateTime.Now, transaction);

                        daS.InsertarStock(sku.IdSKU, 0, DateTime.Now, transaction);

                        daAu.Insertar($"Se creó el SKU: {sku.Codigo}", DateTime.Now, (int)TipoEntidad.SKU, (int)TipoOperacion.Creacion,
                            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)), transaction);

                        transaction.Commit();
                        cnn.Close();
                    }

                    return Ok(sku);
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

        [HttpPut]
        public IActionResult PutSKU(SKU sku)
        {
            SqlTransaction transaction = null;
            try
            {
                DA_SKU daSKU = new(connectionString);
                DA_Auditoria daAu = new(connectionString);

                using (DataTable dt = daSKU.ObtenerSKU(null, sku.Codigo, null, null, null))
                {
                    if (dt.Rows.Count > 0 && (int)dt.Rows[0]["IdSKU"] != sku.IdSKU)
                        ModelState.AddModelError(nameof(SKU.Codigo), "Ya existe un SKU con el mismo código");
                }

                using (DataTable dt = daSKU.ObtenerSKU(null, null, sku.Nombre.Trim(), null, null))
                {
                    if (dt.Rows.Count > 0 && (int)dt.Rows[0]["IdSKU"] != sku.IdSKU)
                        ModelState.AddModelError(nameof(SKU.Nombre), "Ya existe un SKU con el mismo nombre");
                }

                if (ModelState.IsValid)
                {
                    using (SqlConnection cnn = new(connectionString))
                    {
                        cnn.Open();
                        transaction = cnn.BeginTransaction();

                        daSKU.ActualizarSKU(sku.IdSKU, (int)sku.Codigo!, sku.Nombre.Trim(), sku.Descripcion?.Trim()!, sku.Activo, sku.Especial, sku.IdMarca,
                            (int)sku.IdTipoContenedor!, (int)sku.UnidadesPorContenedor!, transaction);

                        daAu.Insertar($"Se editó el SKU: {sku.Codigo}", DateTime.Now, (int)TipoEntidad.SKU, (int)TipoOperacion.Edicion,
                            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)), transaction);

                        transaction.Commit();
                        cnn.Close();
                    }

                    return Ok(sku);
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

        [HttpGet("marcas")]
        public IActionResult GetMarcas()
        {
            List<Marca> marcas = new();
            using (DataTable dt = new DA_SKU(connectionString).ObtenerMarcas())
            {
                foreach (DataRow dr in dt.Rows)
                {
                    Marca m = new()
                    {
                        IdMarca = (int)dr["IdMarca"],
                        Descripcion = (string)dr["Descripcion"]
                    };
                    marcas.Add(m);
                }
            }
            return Ok(marcas);
        }

        [HttpGet("tiposcontenedores")]
        public IActionResult GetTiposContenedores()
        {
            List<TipoContenedor> tipos = new();
            using (DataTable dt = new DA_SKU(connectionString).ObtenerTiposContendores())
            {
                foreach (DataRow dr in dt.Rows)
                {
                    TipoContenedor m = new()
                    {
                        IdTipoContenedor = (int)dr["IdTipoContenedor"],
                        Nombre = (string)dr["Nombre"]
                    };
                    tipos.Add(m);
                }
            }
            return Ok(tipos);
        }

        [HttpGet("sugerencias")]
        public IActionResult GetSugerencias(string sugerencia)
        {
            List<SKUDTO> skus = new();
            using (DataTable dt = new DA_SKU(connectionString).ObtenerSugerencias(sugerencia.Trim()))
            {
                foreach (DataRow dr in dt.Rows)
                {
                    SKUDTO s = new()
                    {
                        IdSku = (int)dr["IdSKU"],
                        Codigo = (int?)dr["Codigo"],
                        NombreSku = (string)dr["Nombre"]
                    };
                    skus.Add(s);
                }
            }
            return Ok(skus);
        }

        [HttpPost("ExportToExcel")]
        public IActionResult PostExportToExcel([FromBody] List<SKU> skus)
        {
            try
            {
                using var workbook = new XLWorkbook();
                var worksheet = workbook.Worksheets.Add("Datos");

                worksheet.Cell(1, 1).Value = "Código";
                worksheet.Cell(1, 2).Value = "Nombre";
                worksheet.Cell(1, 3).Value = "Descripción";
                worksheet.Cell(1, 4).Value = "Marca";
                worksheet.Cell(1, 5).Value = "Activo";
                worksheet.Cell(1, 6).Value = "Especial";
                worksheet.Cell(1, 7).Value = "Contenedor";
                worksheet.Cell(1, 8).Value = "Unidades por contenedor";

                for (int i = 0; i < skus.Count; i++)
                {
                    worksheet.Cell(i + 2, 1).Value = skus[i].Codigo;
                    worksheet.Cell(i + 2, 2).Value = skus[i].Nombre;
                    worksheet.Cell(i + 2, 3).Value = skus[i].Descripcion;
                    worksheet.Cell(i + 2, 4).Value = skus[i].NombreMarca;
                    worksheet.Cell(i + 2, 5).Value = skus[i].Activo ? "Sí" : "No";
                    worksheet.Cell(i + 2, 6).Value = skus[i].Especial ? "Sí" : "No";
                    worksheet.Cell(i + 2, 7).Value = skus[i].NombreTipoContenedor;
                    worksheet.Cell(i + 2, 8).Value = skus[i].UnidadesPorContenedor;
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
    }
}
