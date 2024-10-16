using System.Net;
using System.Net.Http.Json;
using InvControl.Client.Helpers;
using InvControl.Shared.Models;

namespace InvControl.Client.Services
{
    public class RemitosService
    {
        readonly HttpClient _httpClient;
        const string BASE_REQUEST_URI = "api/remitos";

        public RemitosService(HttpClient httpClient) => _httpClient = httpClient;

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
