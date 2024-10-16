namespace InvControl.Shared.Models
{
    public class Remito
    {
        public int IdRemito { get; set; }
        public string Numero { get; set; } = string.Empty;
        public DateTime Fecha { get; set; }
        public int IdTransporte { get; set; }
        public int IdChofer { get; set; }
        public int IdEstado { get; set; }
        public List<RemitoDetalle> Detalle { get; set; } = new();
    }
}
