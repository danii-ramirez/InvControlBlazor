namespace InvControl.Shared.Models
{
    public class Stock
    {
        public int IdStock { get; set; }
        public int IdSku { get; set; }
        public int Codigo { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Marca { get; set; } = string.Empty;
        public int Cantidad { get; set; }
        public DateTime FechaActualizacion { get; set; }
    }
}
