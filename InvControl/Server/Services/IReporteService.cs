using InvControl.Shared.DTO;

namespace InvControl.Server.Services
{
    public interface IReporteService
    {
        byte[] GenerarReporteStock(List<StockReporteDTO> data);
        byte[] GenerarReporteMovimientos(List<MovimientoStockReporteDTO> data);
    }
}
