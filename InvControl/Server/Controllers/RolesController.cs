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
    public class RolesController : ControllerBase
    {
        private readonly ILogger<RolesController> _logger;
        private readonly string connectionString;

        public RolesController(ILogger<RolesController> logger, IConfiguration configuration)
        {
            _logger = logger;
            connectionString = configuration.GetConnectionString("InvControlDB");
        }

        [HttpGet]
        public IActionResult GetRoles(int? idRol, string descripcion)
        {
            List<Rol> roles = new();
            DA_Rol da = new(connectionString);
            using (DataTable dt = da.ObtenerRoles(idRol, descripcion))
            {
                foreach (DataRow dr in dt.Rows)
                {
                    Rol rol = new()
                    {
                        IdRol = (int)dr["IdRol"],
                        Descripcion = (string)dr["Descripcion"]
                    };

                    foreach (DataRow drP in da.ObtenerPermisosPorRol(rol.IdRol).Rows)
                    {
                        rol.Permisos.Add(new() { IdPermiso = (int)drP["IdPermiso"] });
                    }

                    roles.Add(rol);
                }
            }
            return Ok(roles);
        }

        [HttpPost]
        public IActionResult PostRol(Rol rol)
        {
            SqlTransaction transaction = default!;
            try
            {
                DA_Rol da = new(connectionString);
                DA_Auditoria daAu = new(connectionString);

                if (da.ObtenerRoles(null, rol.Descripcion.Trim()).Rows.Count > 0)
                    ModelState.AddModelError(nameof(Rol.Descripcion), "Ya existe un rol con la misma descripción");

                if (ModelState.IsValid)
                {
                    using (SqlConnection cnn = new(connectionString))
                    {
                        cnn.Open();
                        transaction = cnn.BeginTransaction();

                        rol.IdRol = da.InsertarRol(rol.Descripcion.Trim(), transaction);

                        foreach (var p in rol.Permisos)
                        {
                            da.InsertarRolesPorPermiso(rol.IdRol, p.IdPermiso, transaction);
                        }

                        daAu.Insertar($"Se creó el rol {rol.Descripcion.ToLower().Trim()}", DateTime.Now, (int)TipoEntidad.Rol, (int)TipoOperacion.Creacion,
                            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)), transaction);

                        transaction.Commit();
                        cnn.Close();
                    }

                    return Ok(rol);
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
        public IActionResult PutRol(Rol rol)
        {
            SqlTransaction transaction = default!;
            try
            {
                DA_Rol da = new(connectionString);
                DA_Auditoria daAu = new(connectionString);

                using (DataTable dt = da.ObtenerRoles(null, rol.Descripcion.Trim()))
                {
                    if (dt.Rows.Count > 0 && (int)dt.Rows[0]["IdRol"] != rol.IdRol)
                        ModelState.AddModelError(nameof(Rol.Descripcion), "Ya existe un rol con la misma descripción");
                }

                if (ModelState.IsValid)
                {
                    using (SqlConnection cnn = new(connectionString))
                    {
                        cnn.Open();
                        transaction = cnn.BeginTransaction();

                        da.ActualizarRol(rol.IdRol, rol.Descripcion.Trim(), transaction);
                        da.EliminarRolesPorPermiso(rol.IdRol, transaction);

                        foreach (var p in rol.Permisos)
                        {
                            da.InsertarRolesPorPermiso(rol.IdRol, p.IdPermiso, transaction);
                        }

                        daAu.Insertar($"Se editó el rol {rol.Descripcion.ToLower().Trim()}", DateTime.Now, (int)TipoEntidad.Rol, (int)TipoOperacion.Edicion,
                            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)), transaction);

                        transaction.Commit();
                        cnn.Close();
                    }

                    return Ok(rol);
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

        [HttpGet("permisos")]
        public IActionResult GetPermisos(bool jerarquico)
        {
            List<Permiso> permisos = new();
            DA_Rol da = new(connectionString);

            using (DataTable dt = da.ObtenerPermisos())
            {
                foreach (DataRow dr in dt.Rows)
                {
                    Permiso permiso = new()
                    {
                        IdPermiso = (int)dr["IdPermiso"],
                        Descripcion = (string)dr["Nombre"]
                    };
                    if (dr["IdPadre"] != DBNull.Value) permiso.IdPadre = (int?)dr["IdPadre"];

                    permisos.Add(permiso);
                }
            }
            
            if (jerarquico)
            {
                var permisosJerarquicos = Functions.ConstruirArbolPermiso(permisos);
                return Ok(permisosJerarquicos);
            }
            else
            {
                return Ok(permisos);
            }
        }
    }
}
