using InvControl.Shared.Dashboard;
using System.Net.Http.Json;

namespace InvControl.Client.Services
{
    public class DashboardService
    {
        readonly HttpClient _httpClient;
        const string BASE_REQUEST_URI = "api/dashboard";

        public DashboardService(HttpClient httpClient) => _httpClient = httpClient;

        public async ValueTask<List<RemitoEstadoDash>> GetRemitosEstados()
        {
            return (await _httpClient.GetFromJsonAsync<List<RemitoEstadoDash>>($"{BASE_REQUEST_URI}/remitos/estados"));
        }
    }
}
