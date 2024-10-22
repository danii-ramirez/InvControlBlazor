using System.ComponentModel.DataAnnotations;

namespace InvControl.Shared.Models
{
    public class LoginUser
    {
        [Required(ErrorMessage = "Debe ingresar un usuario"),
            StringLength(50, ErrorMessage = "La longitud no puede ser mayor a 50")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Debe ingresar una contraseña"),
            StringLength(50, ErrorMessage = "La longitud no puede ser mayor a 50")]
        public string Password { get; set; } = string.Empty;
    }
}
