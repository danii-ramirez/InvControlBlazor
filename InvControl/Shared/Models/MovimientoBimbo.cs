namespace InvControl.Shared.Models
{
    public class MovimientoBimbo
    {
        public int? IdCanalVenta { get; set; }
        public string CanalVenta { get; set; } = string.Empty;
        public string NumeroRemito { get; set; } = string.Empty;
        public int? IdSku { get; set; }
        public string CodigoSku { get; set; } = string.Empty;
        public string NombreSku { get; set; } = string.Empty;
        public string NombreSkuOriginal { get; set; } = string.Empty;
        public string Cantidad { get; set; } = string.Empty;
        public string TipoEstoque { get; set; } = string.Empty;
        public string MotivoAjuste { get; set; } = string.Empty;
    }
}
