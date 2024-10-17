using System.Net;
using System.Net.Http.Json;
using InvControl.Client.Helpers;
using InvControl.Shared.DTO;
using InvControl.Shared.Models;

namespace InvControl.Client.Services
{
    public class RemitosService
    {
        readonly HttpClient _httpClient;
        const string BASE_REQUEST_URI = "api/remitos";

        public RemitosService(HttpClient httpClient) => _httpClient = httpClient;

        public async ValueTask<List<RemitoDTO>> GetRemitosDTO(int? idRemito, string? numeroRemito, int? idEstado)
        {
            string uri = $"{BASE_REQUEST_URI}";
            Dictionary<string, object> query = new();
            if (idRemito != null) query["idRemito"] = idRemito;
            if (numeroRemito != null) query["numeroRemito"] = numeroRemito;
            if (idEstado != null) query["idEstado"] = idEstado;

            if (query.Any())
                uri += "?" + string.Join("&", query.Select(x => $"{x.Key}={x.Value}"));

            return (await _httpClient.GetFromJsonAsync<List<RemitoDTO>>(uri))!;
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
    }
}
