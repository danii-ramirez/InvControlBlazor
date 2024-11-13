using InvControl.Client.Helpers;
using InvControl.Shared.Models;
using System.Net;
using System.Net.Http.Json;

namespace InvControl.Client.Services
{
    public class CanalesVentasService
    {
        readonly HttpClient _httpClient;
        const string BASE_REQUEST_URI = "api/CanalesVentas";

        public CanalesVentasService(HttpClient httpClient) => _httpClient = httpClient;

        public async ValueTask<List<CanalVenta>> GetCanalesVentas()
        {
            return (await _httpClient.GetFromJsonAsync<List<CanalVenta>>(BASE_REQUEST_URI))!;
        }

        public async ValueTask<Response> PostCanalVenta(CanalVenta canalVenta)
        {
            var res = await _httpClient.PostAsJsonAsync(BASE_REQUEST_URI, canalVenta);
            if (res.StatusCode == HttpStatusCode.OK)
            {
                var newCV = await res.Content.ReadFromJsonAsync<CanalVenta>();
                canalVenta.IdCanalVenta = newCV!.IdCanalVenta;
                return new(true);
            }
            else if (res.StatusCode == HttpStatusCode.BadRequest)
                return new(false, (await res.Content.ReadFromJsonAsync<Dictionary<string, List<string>>>())!);
            else
                return new(false);
        }

        public async ValueTask<Response> PutCanalVenta(CanalVenta canalVenta)
        {
            var res = await _httpClient.PutAsJsonAsync(BASE_REQUEST_URI, canalVenta);
            if (res.StatusCode == HttpStatusCode.OK)
                return new(true);
            else if (res.StatusCode == HttpStatusCode.BadRequest)
                return new(false, (await res.Content.ReadFromJsonAsync<Dictionary<string, List<string>>>())!);
            else
                return new(false);
        }

        public async ValueTask<bool> DeleteCanalVenta(int idCanalVenta)
        {
            var res = await _httpClient.DeleteAsync($"{BASE_REQUEST_URI}/{idCanalVenta}");
            return res.StatusCode == HttpStatusCode.OK;
        }
    }
}
