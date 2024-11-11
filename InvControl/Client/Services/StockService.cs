using InvControl.Client.Helpers;
using InvControl.Shared.Filtros;
using InvControl.Shared.Models;
using System.Net;
using System.Net.Http.Json;

namespace InvControl.Client.Services
{
    public class StockService
    {
        readonly HttpClient _httpClient;
        readonly IJSFunctions _jsFunctions;
        const string BASE_REQUEST_URI = "api/stock";

        public StockService(HttpClient httpClient, IJSFunctions jsFunctions)
        {
            _httpClient = httpClient;
            _jsFunctions = jsFunctions;
        }

        public async ValueTask<List<Stock>> GetStock(string nombre, int? idMarca, bool? especial, int? cantidadMin, int? cantidadMax, DateTime? fechaMin, DateTime? fechaMax)
        {
            string uri = $"{BASE_REQUEST_URI}/consulta";
            Dictionary<string, object> query = new();
            if (nombre != null) query["nombre"] = nombre.Trim();
            if (idMarca != null) query["idMarca"] = idMarca;
            if (especial != null) query["especial"] = especial;
            if (cantidadMin != null) query["cantidadMin"] = cantidadMin;
            if (cantidadMax != null) query["cantidadMax"] = cantidadMax;
            if (fechaMin != null) query["fechaMin"] = fechaMin;
            if (fechaMax != null) query["fechaMax"] = fechaMax;

            if (query.Count > 0)
                uri += "?" + string.Join("&", query.Select(x => $"{x.Key}={x.Value}"));

            return (await _httpClient.GetFromJsonAsync<List<Stock>>(uri))!;
        }

        public async ValueTask PostStockExportToExcel(List<Stock> stock)
        {
            var response = await _httpClient.PostAsJsonAsync($"{BASE_REQUEST_URI}/consulta/exportToExcel", stock);
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsByteArrayAsync();
                await _jsFunctions.downloadFileFromStream($"stock {DateTime.Now:dd-MM-yyyy}.xlsx", data);
            }
            else
            {
                Console.WriteLine(await response.Content.ReadAsStringAsync());
            }
        }

        public async ValueTask<List<TipoMovimiento>> GetTiposMovimientos(int? idTipoMovimiento, string nombre, bool? soloLectura, bool? interno)
        {
            string uri = $"{BASE_REQUEST_URI}/tipoMovimiento";
            Dictionary<string, object> query = new();
            if (idTipoMovimiento != null) query["idTipoMovimiento"] = idTipoMovimiento;
            if (nombre != null) query["nombre"] = nombre;
            if (soloLectura != null) query["soloLectura"] = soloLectura;
            if (interno != null) query["interno"] = interno;

            if (query.Count > 0)
                uri += "?" + string.Join("&", query.Select(x => $"{x.Key}={x.Value}"));

            return (await _httpClient.GetFromJsonAsync<List<TipoMovimiento>>(uri))!;
        }

        public async ValueTask<Response> PostTipoMovimiento(TipoMovimiento tipoMovimiento)
        {
            var res = await _httpClient.PostAsJsonAsync($"{BASE_REQUEST_URI}/tipoMovimiento", tipoMovimiento);
            if (res.StatusCode == HttpStatusCode.OK)
            {
                var newTipoMovimiento = await res.Content.ReadFromJsonAsync<TipoMovimiento>();
                tipoMovimiento.IdTipoMovimiento = newTipoMovimiento.IdTipoMovimiento;
                return new(true);
            }
            else if (res.StatusCode == HttpStatusCode.BadRequest)
                return new(false, (await res.Content.ReadFromJsonAsync<Dictionary<string, List<string>>>())!);
            else
                return new(false);
        }

        public async ValueTask<Response> PutTipoMovimiento(TipoMovimiento tipoMovimiento)
        {
            var res = await _httpClient.PutAsJsonAsync($"{BASE_REQUEST_URI}/tipoMovimiento", tipoMovimiento);
            if (res.StatusCode == HttpStatusCode.OK)
                return new(true);
            else if (res.StatusCode == HttpStatusCode.BadRequest)
                return new(false, (await res.Content.ReadFromJsonAsync<Dictionary<string, List<string>>>())!);
            else
                return new(false);
        }

        public async ValueTask<bool> PostStockAjuste(StockAjuste stockAjuste)
        {
            var res = await _httpClient.PostAsJsonAsync($"{BASE_REQUEST_URI}/ajuste", stockAjuste);
            return res.StatusCode == HttpStatusCode.OK;
        }

        public async ValueTask<bool> PostMovimientosBimbo(List<MovimientoBimbo> movimientos)
        {
            var res = await _httpClient.PostAsJsonAsync($"{BASE_REQUEST_URI}/movimientosbimbo", movimientos);
            return res.StatusCode == HttpStatusCode.OK;
        }

        public async ValueTask PostMovimientosBimboExportToExcel(List<MovimientoBimbo> movimientos)
        {
            var response = await _httpClient.PostAsJsonAsync($"{BASE_REQUEST_URI}/movimientosbimbo/exportToExcel", movimientos);
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsByteArrayAsync();
                await _jsFunctions.downloadFileFromStream($"movimientos {DateTime.Now:dd-MM-yyyy}.xlsx", data);
            }
            else
            {
                Console.WriteLine(await response.Content.ReadAsStringAsync());
            }
        }

        public async ValueTask<List<StockMovimiento>> PostStockMovimientos(StockMovimientoFiltro stockMovimientoFiltro)
        {
            var res = await _httpClient.PostAsJsonAsync($"{BASE_REQUEST_URI}/movimientos", stockMovimientoFiltro);
            return await res.Content.ReadFromJsonAsync<List<StockMovimiento>>();
        }

        public async ValueTask PostStockMovimientosExportToExcel(List<StockMovimiento> movimientos)
        {
            var response = await _httpClient.PostAsJsonAsync($"{BASE_REQUEST_URI}/movimientos/exportToExcel", movimientos);
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsByteArrayAsync();
                await _jsFunctions.downloadFileFromStream($"movimientos {DateTime.Now:dd-MM-yyyy}.xlsx", data);
            }
            else
            {
                Console.WriteLine(await response.Content.ReadAsStringAsync());
            }
        }
    }
}
