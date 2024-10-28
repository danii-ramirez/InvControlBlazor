using InvControl.Server.Data;
using InvControl.Server.Helpers;
using InvControl.Shared.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text;

namespace InvControl.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly ILogger<AuthenticationController> _logger;
        private readonly string connectionString;

        public AuthenticationController(ILogger<AuthenticationController> logger, IConfiguration configuration)
        {
            _logger = logger;
            connectionString = configuration.GetConnectionString("InvControlDB");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginUser user)
        {
            try
            {
                DA_Usuario da = new(connectionString);
                DA_Auditoria daAu = new(connectionString);

                Hashing hashing = new();

                user.Username = user.Username.ToLower().Trim();
                user.Password = user.Password.Trim();

                var dtUser = da.Login(user.Username);
                if (dtUser.Rows.Count > 0)
                {
                    var dr = dtUser.Rows[0];

                    if (!(bool)dr["Bloqueado"])
                    {
                        if (hashing.VerifyPassword(user.Username, (string)dr["Pass"], Convert.ToBase64String(Encoding.UTF8.GetBytes(user.Password))))
                        {
                            if (!(bool)dr["Activo"])
                            {
                                return BadRequest(new LoginUserResponse(Status.Failed, new() { { nameof(LoginUser.Username), new() { "Usuario inactivo" } } }));
                            }
                            else if ((bool)dr["ResetearPass"])
                            {
                                return BadRequest(new LoginUserResponse(Status.ResetPassword) { IdUsuario = (int)dr["IdUsuario"] });
                            }
                            else
                            {
                                da.ResetearIntentosFallidos((int)dr["IdUsuario"]);

                                CurrentUser currentUser = new()
                                {
                                    IdUsuario = (int)dr["IdUsuario"],
                                    Nombre = (string)dr["Nombre"],
                                    Apellido = (string)dr["Apellido"]
                                };

                                var claims = new List<Claim> {
                                    new(ClaimTypes.NameIdentifier, currentUser.IdUsuario.ToString()),
                                    new(ClaimTypes.Name, currentUser.Nombre),
                                    new(ClaimTypes.Surname, currentUser.Apellido)
                                };

                                daAu.Insertar("Inició sesión", DateTime.Now, (int)TipoEntidad.Login, (int)TipoOperacion.InicioSesion, (int)dr["IdUsuario"]);

                                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                                return Ok(currentUser);
                            }
                        }
                        else
                        {
                            var intentosFallidos = da.IncrementarIntentosFallidos((int)dr["IdUsuario"]);
                            if (intentosFallidos < 3)
                                return BadRequest(new LoginUserResponse(Status.Failed, new() { { nameof(LoginUser.Password), new() { "Contraseña invalida" } } }));
                            else
                                return BadRequest(new LoginUserResponse(Status.ResetPassword));
                        }
                    }
                    else
                    {
                        return BadRequest(new LoginUserResponse(Status.Blocked));
                    }
                }
                else
                {
                    return BadRequest(new LoginUserResponse(Status.Failed, new() { { nameof(LoginUser.Username), new() { "Usuario invalido" } } }));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{msg}", ex.Message);
                return StatusCode(500, ex);
            }
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            if (User.Identity?.IsAuthenticated ?? false)
            {
                DA_Auditoria daAu = new(connectionString);
                daAu.Insertar("Cerro sesión", DateTime.Now,
                    (int)TipoEntidad.Login, (int)TipoOperacion.CerrarSesion, int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)));
            }

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok(new { message = "Cierre de sesión exitoso" });
        }

        [HttpGet("currentUser")]
        public IActionResult GetCurrentUser()
        {
            if (User.Identity?.IsAuthenticated ?? false)
            {
                CurrentUser currentUser = new()
                {
                    IdUsuario = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)),
                    Nombre = User.FindFirstValue(ClaimTypes.Name),
                    Apellido = User.FindFirstValue(ClaimTypes.Surname)
                };

                return Ok(currentUser);
            }

            return Unauthorized();
        }
    }
}
