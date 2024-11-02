using InvControl.Server.Data;
using InvControl.Server.Helpers;
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
    public class TransportesController : ControllerBase
    {
        private readonly ILogger<TransportesController> _logger;
        private readonly string connectionString;

        public TransportesController(ILogger<TransportesController> logger, IConfiguration configuration)
        {
            _logger = logger;
            connectionString = configuration.GetConnectionString("InvControlDB");
        }

        [HttpGet]
        public ActionResult GetTransportes()
        {
            List<Transporte> transportes = new();
            DA_Transporte da = new(connectionString);
            using (DataTable dt = da.ObtenerTransportes(null, null, null))
            {
                foreach (DataRow dr in dt.Rows)
                {
                    Transporte t = new()
                    {
                        IdTransporte = (int)dr["IdTransporte"],
                        Nombre = (string)dr["Nombre"],
                        Patente = (string)dr["Patente"],
                        Activo = (bool)dr["Activo"]
                    };
                    transportes.Add(t);
                }
            }
            return Ok(transportes);
        }

        [HttpPost]
        public IActionResult PostTransportes(Transporte transporte)
        {
            SqlTransaction transaction = null;
            try
            {
                DA_Transporte daT = new(connectionString);
                DA_Auditoria daAu = new(connectionString);

                if (daT.ObtenerTransportes(null, transporte.Patente.Trim(), null).Rows.Count > 0)
                    ModelState.AddModelError(nameof(Transporte.Patente), "La patente ya se encuentra registrado");

                if (ModelState.IsValid)
                {
                    using (SqlConnection cnn = new(connectionString))
                    {
                        cnn.Open();
                        transaction = cnn.BeginTransaction();

                        transporte.IdTransporte = daT.InsertarTransportes(transporte.Nombre.Trim(), transporte.Patente.Trim(), transporte.Activo, transaction);

                        daAu.Insertar($"Se creó el transporte {transporte.Patente.Trim()}", DateTime.Now, (int)TipoEntidad.Transporte, (int)TipoOperacion.Creacion,
                            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)), transaction);

                        transaction.Commit();
                        cnn.Close();
                    }

                    return Ok(transporte);
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
        public IActionResult PutTransportes(Transporte transporte)
        {
            SqlTransaction transaction = null;
            try
            {
                DA_Transporte daT = new(connectionString);
                DA_Auditoria daAu = new(connectionString);

                using (DataTable dt = daT.ObtenerTransportes(null, transporte.Patente.Trim(), null))
                {
                    if (dt.Rows.Count > 0 && (int)dt.Rows[0]["IdTransporte"] != transporte.IdTransporte)
                        ModelState.AddModelError(nameof(Transporte.Patente), "La patente ya se encuentra registrado");
                }

                if (ModelState.IsValid)
                {
                    using (SqlConnection cnn = new(connectionString))
                    {
                        cnn.Open();
                        transaction = cnn.BeginTransaction();

                        daT.ModificarTransportes(transporte.IdTransporte, transporte.Nombre.Trim(), transporte.Patente.Trim(), transporte.Activo, transaction);

                        daAu.Insertar($"Se editó el transporte {transporte.Patente.Trim()}", DateTime.Now, (int)TipoEntidad.Transporte, (int)TipoOperacion.Edicion,
                            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)), transaction);

                        transaction.Commit();
                        cnn.Close();
                    }

                    return Ok(transporte);
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
