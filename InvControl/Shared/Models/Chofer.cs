using InvControl.Shared.Helpers;
using System.ComponentModel.DataAnnotations;

namespace InvControl.Shared.Models
{
    public class Chofer : ICloneable
    {
        public int IdChofer { get; set; }

        [Required(ErrorMessage = "Debe completar este campo"),
            StringLength(50, ErrorMessage = "Longitud máxima 50 caracteres"),
            RegularExpression(ExpresionesRegulares.LETRAS_ESPACIOS_CE, ErrorMessage = "El nombre solo debe contener letras")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "Debe completar este campo"),
            StringLength(50, ErrorMessage = "Longitud máxima 50 caracteres"),
            RegularExpression(ExpresionesRegulares.LETRAS_ESPACIOS_CE, ErrorMessage = "El apellido solo debe contener letras")]
        public string Apellido { get; set; } = string.Empty;

        public bool Activo { get; set; }

        public string NombreCompleto => $"{Nombre} {Apellido}";

        public object Clone() => MemberwiseClone();
    }
}
