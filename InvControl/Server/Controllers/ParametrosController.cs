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
                    };
                    if (dr["Descripcion"] != DBNull.Value) parametro.Descripcion = (string)dr["Descripcion"];
                    if (dr["Valor"] != DBNull.Value) parametro.Valor = (string)dr["Valor"];
                }
            }

            return parametro;
        }

        [HttpGet("bimbo")]
        public IActionResult GetParametrosBimbo()
        {
            List<ParametroBimbo> parametros = new();
            using (DataTable dt = new DA_Parametro(connectionString).ObtenerParametrosBimbo(null, null, null))
            {
                foreach (DataRow dr in dt.Rows)
                {
                    ParametroBimbo parametroBimbo = new()
                    {
                        IdParametroBimbo = (int)dr["IdBimboParametro"],
                        Nombre = (string)dr["Nombre"],
                        EsMotivoAjuste = (bool)dr["EsMotivoAjuste"]
                    };
                    if (dr["Descripcion"] != DBNull.Value) parametroBimbo.Descripcion = (string)dr["Descripcion"];

                    parametros.Add(parametroBimbo);
                }
            }
            return Ok(parametros);
        }

        [HttpPost("bimbo")]
        public IActionResult PostParametroBimbo(ParametroBimbo parametro)
        {
            SqlTransaction transaction = null;
            try
            {
                DA_Parametro daP = new(connectionString);

                if (daP.ObtenerParametrosBimbo(null, parametro.Nombre.Trim(), parametro.EsMotivoAjuste).Rows.Count > 0)
                    ModelState.AddModelError(nameof(ParametroBimbo.Nombre), "El nombre ya se encuentra registrado");

                if (ModelState.IsValid)
                {
                    using (SqlConnection cnn = new(connectionString))
                    {
                        cnn.Open();
                        transaction = cnn.BeginTransaction();

                        parametro.IdParametroBimbo = daP.InsertarParametrosBimbo(parametro.Nombre.Trim(), parametro.Descripcion?.Trim(), parametro.EsMotivoAjuste, transaction);

                        transaction.Commit();
                        cnn.Close();
                    }

                    return Ok(parametro);
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

        [HttpPut("bimbo")]
        public IActionResult PutParametroBimbo(ParametroBimbo parametro)
        {
            SqlTransaction transaction = null;
            try
            {
                DA_Parametro daP = new(connectionString);

                using (DataTable dt = daP.ObtenerParametrosBimbo(null, parametro.Nombre.Trim(), parametro.EsMotivoAjuste))
                {
                    if (dt.Rows.Count > 0 && (int)dt.Rows[0]["IdBimboParametro"] != parametro.IdParametroBimbo)
                        ModelState.AddModelError(nameof(ParametroBimbo.Nombre), "El nombre ya se encuentra registrado");
                }

                if (ModelState.IsValid)
                {
                    using (SqlConnection cnn = new(connectionString))
                    {
                        cnn.Open();
                        transaction = cnn.BeginTransaction();

                        daP.ActualizarParametroBimbo(parametro.IdParametroBimbo, parametro.Nombre.Trim(), parametro.Descripcion?.Trim(), parametro.EsMotivoAjuste, transaction);

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

        [HttpDelete("bimbo/{idParametroBimbo:int}")]
        public IActionResult DeleteParametroBimbo(int idParametroBimbo)
        {
            SqlTransaction transaction = null;
            try
            {
                DA_Parametro daP = new(connectionString);

                ParametroBimbo parametroBimbo;

                using (DataTable dt = daP.ObtenerParametrosBimbo(idParametroBimbo, null, null))
                {
                    DataRow dr = dt.Rows[0];

                    parametroBimbo = new()
                    {
                        IdParametroBimbo = (int)dr["IdBimboParametro"],
                        Nombre = (string)dr["Nombre"],
                        EsMotivoAjuste = (bool)dr["EsMotivoAjuste"]
                    };
                    if (dr["Descripcion"] != DBNull.Value) parametroBimbo.Descripcion = (string)dr["Descripcion"];
                }

                using (SqlConnection cnn = new(connectionString))
                {
                    cnn.Open();
                    transaction = cnn.BeginTransaction();

                    daP.EliminarParametroBimbo(parametroBimbo.IdParametroBimbo, parametroBimbo.EsMotivoAjuste, transaction);

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
