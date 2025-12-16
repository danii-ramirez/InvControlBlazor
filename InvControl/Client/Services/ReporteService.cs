using InvControl.Shared.Filtros;
using Microsoft.JSInterop;
using System.Net;
using System.Net.Http.Json;

namespace InvControl.Client.Services
{
    public class ReporteService
    {
        private readonly HttpClient _httpClient;
        private readonly IJSRuntime js;
        const string BASE_REQUEST_URI = "api/Reportes";

        public ReporteService(HttpClient httpClient, IJSRuntime js)
        {
            _httpClient = httpClient;
            this.js = js;
        }

        public async ValueTask<bool> GetStockReportes(StockFiltro filtro)
        {
            var res = await _httpClient.PostAsJsonAsync($"{BASE_REQUEST_URI}/stock", filtro);
            if (res.StatusCode == HttpStatusCode.OK)
            {
                var bytes = await res.Content.ReadAsByteArrayAsync();
                await js.InvokeVoidAsync("downloadFile", $"stock {DateTime.Now:d}.pdf", "application/pdf", bytes);
                return true;
            }
            else
                return false;
        }

        public async ValueTask<bool> GetStockMovimientosReportes(StockMovimientoFiltro filtro)
        {
            var res = await _httpClient.PostAsJsonAsync($"{BASE_REQUEST_URI}/stock/movimientos", filtro);
            if (res.StatusCode == HttpStatusCode.OK)
            {
                var bytes = await res.Content.ReadAsByteArrayAsync();
                await js.InvokeVoidAsync("downloadFile", $"stock movimientos {DateTime.Now:d}.pdf", "application/pdf", bytes);
                return true;
            }
            else
                return false;
        }
    }
}
