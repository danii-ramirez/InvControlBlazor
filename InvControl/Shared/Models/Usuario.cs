using InvControl.Shared.Helpers;
using System.ComponentModel.DataAnnotations;

namespace InvControl.Shared.Models
{
    public class Usuario : ICloneable
    {
        public int IdUsuario { get; set; }

        [Required(ErrorMessage = "Debe ingresar un usuario"),
            StringLength(18, ErrorMessage = "El usuario no puede superar los 50 caracteres")]
        public string User { get; set; } = string.Empty;

        [Required(ErrorMessage = "Debe ingresar un nombre"),
            StringLength(50, ErrorMessage = "El nombre no puede superar los 50 caracteres"),
            RegularExpression(ExpresionesRegulares.LETRAS_ESPACIOS_CE, ErrorMessage = "El nombre solo debe contener letras")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "Debe ingresar un apellido"),
            StringLength(50, ErrorMessage = "El apellido no puede superar los 50 caracteres"),
            RegularExpression(ExpresionesRegulares.LETRAS_ESPACIOS_CE, ErrorMessage = "El nombre solo debe contener letras")]
        public string Apellido { get; set; } = string.Empty;

        public bool Activo { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un rol"),
            Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un rol")]
        public int IdRol { get; set; }

        public string DescripcionRol { get; set; } = string.Empty;

        public bool Bloqueado { get; set; }

        public bool ResetPas { get; set; }

        public string NombreCompleto => $"{Nombre} {Apellido}";

        public object Clone() => MemberwiseClone();
    }
}
