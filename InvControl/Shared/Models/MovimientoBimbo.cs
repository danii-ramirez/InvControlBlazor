namespace InvControl.Shared.Models
{
    public class MovimientoBimbo
    {
        public int? CanalVenta { get; set; }
        public string NumeroRemito { get; set; } = string.Empty;
        public int CodigoSku { get; set; }
        public string NombreSku { get; set; } = string.Empty;
        public int Cantidad { get; set; }
        public string TipoEstoque { get; set; } = string.Empty;
        public string MotivoAjuste { get; set; } = string.Empty;
        public bool SkuNoEncontrado { get; set; }
        public bool CanalVentaNoEncontrado { get; set; }
    }
}
