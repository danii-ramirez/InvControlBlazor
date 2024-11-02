using System.Net;
using System.Net.Http.Json;
using InvControl.Client.Helpers;
using InvControl.Shared.DTO;
using InvControl.Shared.Helpers;
using InvControl.Shared.Models;

namespace InvControl.Client.Services
{
    public class RemitosService
    {
        readonly HttpClient _httpClient;
        const string BASE_REQUEST_URI = "api/remitos";

        public RemitosService(HttpClient httpClient) => _httpClient = httpClient;

        public async ValueTask<List<RemitoDTO>> GetRemitosDTO(int? idRemito, string numeroRemito, int? remitoEstado)
        {
            string uri = $"{BASE_REQUEST_URI}";
            Dictionary<string, object> query = new();
            if (idRemito != null) query["idRemito"] = idRemito;
            if (numeroRemito != null) query["numeroRemito"] = numeroRemito;
            if (remitoEstado != null) query["remitoEstado"] = remitoEstado;

            if (query.Any())
                uri += "?" + string.Join("&", query.Select(x => $"{x.Key}={x.Value}"));

            return (await _httpClient.GetFromJsonAsync<List<RemitoDTO>>(uri))!;
        }

        public async ValueTask<Remito> GetRemito(int idRemito, RemitoEstado? remitoEstado)
        {
            var uri = $"{BASE_REQUEST_URI}/{idRemito}";

            if (remitoEstado != null)
                uri += $"?remitoEstado={remitoEstado}";

            var res = await _httpClient.GetAsync(uri);

            return res.StatusCode == HttpStatusCode.OK ? await res.Content.ReadFromJsonAsync<Remito>() : null;
        }

        public async ValueTask<Response> PostRemito(Remito remito)
        {
            var res = await _httpClient.PostAsJsonAsync(BASE_REQUEST_URI, remito);
            if (res.StatusCode == HttpStatusCode.OK)
                return new(true);
            else if (res.StatusCode == HttpStatusCode.BadRequest)
                return new(false, (await res.Content.ReadFromJsonAsync<Dictionary<string, List<string>>>())!);
            else
                return new(false);
        }

        public async ValueTask<Response> PutRemito(Remito remito)
        {
            var res = await _httpClient.PutAsJsonAsync(BASE_REQUEST_URI, remito);
            if (res.StatusCode == HttpStatusCode.OK)
                return new(true);
            else if (res.StatusCode == HttpStatusCode.BadRequest)
                return new(false, (await res.Content.ReadFromJsonAsync<Dictionary<string, List<string>>>())!);
            else
                return new(false);
        }

        public async ValueTask<bool> PutRemitoEstado(RemitoState remito)
        {
            var res = await _httpClient.PutAsJsonAsync($"{BASE_REQUEST_URI}/estado", remito);
            return res.StatusCode == HttpStatusCode.OK;
        }

        public async ValueTask<bool> PutRemitosProcesar(List<RemitoDTO> remitos)
        {
            var res = await _httpClient.PutAsJsonAsync($"{BASE_REQUEST_URI}/procesar", remitos);
            return res.StatusCode == HttpStatusCode.OK;
        }
    }
}
