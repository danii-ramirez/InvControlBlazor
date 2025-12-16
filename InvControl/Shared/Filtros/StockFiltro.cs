namespace InvControl.Shared.Filtros
{
    public class StockFiltro
    {
        public string Nombre { get; set; } = string.Empty;
        public int? StockMinimo { get; set; }
        public int? StockMaximo { get; set; }
        public int? IdMarca { get; set; }
        public bool? Estado { get; set; }
    }
}
