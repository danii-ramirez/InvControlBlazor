using InvControl.Server.Helpers;
using InvControl.Shared.DTO;
using QuestPDF.Fluent;

namespace InvControl.Server.Services
{
    public class ReporteService : IReporteService
    {
        public byte[] GenerarReporteStock(List<StockReporteDTO> data)
        {
            var doc = new StockReporteDocument(data);
            return doc.GeneratePdf();
        }

        public byte[] GenerarReporteMovimientos(List<MovimientoStockReporteDTO> data)
        {
            var document = new MovimientosStockDocument(data);
            return document.GeneratePdf();
        }
    }
}
