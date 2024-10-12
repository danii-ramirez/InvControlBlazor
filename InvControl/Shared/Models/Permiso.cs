namespace InvControl.Shared.Models
{
    public class Permiso
    {
        public int IdPermiso { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public int? IdPadre { get; set; }
        public string? Icon { get; set; }
        public string? Url { get; set; }
        public List<Permiso> Permisos { get; set; } = new();
        public bool Checked { get; set; }
    }
}
