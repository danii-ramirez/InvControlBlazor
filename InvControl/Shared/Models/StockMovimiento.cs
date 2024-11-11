namespace InvControl.Shared.Models
{
    public class StockMovimiento
    {
        public string TipoMovimiento { get; set; } = string.Empty;
        public int CodigoSku { get; set; }
        public string NombreSku { get; set; } = string.Empty;
        public int Cantidad { get; set; }
        public string Referencia { get; set; } = string.Empty;
        public DateTime FechaMovimiento { get; set; }
        public string NombreUsuario { get; set; } = string.Empty;
        public string NombreCanalVenta { get; set; } = string.Empty;
    }
}
