using System.ComponentModel.DataAnnotations;

namespace InvControl.Shared.Models
{
    public class Chofer : ICloneable
    {
        public int IdChofer { get; set; }

        [Required(ErrorMessage = "Debe completar este campo"),
            StringLength(50, ErrorMessage = "Longitud máxima 50 caracteres")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "Debe completar este campo"),
            StringLength(50, ErrorMessage = "Longitud máxima 50 caracteres")]
        public string Apellido { get; set; } = string.Empty;

        public bool Activo { get; set; }

        public string NombreCompleto => $"{Nombre} {Apellido}";

        public object Clone() => MemberwiseClone();
    }
}
