using InvControl.Shared.Models;
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

        public async ValueTask<List<Stock>> GetStock(string? nombre, int? idMarca, bool? especial, int? cantidadMin, int? cantidadMax, DateTime? fechaMin, DateTime? fechaMax)
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

        public async ValueTask PostExportToExcel(List<Stock> stock)
        {
            var response = await _httpClient.PostAsJsonAsync($"{BASE_REQUEST_URI}/ExportToExcel", stock);
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
    }
}
