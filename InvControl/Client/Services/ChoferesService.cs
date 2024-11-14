using InvControl.Client.Helpers;
using InvControl.Shared.Models;
using System.Net;
using System.Net.Http.Json;

namespace InvControl.Client.Services
{
    public class ChoferesService
    {
        readonly HttpClient _httpClient;
        const string BASE_REQUEST_URI = "api/choferes";

        public ChoferesService(HttpClient httpClient) => _httpClient = httpClient;

        public async ValueTask<List<Chofer>> GetChoferes()
        {
            return (await _httpClient.GetFromJsonAsync<List<Chofer>>(BASE_REQUEST_URI))!;
        }

        public async ValueTask<Response> PostChoferes(Chofer chofer)
        {
            var res = await _httpClient.PostAsJsonAsync(BASE_REQUEST_URI, chofer);
            if (res.StatusCode == HttpStatusCode.OK)
            {
                var newC = await res.Content.ReadFromJsonAsync<Chofer>();
                chofer.IdChofer = newC!.IdChofer;
                return new(true);
            }
            else if (res.StatusCode == HttpStatusCode.BadRequest)
                return new(false, (await res.Content.ReadFromJsonAsync<Dictionary<string, List<string>>>())!);
            else
                return new(false);
        }

        public async ValueTask<Response> PutChoferes(Chofer chofer)
        {
            var res = await _httpClient.PutAsJsonAsync(BASE_REQUEST_URI, chofer);
            if (res.StatusCode == HttpStatusCode.OK)
                return new(true);
            else if (res.StatusCode == HttpStatusCode.BadRequest)
                return new(false, (await res.Content.ReadFromJsonAsync<Dictionary<string, List<string>>>())!);
            else
                return new(false);
        }

        public async ValueTask<bool> DeleteChofer(int idChofer)
        {
            var res = await _httpClient.DeleteAsync($"{BASE_REQUEST_URI}/{idChofer}");
            return res.StatusCode == HttpStatusCode.OK;
        }
    }
}
