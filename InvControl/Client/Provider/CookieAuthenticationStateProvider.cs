using InvControl.Shared.Models;
using Microsoft.AspNetCore.Components.Authorization;
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
            var username = await GetCurrentUser();
            ClaimsIdentity identity;

            if (!string.IsNullOrEmpty(username))
            {
                identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, username) }, "Cookies");
            }
            else
            {
                identity = new ClaimsIdentity();
            }

            var user = new ClaimsPrincipal(identity);
            return new AuthenticationState(user);
        }

        public async Task<bool> SingIn(LoginUser user)
        {
            var response = await _httpClient.PostAsJsonAsync($"{BASE_REQUEST_URI}/login", user);
            if (response.IsSuccessStatusCode)
            {
                NotifyAuthenticationStateChanged();
                return true;
            }
            return false;
        }

        public async Task LogOut()
        {
            await _httpClient.PostAsync($"{BASE_REQUEST_URI}/logout", null);
            NotifyAuthenticationStateChanged();
        }

        public async Task<string?> GetCurrentUser()
        {
            var response = await _httpClient.GetAsync($"{BASE_REQUEST_URI}/currentUser");
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<CurrentUserModel>();
                return result?.Username;
            }

            return null;
        }

        public void NotifyAuthenticationStateChanged()
        {
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }
    }

    public class CurrentUserModel
    {
        public string Username { get; set; } = string.Empty;
    }
}
