using InvControl.Shared.Models;
using System.Net.Http.Json;

namespace InvControl.Client.Services
{
    public class UsuariosService
    {
        readonly HttpClient _httpClient;
        const string BASE_REQUEST_URI = "api/usuarios";

        public UsuariosService(HttpClient httpClient) => _httpClient = httpClient;

        public async ValueTask PostResetPassword(LoginUserResetPassword user)
        {
            await _httpClient.PostAsJsonAsync($"{BASE_REQUEST_URI}/resetpassword", user);
        }
    }
}
