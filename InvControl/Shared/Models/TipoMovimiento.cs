using System.ComponentModel.DataAnnotations;

namespace InvControl.Shared.Models
{
    public class TipoMovimiento : ICloneable
    {
        public int IdTipoMovimiento { get; set; }

        [Required(ErrorMessage = "Debe completar este campo"),
            StringLength(50, ErrorMessage = "Cantidad de máxima de caracteres 50")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "Debe completar este campo")]
        public string Tipo { get; set; } = string.Empty;

        public bool SoloLectura { get; set; }
        public bool Interno { get; set; }

        public object Clone() => MemberwiseClone();
    }
}
