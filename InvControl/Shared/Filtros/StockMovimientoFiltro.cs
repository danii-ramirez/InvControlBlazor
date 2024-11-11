namespace InvControl.Shared.Filtros
{
    public class StockMovimientoFiltro
    {
        public int IdTipoMovimiento { get; set; }
        public int? Codigo { get; set; }
        public string? Nombre { get; set; }
        public DateTime FechaDesde { get; set; }
        public DateTime FechaHasta { get; set; }
        public int? IdCanalVenta { get; set; }
    }
}
