using InvControl.Client.Helpers;
using InvControl.Shared.DTO;
using InvControl.Shared.Models;
using System.Net;
using System.Net.Http.Json;

namespace InvControl.Client.Services
{
    public class SKUService
    {
        readonly HttpClient _httpClient;
        readonly IJSFunctions _jsFunctions;
        const string BASE_REQUEST_URI = "api/sku";

        public SKUService(HttpClient httpClient, IJSFunctions jsFunctions)
        {
            _httpClient = httpClient;
            _jsFunctions = jsFunctions;
        }

        public async ValueTask<List<SKU>> GetSKU(int? codigo, string nombre, bool? activo, int? idMarca)
        {
            string uri = $"{BASE_REQUEST_URI}";

            Dictionary<string, object> query = new();
            if (codigo != null) query["codigo"] = codigo;
            if (nombre != null) query["nombre"] = WebUtility.UrlEncode(nombre);
            if (activo != null) query["activo"] = activo;
            if (idMarca != null) query["idMarca"] = idMarca;

            if (query.Count > 0)
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

        public async ValueTask<Response> PostMarca(Marca marca)
        {
            var res = await _httpClient.PostAsJsonAsync($"{BASE_REQUEST_URI}/marcas", marca);
            if (res.StatusCode == HttpStatusCode.OK)
            {
                var newMarca = await res.Content.ReadFromJsonAsync<Marca>();
                marca.IdMarca = newMarca.IdMarca;
                return new(true);
            }
            else if (res.StatusCode == HttpStatusCode.BadRequest)
                return new(false, (await res.Content.ReadFromJsonAsync<Dictionary<string, List<string>>>())!);
            else
                return new(false);
        }

        public async ValueTask<Response> PutMarca(Marca marca)
        {
            var res = await _httpClient.PutAsJsonAsync($"{BASE_REQUEST_URI}/marcas", marca);
            if (res.StatusCode == HttpStatusCode.OK)
                return new(true);
            else if (res.StatusCode == HttpStatusCode.BadRequest)
                return new(false, (await res.Content.ReadFromJsonAsync<Dictionary<string, List<string>>>())!);
            else
                return new(false);
        }

        public async ValueTask<Response> DeleteMarca(int idMarca)
        {
            var res = await _httpClient.DeleteAsync($"{BASE_REQUEST_URI}/marcas/{idMarca}");
            if (res.StatusCode == HttpStatusCode.OK)
                return new(true);
            else if (res.StatusCode == HttpStatusCode.BadRequest)
                return new(false, await res.Content.ReadAsStringAsync());
            else
                return new(false);
        }

        public async ValueTask<List<TipoContenedor>> GetTiposContenedores()
        {
            return (await _httpClient.GetFromJsonAsync<List<TipoContenedor>>($"{BASE_REQUEST_URI}/tipocontenedor"))!;
        }

        public async ValueTask<Response> PostTipoContenedor(TipoContenedor tipoContenedor)
        {
            var res = await _httpClient.PostAsJsonAsync($"{BASE_REQUEST_URI}/tipocontenedor", tipoContenedor);
            if (res.StatusCode == HttpStatusCode.OK)
            {
                var newTipoContenedor = await res.Content.ReadFromJsonAsync<TipoContenedor>();
                tipoContenedor.IdTipoContenedor = newTipoContenedor.IdTipoContenedor;
                return new(true);
            }
            else if (res.StatusCode == HttpStatusCode.BadRequest)
                return new(false, (await res.Content.ReadFromJsonAsync<Dictionary<string, List<string>>>())!);
            else
                return new(false);
        }

        public async ValueTask<Response> PutTipoContenedor(TipoContenedor tipoContenedor)
        {
            var res = await _httpClient.PutAsJsonAsync($"{BASE_REQUEST_URI}/tipocontenedor", tipoContenedor);
            if (res.StatusCode == HttpStatusCode.OK)
                return new(true);
            else if (res.StatusCode == HttpStatusCode.BadRequest)
                return new(false, (await res.Content.ReadFromJsonAsync<Dictionary<string, List<string>>>())!);
            else
                return new(false);
        }

        public async ValueTask<Response> DeleteTipoContenedor(int idTipoContenedor)
        {
            var res = await _httpClient.DeleteAsync($"{BASE_REQUEST_URI}/tipocontenedor/{idTipoContenedor}");
            if (res.StatusCode == HttpStatusCode.OK)
                return new(true);
            else if (res.StatusCode == HttpStatusCode.BadRequest)
                return new(false, await res.Content.ReadAsStringAsync());
            else
                return new(false);
        }

        public async ValueTask<List<SKUDTO>> GetSugerencias(string sugerencia)
        {
            return (await _httpClient.GetFromJsonAsync<List<SKUDTO>>($"{BASE_REQUEST_URI}/sugerencias?sugerencia={sugerencia}"))!;
        }

        public async ValueTask PostExportToExcel(List<SKU> skus)
        {
            var response = await _httpClient.PostAsJsonAsync($"{BASE_REQUEST_URI}/ExportToExcel", skus);
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsByteArrayAsync();
                await _jsFunctions.downloadFileFromStream($"skus {DateTime.Now:dd-MM-yyyy}.xlsx", data);
            }
            else
            {
                Console.WriteLine(await response.Content.ReadAsStringAsync());
            }
        }
    }
}
