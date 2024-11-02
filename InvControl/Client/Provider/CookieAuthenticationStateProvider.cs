using InvControl.Shared.Models;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net;
using System.Net.Http.Json;
using System.Security.Claims;

namespace InvControl.Client.Provider
{
    public class CookieAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly HttpClient _httpClient;

        private const string BASE_REQUEST_URI = "api/authentication";

        public CookieAuthenticationStateProvider(HttpClient httpClient) => _httpClient = httpClient;

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var currentUser = await GetCurrentUser();
            ClaimsIdentity identity;

            if (currentUser != null)
            {
                var claims = new List<Claim> {
                    new(ClaimTypes.NameIdentifier, currentUser.IdUsuario.ToString()),
                    new(ClaimTypes.Name, currentUser.Nombre),
                    new(ClaimTypes.Surname, currentUser.Apellido)
                };

                identity = new ClaimsIdentity(claims, "Cookies");
            }
            else
            {
                identity = new ClaimsIdentity();
            }

            var user = new ClaimsPrincipal(identity);
            return new AuthenticationState(user);
        }

        public async Task<LoginUserResponse> SingIn(LoginUser user)
        {
            var response = await _httpClient.PostAsJsonAsync($"{BASE_REQUEST_URI}/login", user);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                NotifyAuthenticationStateChanged();
            }

            return (await response.Content.ReadFromJsonAsync<LoginUserResponse>())!;
        }

        public async Task LogOut()
        {
            await _httpClient.PostAsync($"{BASE_REQUEST_URI}/logout", null);
            NotifyAuthenticationStateChanged();
        }

        public async Task<CurrentUser> GetCurrentUser()
        {
            var response = await _httpClient.GetAsync($"{BASE_REQUEST_URI}/currentUser");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<CurrentUser>();
            }

            return null;
        }

        public void NotifyAuthenticationStateChanged()
        {
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }
    }
}
