using InvControl.Client.Helpers;
using InvControl.Shared.Models;
using System.Net;
using System.Net.Http.Json;

namespace InvControl.Client.Services
{
    public class SKUService
    {
        readonly HttpClient _httpClient;
        const string BASE_REQUEST_URI = "api/sku";

        public SKUService(HttpClient httpClient) => _httpClient = httpClient;

        public async ValueTask<List<SKU>> GetSKU(int? codigo, string? nombre, bool? activo, int? idMarca)
        {
            string uri = $"{BASE_REQUEST_URI}";

            Dictionary<string, object> query = new();
            if (codigo != null) query["codigo"] = codigo;
            if (nombre != null) query["nombre"] = WebUtility.UrlEncode(nombre);
            if (activo != null) query["activo"] = activo;
            if (idMarca != null) query["idMarca"] = idMarca;

            if (query.Any())
                uri += "?" + string.Join("&", query.Select(x => $"{x.Key}={x.Value}"));

            return (await _httpClient.GetFromJsonAsync<List<SKU>>(uri))!;
        }

        public async ValueTask<Response> PostSKU(SKU sku)
        {
            var res = await _httpClient.PostAsJsonAsync(BASE_REQUEST_URI, sku);
            if (res.StatusCode == HttpStatusCode.OK)
            {
                var newSku = await res.Content.ReadFromJsonAsync<SKU>();
                sku.IdSKU = newSku!.IdSKU;
                return new(true);
            }
            else if (res.StatusCode == HttpStatusCode.BadRequest)
                return new(false, (await res.Content.ReadFromJsonAsync<Dictionary<string, List<string>>>())!);
            else
                return new(false);
        }

        public async ValueTask<Response> PutSKU(SKU sku)
        {
            var res = await _httpClient.PutAsJsonAsync(BASE_REQUEST_URI, sku);
            if (res.StatusCode == HttpStatusCode.OK)
                return new(true);
            else if (res.StatusCode == HttpStatusCode.BadRequest)
                return new(false, (await res.Content.ReadFromJsonAsync<Dictionary<string, List<string>>>())!);
            else
                return new(false);
        }

        public async ValueTask<List<Marca>> GetMarcas()
        {
            return (await _httpClient.GetFromJsonAsync<List<Marca>>($"{BASE_REQUEST_URI}/marcas"))!;
        }
    }
}
