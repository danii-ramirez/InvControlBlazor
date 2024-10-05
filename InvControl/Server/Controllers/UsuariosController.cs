using InvControl.Server.Data;
using InvControl.Server.Helpers;
using InvControl.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data;
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

                    using (DataTable dt = da.ObtenerUsuario(user.IdUsuario))
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
