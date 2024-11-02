using InvControl.Server.Data;
using InvControl.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace InvControl.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ParametrosController : ControllerBase
    {
        private readonly ILogger<ParametrosController> _logger;
        private readonly string connectionString;

        public ParametrosController(ILogger<ParametrosController> logger = null, IConfiguration configuration = null, string connectionString = null)
        {
            _logger = logger!;

            if (configuration != null)
                this.connectionString = configuration.GetConnectionString("InvControlDB");
            else
                this.connectionString = connectionString!;
        }

        [HttpGet]
        public IActionResult GetParametros()
        {
            List<Parametro> parametros = new();

            using (DataTable dt = new DA_Parametro(connectionString).ObtenerParametros(null, null))
            {
                foreach (DataRow dr in dt.Rows)
                {
                    Parametro p = new()
                    {
                        IdParametro = (int)dr["IdParametro"],
                        Nombre = (string)dr["Nombre"],
                        Descripcion = (string)dr["Descripcion"],
                        Valor = (string)dr["Valor"]
                    };

                    parametros.Add(p);
                }
            }

            return Ok(parametros);
        }

        [HttpGet("{nombre}")]
        public IActionResult GetParametros(string nombre)
        {
            return Ok(ObtenerParametro(nombre));
        }

        [HttpPut]
        public IActionResult PutParametro(Parametro parametro)
        {
            SqlTransaction transaction = null;
            try
            {
                DA_Parametro daP = new(connectionString);

                using (SqlConnection cnn = new(connectionString))
                {
                    cnn.Open();
                    transaction = cnn.BeginTransaction();

                    daP.ActualizarParametro(parametro.IdParametro, parametro.Valor, transaction);

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

        internal Parametro ObtenerParametro(string nombre)
        {
            Parametro parametro = null;
            using (DataTable dt = new DA_Parametro(connectionString).ObtenerParametros(null, nombre))
            {
                if (dt.Rows.Count > 0)
                {
                    DataRow dr = dt.Rows[0];

                    parametro = new()
                    {
                        IdParametro = (int)dr["IdParametro"],
                        Nombre = (string)dr["Nombre"],
                        Descripcion = (string)dr["Descripcion"],
                        Valor = (string)dr["Valor"]
                    };
                }
            }

            return parametro;
        }
    }
}
