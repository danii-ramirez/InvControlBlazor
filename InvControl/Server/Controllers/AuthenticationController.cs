using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using InvControl.Shared.Models;

namespace InvControl.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUser model)
        {
            if (model.Username == "admin" && model.Password == "admin")
            {
                var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, model.Username)
            };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true
                };

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

                return Ok(new { message = "Inicio de sesión exitoso" });
            }

            return Unauthorized(new { message = "Credenciales inválidas" });
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok(new { message = "Cierre de sesión exitoso" });
        }

        [HttpGet("currentUser")]
        public IActionResult GetCurrentUser()
        {
            if (User.Identity?.IsAuthenticated ?? false)
            {
                return Ok(new { username = User.Identity.Name });
            }
            return Unauthorized();
        }
    }
}
