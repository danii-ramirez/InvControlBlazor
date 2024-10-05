namespace InvControl.Shared.Models
{
    public class Permiso
    {
        public int IdPermiso { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public int? IdPadre { get; set; }
        public List<Permiso> Permisos { get; set; } = new();
    }
}
