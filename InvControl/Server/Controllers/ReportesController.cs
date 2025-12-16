using InvControl.Server.Data;
using InvControl.Server.Services;
using InvControl.Shared.DTO;
using InvControl.Shared.Filtros;
using InvControl.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace InvControl.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportesController : ControllerBase
    {
        private readonly string connectionString;
        private readonly IReporteService _reporteService;

        public ReportesController(IReporteService reporteService, IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("InvControlDB");
            _reporteService = reporteService;
        }

        [HttpPost("stock")]
        public IActionResult ReporteStock([FromBody] StockFiltro filtros)
        {
            List<StockReporteDTO> data = new();

            using (DataTable dt = new DA_Stock(connectionString).ObtenerStock(filtros.Nombre?.Trim(), filtros.IdMarca == 0 ? null : filtros.IdMarca, filtros.Estado, filtros.StockMinimo, filtros.StockMaximo, null, null))
            {
                foreach (DataRow dr in dt.Rows)
                {
                    StockReporteDTO s = new()
                    {
                        Codigo = (int)dr["Codigo"],
                        Nombre = (string)dr["Nombre"],
                        Cantidad = (int)dr["Cantidad"]
                    };

                    data.Add(s);
                }
            }

            if (data.Count > 0)
            {
                var pdf = _reporteService.GenerarReporteStock(data);
                return File(pdf, "application/pdf", "stock.pdf");
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost("stock/movimientos")]
        public IActionResult ReporteStockMovimiento([FromBody] StockMovimientoFiltro filtros)
        {
            List<MovimientoStockReporteDTO> data = new();

            using (DataTable dt = new DA_Stock(connectionString).ObtenerStockMovimientos(filtros.IdTipoMovimiento, filtros.Codigo, filtros.Nombre, filtros.FechaDesde, filtros.FechaHasta, filtros.IdCanalVenta))
            {
                foreach (DataRow dr in dt.Rows)
                {
                    MovimientoStockReporteDTO sm = new()
                    {
                        Movimiento = (string)dr["NombreTipoMovimiento"],
                        Codigo = (int)dr["CodigoSKU"],
                        Nombre = (string)dr["NombreSKU"],
                        Cantidad = (int)dr["Cantidad"],
                        Fecha = (DateTime)(dr["FechaMovimiento"] = dr["FechaMovimiento"]),
                    };
                    if (dr["Referencia"] != DBNull.Value) sm.Referencia = (string)dr["Referencia"];

                    data.Add(sm);
                }
            }

            if (data.Count > 0)
            {
                var pdf = _reporteService.GenerarReporteMovimientos(data);
                return File(pdf, "application/pdf", "stockMovimiento.pdf");
            }
            else
            {
                return NotFound();
            }
        }
    }
}
