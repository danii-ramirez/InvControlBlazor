namespace InvControl.Shared.Models
{
    public class Transporte
    {
        public int IdTransporte { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Patente { get; set; } = string.Empty;
        public bool Activo { get; set; }
    }
}
