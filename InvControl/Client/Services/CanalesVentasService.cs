using InvControl.Shared.Models;
using System.Net.Http.Json;

namespace InvControl.Client.Services
{
    public class CanalesVentasService
    {
        readonly HttpClient _httpClient;
        const string BASE_REQUEST_URI = "api/CanalesVentas";

        public CanalesVentasService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async ValueTask<List<CanalVenta>> GetCanalesVentas()
        {
            return (await _httpClient.GetFromJsonAsync<List<CanalVenta>>(BASE_REQUEST_URI))!;
        }
    }
}
