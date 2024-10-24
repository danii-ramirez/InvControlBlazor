using System.ComponentModel.DataAnnotations;

namespace InvControl.Shared.Models
{
    public class Remito
    {
        public int IdRemito { get; set; }

        [Required(ErrorMessage = "Debe completar este campo")]
        public string Numero { get; set; } = string.Empty; //TODO: renombrar NumeroRemito

        [Required(ErrorMessage = "Debe completar este campo")]
        public DateTime? Fecha { get; set; } //TODO: renombrar FechaRemito

        public int? IdTransporte { get; set; }

        public int? IdChofer { get; set; }

        public int IdEstado { get; set; }

        [MinLength(1, ErrorMessage = "Debe ingresar al menos una l�nea")]
        public List<RemitoDetalle> Detalle { get; set; } = new();
    }
}
