namespace InvControl.Shared.DTO
{
    public class AuditoriaDTO
    {
        public int IdAuditoria { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public DateTime Fecha { get; set; }
        public string DescripcionEntidad { get; set; } = string.Empty;
        public string DescripcionOperacion { get; set; } = string.Empty;
        public string Usuario { get; set; } = string.Empty;
    }
}
