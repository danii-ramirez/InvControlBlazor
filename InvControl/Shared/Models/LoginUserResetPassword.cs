using System.ComponentModel.DataAnnotations;

namespace InvControl.Shared.Models
{
    public class LoginUserResetPassword
    {
        public int IdUsuario { get; set; }

        [Required(ErrorMessage = "Debe completar este campo")]
        [StringLength(18, ErrorMessage = "La {0} debe tener al menos {2} y un máximo de {1} caracteres.", MinimumLength = 4)]
        [DataType(DataType.Password)]
        [Display(Name = "Nueva contraseña")]
        public string NewPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Debe completar este campo")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirmar contraseña")]
        [Compare("NewPassword", ErrorMessage = "La nueva contraseña y la confirmación no coinciden.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
