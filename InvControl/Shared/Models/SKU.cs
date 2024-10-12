using System.ComponentModel.DataAnnotations;

namespace InvControl.Shared.Models
{
    public class SKU
    {
        public int IdSKU { get; set; }

        [Required(ErrorMessage = "Debe completar este campo"),
            Range(1, int.MaxValue, ErrorMessage = "Debe ingresar un número valido")]
        public int? Codigo { get; set; }

        [Required(ErrorMessage = "Debe completar este campo"),
            StringLength(50, ErrorMessage = "Cantidad de máxima de caracteres 50")]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(100, ErrorMessage = "Cantidad de máxima de caracteres 100")]
        public string Descripcion { get; set; } = string.Empty;

        public bool Activo { get; set; }

        public bool Especial { get; set; }

        [Required(ErrorMessage = "Debe completar este campo"),
            Range(1, int.MaxValue, ErrorMessage = "Debe ingresar un número valido")]
        public int? UnidadesPorBandeja { get; set; }

        [Required]
        public int IdMarca { get; set; }
        public string DescripcionMarca { get; set; } = string.Empty;

        public int Stock { get; set; }
    }
}
