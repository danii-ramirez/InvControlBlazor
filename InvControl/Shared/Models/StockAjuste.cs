using System.ComponentModel.DataAnnotations;

namespace InvControl.Shared.Models
{
    public class StockAjuste
    {
        [Required(ErrorMessage = "Debe completar este campo"),
            Range(1, int.MaxValue, ErrorMessage = "Debe completar este campo")]
        public int IdTipoMovimiento { get; set; }

        public string Tipo { get; set; } = string.Empty;

        [Required(ErrorMessage = "Debe completar este campo")]
        public string Observaciones { get; set; } = string.Empty;

        [MinLength(1, ErrorMessage = "Debe ingresar al menos una línea")]
        public List<StockAjusteDetalle> Detalle { get; set; } = new();
    }
}
