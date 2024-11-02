using InvControl.Server.Data;
using InvControl.Server.Helpers;
using InvControl.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Security.Claims;

namespace InvControl.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CanalesVentasController : ControllerBase
    {
        private readonly ILogger<CanalesVentasController> _logger;
        private readonly string connectionString;

        public CanalesVentasController(ILogger<CanalesVentasController> logger, IConfiguration configuration)
        {
            _logger = logger;
            connectionString = configuration.GetConnectionString("InvControlDB");
        }

        [HttpGet]
        public IActionResult GetCanalesVentas()
        {
            List<CanalVenta> canales = new();
            DA_CanalVenta da = new(connectionString);
            using (DataTable dt = da.ObtenerCanalesVentas(null, null))
            {
                foreach (DataRow dr in dt.Rows)
                {
                    CanalVenta c = new()
                    {
                        IdCanalVenta = (int)dr["IdCanalVenta"],
                        Codigo = (int)dr["Codigo"],
                        Nombre = (string)dr["Nombre"]
                    };
                    if (dr["Descripcion"] != DBNull.Value) c.Descripcion = (string)dr["Descripcion"];

                    canales.Add(c);
                }
            }
            return Ok(canales);
        }

        [HttpPost]
        public IActionResult PostCanalesVentas(CanalVenta canalVenta)
        {
            SqlTransaction transaction = null;
            try
            {
                DA_CanalVenta daC = new(connectionString);
                DA_Auditoria daAu = new(connectionString);

                if (daC.ObtenerCanalesVentas(canalVenta.Codigo, null).Rows.Count > 0)
                    ModelState.AddModelError(nameof(CanalVenta.Codigo), "El código ya se encuentra registrado");

                if (daC.ObtenerCanalesVentas(null, canalVenta.Nombre.Trim()).Rows.Count > 0)
                    ModelState.AddModelError(nameof(CanalVenta.Nombre), "Ya existe un canal de venta con la misma descripción");

                if (ModelState.IsValid)
                {
                    using (SqlConnection cnn = new(connectionString))
                    {
                        cnn.Open();
                        transaction = cnn.BeginTransaction();

                        canalVenta.IdCanalVenta = daC.InsertarCanalesVentas((int)canalVenta.Codigo!, canalVenta.Nombre.Trim(), canalVenta.Descripcion?.Trim(), transaction);

                        daAu.Insertar($"Se creó el canal de venta {canalVenta.Codigo}", DateTime.Now, (int)TipoEntidad.CanalVenta, (int)TipoOperacion.Creacion,
                            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)), transaction);

                        transaction.Commit();
                        cnn.Close();
                    }

                    return Ok(canalVenta);
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
        public IActionResult PutCanalesVentas(CanalVenta canalVenta)
        {
            SqlTransaction transaction = null;
            try
            {
                DA_CanalVenta daC = new(connectionString);
                DA_Auditoria daAu = new(connectionString);

                using (DataTable dt = daC.ObtenerCanalesVentas(canalVenta.Codigo, null))
                {
                    if (dt.Rows.Count > 0 && (int)dt.Rows[0]["IdCanalVenta"] != canalVenta.IdCanalVenta)
                        ModelState.AddModelError(nameof(CanalVenta.Codigo), "El código ya se encuentra registrado");
                }

                using (DataTable dt = daC.ObtenerCanalesVentas(null, canalVenta.Nombre))
                {
                    if (dt.Rows.Count > 0 && (int)dt.Rows[0]["IdCanalVenta"] != canalVenta.IdCanalVenta)
                        ModelState.AddModelError(nameof(CanalVenta.Nombre), "Ya existe un canal de venta con la misma descripción");
                }

                if (ModelState.IsValid)
                {
                    using (SqlConnection cnn = new(connectionString))
                    {
                        cnn.Open();
                        transaction = cnn.BeginTransaction();

                        daC.ModificarCanalesVentas(canalVenta.IdCanalVenta, (int)canalVenta.Codigo!, canalVenta.Nombre.Trim(), canalVenta.Descripcion?.Trim(), transaction);

                        daAu.Insertar($"Se editó el canal de venta {canalVenta.Codigo}", DateTime.Now, (int)TipoEntidad.CanalVenta, (int)TipoOperacion.Edicion,
                            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)), transaction);

                        transaction.Commit();
                        cnn.Close();
                    }

                    return Ok();
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
