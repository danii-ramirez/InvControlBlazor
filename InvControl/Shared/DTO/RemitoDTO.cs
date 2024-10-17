using InvControl.Shared.Models;

namespace InvControl.Shared.DTO
{
    public class RemitoDTO
    {
        public int IdRemito { get; set; }
        public string Numero { get; set; } = string.Empty;
        public DateTime Fecha { get; set; }
        public int? IdTransporte { get; set; }
        public string NombreTransporte { get; set; } = string.Empty;
        public string Patente { get; set; } = string.Empty;
        public int? IdChofer { get; set; }
        public string NombreChofer { get; set; } = string.Empty;
        public string ApellidoChofer { get; set; } = string.Empty;
        public int IdEstado { get; set; }
        public string DescripcionEstado { get; set; } = string.Empty;
        public int IdUsuario { get; set; }
        public string NombreUsuario { get; set; } = string.Empty;
        public string ApellidoUsuario { get; set; } = string.Empty;
        public DateTime FechaIngreso { get; set; }
        public List<RemitoDetalle> Detalle { get; set; } = new();
    }
}
