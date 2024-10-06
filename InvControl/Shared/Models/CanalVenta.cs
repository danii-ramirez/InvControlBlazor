using System.ComponentModel.DataAnnotations;

namespace InvControl.Shared.Models
{
    public class CanalVenta : ICloneable
    {
        public int IdCanalVenta { get; set; }

        [Required(ErrorMessage = "Debe ingresar un código"),
            Range(1, int.MaxValue, ErrorMessage = "Debe ingresar un código")]
        public int? Codigo { get; set; }

        [Required(ErrorMessage = "Debe ingresar un nombre"),
            StringLength(25, ErrorMessage = "El nombre no puede superar los 25 caracteres")]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(50, ErrorMessage = "La descripción no puede superar los 50 caracteres")]
        public string Descripcion { get; set; } = string.Empty;

        public object Clone() => MemberwiseClone();
    }
}
