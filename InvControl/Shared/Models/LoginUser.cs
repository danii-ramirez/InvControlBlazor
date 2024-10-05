using System.ComponentModel.DataAnnotations;

namespace InvControl.Shared.Models
{
    public class LoginUser
    {
        [Required(ErrorMessage = "Debe ingresar un usuario")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Debe ingresar una contraseña")]
        public string Password { get; set; } = string.Empty;
    }
}
