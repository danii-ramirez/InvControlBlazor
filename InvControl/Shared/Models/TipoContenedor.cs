using System.ComponentModel.DataAnnotations;

namespace InvControl.Shared.Models
{
    public class TipoContenedor
    {
        public int IdTipoContenedor { get; set; }

        [Required(ErrorMessage = "Debe completar este campo"),
            StringLength(50, ErrorMessage = "Cantidad de máxima de caracteres 50")]
        public string Nombre { get; set; } = string.Empty;
    }
}
