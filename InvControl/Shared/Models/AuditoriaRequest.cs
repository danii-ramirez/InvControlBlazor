using InvControl.Shared.Helpers;

namespace InvControl.Shared.Models
{
    public class AuditoriaRequest
    {
        public int? IdUsuario { get; set; }
        public TipoEntidad? TipoEntidad { get; set; }
        public DateTime FechaDesde { get; set; }
        public DateTime FechaHasta { get; set; }
    }
}
