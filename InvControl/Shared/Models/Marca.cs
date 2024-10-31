using System.ComponentModel.DataAnnotations;

namespace InvControl.Shared.Models
{
    public class Marca
    {
        public int IdMarca { get; set; }

        [Required(ErrorMessage = "Debe completar este campo"),
            StringLength(50, ErrorMessage = "Longitud máxima 50 caracteres")]
        public string Descripcion { get; set; } = string.Empty;
    }
}
