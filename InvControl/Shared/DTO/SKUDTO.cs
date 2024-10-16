namespace InvControl.Shared.DTO
{
    public class SKUDTO
    {
        public int IdSku { get; set; }
        public int? Codigo { get; set; }
        public string NombreSku { get; set; } = string.Empty;
        public string CodigoNombre => $"{Codigo} - {NombreSku}";
    }
}
