using System.ComponentModel.DataAnnotations;

namespace InvControl.Shared.Models
{
    public class Rol : ICloneable
    {
        public int IdRol { get; set; }

        [Required(ErrorMessage = "Debe ingresar una descripción"),
            StringLength(50, ErrorMessage = "La descripción no puede superar los 50 caracteres")]
        public string Descripcion { get; set; } = string.Empty;

        [Required(ErrorMessage = "Debe seleccionar al menos un permiso"),
            MinLength(2, ErrorMessage = "Debe seleccionar al menos un permiso")]
        public List<Permiso> Permisos { get; set; } = new();

        public object Clone()
        {
            List<Permiso> p = new();
            foreach (var item in Permisos)
            {
                p.Add(item);
            }

            var clone = (Rol)MemberwiseClone();
            clone.Permisos = p;
            return clone;
        }
    }
}
