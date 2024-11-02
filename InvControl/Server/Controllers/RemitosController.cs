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
        public IActionResult GetRemitos(int? idRemito, string numeroRemito, RemitoEstado? remitoEstado)
        {
            List<RemitoDTO> remitos = new();
            DA_Remito da = new(connectionString);
            using (DataTable dtR = da.ObtenerRemitos(idRemito, numeroRemito, remitoEstado?.GetHashCode()))
            {
                foreach (DataRow drR in dtR.Rows)
                {
                    RemitoDTO r = new()
                    {
                        IdRemito = (int)drR["IdRemito"],
                        NumeroRemito = (string)drR["Numero"],
                        FechaIngreso = (DateTime)drR["FechaIngreso"],
                        FechaRemito = (DateTime)drR["Fecha"],
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

        [HttpGet("{idRemito}")]
        public IActionResult GetRemito([FromRoute] int idRemito, [FromQuery] RemitoEstado? remitoEstado)
        {
            Remito remito = null;

            DA_Remito da = new(connectionString);

            using (DataTable dtR = da.ObtenerRemitos(idRemito, null, remitoEstado?.GetHashCode()))
            {
                if (dtR.Rows.Count > 0)
                {
                    DataRow drR = dtR.Rows[0];

                    remito = new()
                    {
                        IdRemito = (int)drR["IdRemito"],
                        NumeroRemito = (string)drR["Numero"],
                        FechaRemito = (DateTime)drR["Fecha"],
                        IdEstado = (int)drR["IdEstado"]
                    };

                    if (drR["IdTransporte"] != DBNull.Value) remito.IdTransporte = (int?)drR["IdTransporte"];
                    if (drR["IdChofer"] != DBNull.Value) remito.IdChofer = (int?)drR["IdChofer"];

                    using DataTable dtD = da.ObtenerRemitosDetalle(remito.IdRemito);
                    foreach (DataRow drD in dtD.Rows)
                    {
                        RemitoDetalle rd = new()
                        {
                            IdRemito = remito.IdRemito,
                            IdSku = (int)drD["IdSKU"],
                            Codigo = (int?)drD["CodigoSKU"],
                            NombreSku = (string)drD["NombreSKU"],
                            Cantidad = (int?)drD["Cantidad"]
                        };

                        remito.Detalle.Add(rd);
                    }
                }
            }

            return Ok(remito);
        }

        [HttpPost]
        public IActionResult PostRemito(Remito remito)
        {
            SqlTransaction transaction = null;
            try
            {
                DA_Remito daRe = new(connectionString);
                DA_StockMovimiento daSM = new();
                DA_Auditoria daAu = new(connectionString);

                if (daRe.ObtenerRemitos(null, remito.NumeroRemito.Trim(), null).Rows.Count > 0)
                    ModelState.AddModelError(nameof(Remito.NumeroRemito), "El remito ya se encuetra cargado");

                if (ModelState.IsValid)
                {
                    using (SqlConnection cnn = new(connectionString))
                    {
                        cnn.Open();
                        transaction = cnn.BeginTransaction();

                        remito.IdRemito = daRe.InsertarRemito(remito.NumeroRemito.Trim(), (DateTime)remito.FechaRemito!, remito.IdTransporte, remito.IdChofer, 1,
                            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)), DateTime.Now, transaction);

                        foreach (var d in remito.Detalle)
                        {
                            daRe.InsertarRemitoDetalle(remito.IdRemito, d.IdSku, (int)d.Codigo!, d.NombreSku, (int)d.Cantidad!, transaction);
                        }

                        daAu.Insertar($"Se ingresó el remito {remito.NumeroRemito.Trim()}", DateTime.Now, (int)TipoEntidad.Remito,
                            (int)TipoOperacion.Creacion, int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)), transaction);

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

        [HttpPut("cabecera")]
        public IActionResult PutRemitoCabecera(Remito remito)
        {
            SqlTransaction transaction = null;
            try
            {
                DA_Remito daRe = new(connectionString);
                DA_Auditoria daAu = new(connectionString);

                using (SqlConnection cnn = new(connectionString))
                {
                    cnn.Open();
                    transaction = cnn.BeginTransaction();

                    daRe.ActualizarRemito(remito.IdRemito, remito.IdEstado, remito.IdTransporte, remito.IdChofer, transaction);

                    string accion = remito.IdEstado == (int)RemitoEstado.Aprobado ? "aprobó" : "rechazó";
                    daAu.Insertar($"Se {accion} el remito {remito.NumeroRemito.Trim()}", DateTime.Now, (int)TipoEntidad.Remito,
                            (int)TipoOperacion.Edicion, int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)), transaction);

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

        [HttpPut("detalle")]
        public IActionResult PutRemitoDetalle(Remito remito)
        {
            SqlTransaction transaction = null;
            try
            {
                DA_Remito daRe = new(connectionString);
                DA_StockMovimiento daSM = new();
                DA_Auditoria daAu = new(connectionString);

                if (ModelState.IsValid)
                {
                    using (SqlConnection cnn = new(connectionString))
                    {
                        cnn.Open();
                        transaction = cnn.BeginTransaction();

                        daRe.ActualziarRemitoEstado(remito.IdRemito, (int)RemitoEstado.Pendiente, transaction);

                        daRe.EliminarRemitoDetalle(remito.IdRemito, transaction);
                        foreach (var d in remito.Detalle)
                        {
                            daRe.InsertarRemitoDetalle(remito.IdRemito, d.IdSku, (int)d.Codigo!, d.NombreSku, (int)d.Cantidad!, transaction);
                        }

                        daAu.Insertar($"Se editó el remito {remito.NumeroRemito.Trim()}", DateTime.Now, (int)TipoEntidad.Remito,
                            (int)TipoOperacion.Edicion, int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)), transaction);

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

        [HttpPut("procesar")]
        public IActionResult PutRemitosProcesar(List<RemitoDTO> remitos)
        {
            SqlTransaction transaction = null;
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

                            daSM.InsertarStockMovimientos((int)StockMovimientoEstado.Ingreso, rd.IdSku, (int)rd.Codigo!, rd.NombreSku, unidades,
                                $"Remito nro.: {r.NumeroRemito}", fecha, int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)), transaction);

                            daS.InsertarStock(rd.IdSku, unidades, fecha, transaction);

                            daAu.Insertar($"Se procesó el remito {r.NumeroRemito.Trim()}", DateTime.Now, (int)TipoEntidad.Remito,
                                (int)TipoOperacion.Edicion, int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)), transaction);
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
