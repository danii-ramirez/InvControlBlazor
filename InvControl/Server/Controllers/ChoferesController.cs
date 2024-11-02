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
    public class ChoferesController : ControllerBase
    {
        private readonly ILogger<ChoferesController> _logger;
        private readonly string connectionString;

        public ChoferesController(ILogger<ChoferesController> logger, IConfiguration configuration)
        {
            _logger = logger;
            connectionString = configuration.GetConnectionString("InvControlDB");
        }

        [HttpGet]
        public IActionResult GetChoferes()
        {
            List<Chofer> choferes = new();
            DA_Chofer da = new(connectionString);

            using (DataTable dt = da.ObtenerChoferes(null, null, null))
            {
                foreach (DataRow dr in dt.Rows)
                {
                    Chofer c = new()
                    {
                        IdChofer = (int)dr["IdChofer"],
                        Nombre = (string)dr["Nombre"],
                        Apellido = (string)dr["Apellido"],
                        Activo = (bool)dr["Activo"]
                    };

                    choferes.Add(c);
                }
            }

            return Ok(choferes);
        }

        [HttpPost]
        public IActionResult PostChoferes(Chofer chofer)
        {
            SqlTransaction transaction = null;
            try
            {
                DA_Chofer daC = new(connectionString);
                DA_Auditoria daAu = new(connectionString);

                if (daC.ObtenerChoferes(chofer.Nombre.Trim(), chofer.Apellido.Trim(), null).Rows.Count > 0)
                    ModelState.AddModelError(nameof(CanalVenta.Codigo), "El chofer ya se encuentra registrado");

                if (ModelState.IsValid)
                {
                    using (SqlConnection cnn = new(connectionString))
                    {
                        cnn.Open();
                        transaction = cnn.BeginTransaction();

                        chofer.IdChofer = daC.InsertarChofer(chofer.Nombre.Trim(), chofer.Apellido.Trim(), chofer.Activo, transaction);

                        daAu.Insertar($"Se creó el chofer {chofer.Nombre.Trim()} {chofer.Apellido.Trim()}", DateTime.Now, (int)TipoEntidad.Chofer,
                            (int)TipoOperacion.Creacion, int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)), transaction);

                        transaction.Commit();
                        cnn.Close();
                    }

                    return Ok(chofer);
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
        public IActionResult Putchoferes(Chofer chofer)
        {
            SqlTransaction transaction = null;
            try
            {
                DA_Chofer daC = new(connectionString);
                DA_Auditoria daAu = new(connectionString);

                using (DataTable dt = daC.ObtenerChoferes(chofer.Nombre.Trim(), chofer.Apellido.Trim(), null))
                {
                    if (dt.Rows.Count > 0 && (int)dt.Rows[0]["IdChofer"] != chofer.IdChofer)
                        ModelState.AddModelError(nameof(CanalVenta.Codigo), "El chofer ya se encuentra registrado");
                }

                if (ModelState.IsValid)
                {
                    using (SqlConnection cnn = new(connectionString))
                    {
                        cnn.Open();
                        transaction = cnn.BeginTransaction();

                        daC.ModificarChofer(chofer.IdChofer, chofer.Nombre.Trim(), chofer.Apellido.Trim(), chofer.Activo, transaction);

                        daAu.Insertar($"Se editó el chofer {chofer.Nombre.Trim()} {chofer.Apellido.Trim()}", DateTime.Now, (int)TipoEntidad.Chofer,
                            (int)TipoOperacion.Edicion, int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)), transaction);

                        transaction.Commit();
                        cnn.Close();
                    }

                    return Ok(chofer);
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
