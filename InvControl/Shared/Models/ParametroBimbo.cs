using System.ComponentModel.DataAnnotations;

namespace InvControl.Shared.Models
{
    public class ParametroBimbo : ICloneable
    {
        public int IdParametroBimbo { get; set; }

        [Required(ErrorMessage = "Debe completar este campo"),
            StringLength(50, ErrorMessage = "Longitud máxima 50 caracteres")]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(255, ErrorMessage = "Longitud máxima 255 caracteres")]
        public string Descripcion { get; set; } = string.Empty;

        public bool EsMotivoAjuste { get; set; }

        public object Clone() => MemberwiseClone();
    }
}
