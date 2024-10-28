namespace InvControl.Shared.Models
{
    public class Stock
    {
        public int IdSKU { get; set; }
        public int Codigo { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public int Cantidad { get; set; }
    }
}
