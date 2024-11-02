using InvControl.Shared.Models;

namespace InvControl.Shared.DTO
{
    public class RemitoDTO : Remito
    {
        public string NombreTransporte { get; set; } = string.Empty;
        public string Patente { get; set; } = string.Empty;
        public string NombreChofer { get; set; } = string.Empty;
        public string ApellidoChofer { get; set; } = string.Empty;
        public string DescripcionEstado { get; set; } = string.Empty;
        public int IdUsuario { get; set; }
        public string NombreUsuario { get; set; } = string.Empty;
        public string ApellidoUsuario { get; set; } = string.Empty;
        public DateTime FechaIngreso { get; set; }
    }
}
