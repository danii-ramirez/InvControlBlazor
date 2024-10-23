namespace InvControl.Shared.Models
{
    public class Parametro
    {
        public int IdParametro { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string Valor { get; set; } = string.Empty;
    }
}
