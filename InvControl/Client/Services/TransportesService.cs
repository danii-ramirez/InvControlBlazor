using InvControl.Client.Helpers;
using InvControl.Shared.Models;
using System.Net;
using System.Net.Http.Json;

namespace InvControl.Client.Services
{
    public class TransportesService
    {
        readonly HttpClient _httpClient;
        const string BASE_REQUEST_URI = "api/transportes";

        public TransportesService(HttpClient httpClient) => _httpClient = httpClient;

        public async ValueTask<List<Transporte>> GetTransportes()
        {
            return (await _httpClient.GetFromJsonAsync<List<Transporte>>(BASE_REQUEST_URI))!;
        }

        public async ValueTask<Response> PostTransportes(Transporte transporte)
        {
            var res = await _httpClient.PostAsJsonAsync(BASE_REQUEST_URI, transporte);
            if (res.StatusCode == HttpStatusCode.OK)
            {
                var newT = await res.Content.ReadFromJsonAsync<Transporte>();
                transporte.IdTransporte = newT!.IdTransporte;
                return new(true);
            }
            else if (res.StatusCode == HttpStatusCode.BadRequest)
                return new(false, (await res.Content.ReadFromJsonAsync<Dictionary<string, List<string>>>())!);
            else
                return new(false);
        }

        public async ValueTask<Response> PutTransportes(Transporte transporte)
        {
            var res = await _httpClient.PutAsJsonAsync(BASE_REQUEST_URI, transporte);
            if (res.StatusCode == HttpStatusCode.OK)
                return new(true);
            else if (res.StatusCode == HttpStatusCode.BadRequest)
                return new(false, (await res.Content.ReadFromJsonAsync<Dictionary<string, List<string>>>())!);
            else
                return new(false);
        }
    }
}
