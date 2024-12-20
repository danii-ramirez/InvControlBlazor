namespace InvControl.Shared.Models
{
    public class RemitoDetalle
    {
        public int IdRemito { get; set; }
        public int IdSku { get; set; }
        public int? Codigo { get; set; }
        public string NombreSku { get; set; } = string.Empty;
        public int? Cantidad { get; set; }
        public string CodigoNombre => $"{Codigo} - {NombreSku}";
    }
}
