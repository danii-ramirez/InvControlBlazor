using System.Data;
using System.Security.Claims;
using InvControl.Server.Data;
using InvControl.Shared.DTO;
using InvControl.Shared.Helpers;
using InvControl.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Radzen.Blazor.Rendering;

namespace InvControl.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RemitosController : ControllerBase
    {
        private readonly ILogger<RemitosController> _logger;
        private readonly string connectionString;

        public RemitosController(ILogger<RemitosController> logger, IConfiguration configuration)
        {
            _logger = logger;
            connectionString = configuration.GetConnectionString("InvControlDB");
        }

        [HttpGet]
        public IActionResult GetRemitos(int? idRemito, string? numeroRemito, int? idEstado)
        {
            List<RemitoDTO> remitos = new();
            DA_Remito da = new(connectionString);
            using (DataTable dtR = da.ObtenerRemitos(idRemito, numeroRemito, idEstado))
            {
                foreach (DataRow drR in dtR.Rows)
                {
                    RemitoDTO r = new()
                    {
                        IdRemito = (int)drR["IdRemito"],
                        NumeroRemito = (string)drR["Numero"],
                        FechaIngreso = (DateTime)drR["FechaIngreso"],
                        Fecha = (DateTime)drR["Fecha"],
                        IdEstado = (int)drR["IdEstado"],
                        DescripcionEstado = (string)drR["DescripcionEstado"],
                        IdUsuario = (int)drR["IdUsuario"],
                        NombreUsuario = (string)drR["NombreUsuario"],
                        ApellidoUsuario = (string)drR["ApellidoUsuario"]
                    };

                    if (drR["IdTransporte"] != DBNull.Value) r.IdTransporte = (int?)drR["IdTransporte"];
                    if (drR["NombreTransporte"] != DBNull.Value) r.NombreTransporte = (string)drR["NombreTransporte"];
                    if (drR["Patente"] != DBNull.Value) r.Patente = (string)drR["Patente"];
                    if (drR["IdChofer"] != DBNull.Value) r.IdChofer = (int?)drR["IdChofer"];
                    if (drR["NombreChofer"] != DBNull.Value) r.NombreChofer = (string)drR["NombreChofer"];
                    if (drR["ApellidoChofer"] != DBNull.Value) r.ApellidoChofer = (string)drR["ApellidoChofer"];

                    using (DataTable dtD = da.ObtenerRemitosDetalle(r.IdRemito))
                    {
                        foreach (DataRow drD in dtD.Rows)
                        {
                            RemitoDetalle rd = new()
                            {
                                IdRemito = r.IdRemito,
                                IdSku = (int)drD["IdSKU"],
                                Codigo = (int?)drD["CodigoSKU"],
                                NombreSku = (string)drD["NombreSKU"],
                                Cantidad = (int?)drD["Cantidad"]
                            };

                            r.Detalle.Add(rd);
                        }
                    }

                    remitos.Add(r);
                }
            }

            return Ok(remitos);
        }

        [HttpPost]
        public IActionResult PostRemito(Remito remito)
        {
            SqlTransaction transaction = default!;
            try
            {
                DA_Remito daRe = new(connectionString);
                DA_StockMovimiento daSM = new();
                DA_Auditoria daAu = new(connectionString);

                if (daRe.ObtenerRemitos(null, remito.Numero.Trim(), null).Rows.Count > 0)
                    ModelState.AddModelError(nameof(Remito.Numero), "El remito ya se encuetra cargado");

                if (ModelState.IsValid)
                {
                    using (SqlConnection cnn = new(connectionString))
                    {
                        cnn.Open();
                        transaction = cnn.BeginTransaction();

                        remito.IdRemito = daRe.InsertarRemito(remito.Numero.Trim(), (DateTime)remito.Fecha!, remito.IdTransporte, remito.IdChofer, 1,
                            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)), DateTime.Now, transaction);

                        foreach (var d in remito.Detalle)
                        {
                            daRe.InsertarRemitoDetalle(remito.IdRemito, d.IdSku, (int)d.Codigo!, d.NombreSku, (int)d.Cantidad!, transaction);
                        }

                        transaction.Commit();
                        cnn.Close();
                    }

                    return Ok(remito);
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

        [HttpPut("estado")]
        public IActionResult PutRemito(RemitoState remito)
        {
            SqlTransaction transaction = default!;
            try
            {
                DA_Remito daRe = new(connectionString);
                DA_Auditoria daAu = new(connectionString);

                using (SqlConnection cnn = new(connectionString))
                {
                    cnn.Open();
                    transaction = cnn.BeginTransaction();

                    daRe.ActualziarRemitoEstado(remito.IdRemito, remito.IdEstado, transaction);

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

        [HttpPut("procesar")]
        public IActionResult PutRemitosProcesar(List<RemitoDTO> remitos)
        {
            SqlTransaction transaction = default!;
            try
            {
                DA_Remito daRe = new(connectionString);
                DA_StockMovimiento daSM = new();
                DA_SKU daSKU = new(connectionString);
                DA_Stock daS = new(connectionString);
                DA_Auditoria daAu = new(connectionString);

                using (SqlConnection cnn = new(connectionString))
                {
                    cnn.Open();
                    transaction = cnn.BeginTransaction();

                    foreach (var r in remitos)
                    {
                        daRe.ActualziarRemitoEstado(r.IdRemito, (int)RemitoEstado.Procesado, transaction);

                        foreach (var rd in r.Detalle)
                        {
                            DateTime fecha = DateTime.Now;
                            int unidadesPorContenedor = (int)daSKU.ObtenerSKU(rd.IdSku, null, null, null, null, transaction).Rows[0]["UnidadesPorContenedor"];
                            int unidades = (int)rd.Cantidad! * unidadesPorContenedor;
                            int stock = (int)daS.ObtenerStockPorSKU(rd.IdSku, transaction).Rows[0]["Cantidad"];

                            daSM.InsertarStockMovimientos((int)StockMovimientoEstado.Ingreso, rd.IdSku, (int)rd.Codigo!, rd.NombreSku, unidades,
                                $"Remito nro.: {r.NumeroRemito}", fecha, int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)), transaction);

                            daS.InsertarStock(rd.IdSku, stock + unidades, fecha, transaction);
                        }
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
    }
}
