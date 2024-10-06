﻿using InvControl.Server.Data;
using InvControl.Server.Helpers;
using InvControl.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Security.Claims;
using System.Text;

namespace InvControl.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly ILogger<UsuariosController> _logger;
        private readonly string connectionString;

        public UsuariosController(ILogger<UsuariosController> logger, IConfiguration configuration)
        {
            _logger = logger;
            connectionString = configuration.GetConnectionString("InvControlDB");
        }

        [HttpGet]
        public ActionResult GetUsuarios()
        {
            List<Usuario> usuarios = new();
            DA_Usuario da = new(connectionString);

            using (DataTable dt = da.ObtenerUsuario(null, null))
            {
                foreach (DataRow dr in dt.Rows)
                {
                    Usuario usuario = new()
                    {
                        IdUsuario = (int)dr["IdUsuario"],
                        User = (string)dr["User"],
                        Nombre = (string)dr["Nombre"],
                        Apellido = (string)dr["Apellido"],
                        Activo = (bool)dr["Activo"],
                        Bloqueado = (bool)dr["Bloqueado"],
                        IdRol = (int)dr["IdRol"],
                        DescripcionRol = (string)dr["DescripcionRol"]
                    };
                    usuarios.Add(usuario);
                }
            }

            return Ok(usuarios);
        }

        [HttpPost]
        public IActionResult PostUsuario(Usuario usuario)
        {
            SqlTransaction transaction = default!;
            try
            {
                DA_Usuario da = new(connectionString);
                DA_Auditoria daAu = new(connectionString);

                if (da.ObtenerUsuario(null, usuario.User.Trim()).Rows.Count > 0)
                    ModelState.AddModelError(nameof(Usuario.User), "Ya existe el usuario");

                if (ModelState.IsValid)
                {
                    Hashing hashing = new();
                    string pass = "12345";
                    string passHash = hashing.HashPassword(usuario.User.ToLower().Trim(), Convert.ToBase64String(Encoding.UTF8.GetBytes(pass)));

                    using (SqlConnection cnn = new(connectionString))
                    {
                        cnn.Open();
                        transaction = cnn.BeginTransaction();

                        da.InsertarUsuario(usuario.User.Trim(), passHash, usuario.Nombre.Trim(), usuario.Apellido.Trim(), usuario.Activo, usuario.IdRol, transaction);

                        daAu.Insertar($"Se creó el usuario {usuario.User.ToLower().Trim()}", DateTime.Now, (int)TipoEntidad.Usuario, (int)TipoOperacion.Creacion,
                            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)), transaction);

                        transaction.Commit();
                        cnn.Close();
                    }

                    return Ok(usuario);
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
        public IActionResult PutUsuario(Usuario usuario)
        {
            SqlTransaction transaction = default!;
            try
            {
                DA_Usuario da = new(connectionString);
                DA_Auditoria daAu = new(connectionString);

                using (DataTable dt = da.ObtenerUsuario(null, usuario.User.Trim()))
                {
                    if (dt.Rows.Count > 0 && (int)dt.Rows[0]["IdUsuario"] != usuario.IdUsuario)
                        ModelState.AddModelError(nameof(Usuario.User), "Ya existe el usuario");
                }

                if (ModelState.IsValid)
                {
                    using (SqlConnection cnn = new(connectionString))
                    {
                        cnn.Open();
                        transaction = cnn.BeginTransaction();

                        da.ModificarUsuario(usuario.IdUsuario, usuario.User.Trim(), usuario.Nombre, usuario.Apellido, usuario.Activo, usuario.IdRol, transaction);

                        daAu.Insertar($"Se editó el usuario {usuario.User.ToLower().Trim()}", DateTime.Now, (int)TipoEntidad.Usuario, (int)TipoOperacion.Edicion,
                            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)), transaction);

                        transaction.Commit();
                        cnn.Close();
                    }

                    return Ok(usuario);
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

        [HttpPost("resetpassword")]
        public IActionResult PostResetPassword(LoginUserResetPassword user)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    DA_Usuario da = new(connectionString);
                    Hashing hashing = new();

                    user.NewPassword = user.NewPassword.Trim();

                    using (DataTable dt = da.ObtenerUsuario(user.IdUsuario, null))
                    {
                        var hash = hashing.HashPassword((string)dt.Rows[0]["User"], Convert.ToBase64String(Encoding.UTF8.GetBytes(user.NewPassword)));
                        da.ResetearPass(user.IdUsuario, hash);
                    }

                    return Ok();
                }

                return BadRequest(ModelState);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{msg}", ex.Message);
                return StatusCode(500, ex);
            }
        }
    }
}
