namespace InvControl.Shared.DTO
{
    public class MovimientoStockReporteDTO
    {
        public string Movimiento { get; set; } = string.Empty;
        public int Codigo { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public decimal Cantidad { get; set; }
        public DateTime Fecha { get; set; }
        public string Referencia { get; set; } = string.Empty;
    }
}
