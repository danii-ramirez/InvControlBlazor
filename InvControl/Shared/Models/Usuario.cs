using System.ComponentModel.DataAnnotations;

namespace InvControl.Shared.Models
{
    public class Usuario : ICloneable
    {
        public int IdUsuario { get; set; }

        [Required(ErrorMessage = "Debe ingresar un usuario")]
        public string User { get; set; } = string.Empty;

        [Required(ErrorMessage = "Debe ingresar un nombre")]
        public string Nombre { get; set; } = string.Empty;


        [Required(ErrorMessage = "Debe ingresar un apellido")]
        public string Apellido { get; set; } = string.Empty;

        public bool Activo { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un rol"),
            Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un rol")]
        public int IdRol { get; set; }
        public string DescripcionRol { get; set; } = string.Empty;

        public bool Bloqueado { get; set; }

        public bool ResetPas { get; set; }

        public object Clone() => MemberwiseClone();
    }
}
