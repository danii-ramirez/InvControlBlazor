using System.ComponentModel.DataAnnotations;

namespace InvControl.Shared.Models
{
    public class Remito
    {
        public int IdRemito { get; set; }

        [Required(ErrorMessage = "Debe completar este campo")]
        public string NumeroRemito { get; set; } = string.Empty;

        [Required(ErrorMessage = "Debe completar este campo")]
        public DateTime? FechaRemito { get; set; }

        public int? IdTransporte { get; set; }

        public int? IdChofer { get; set; }

        public int IdEstado { get; set; }

        [MinLength(1, ErrorMessage = "Debe ingresar al menos una línea")]
        public List<RemitoDetalle> Detalle { get; set; } = new();
    }
}
