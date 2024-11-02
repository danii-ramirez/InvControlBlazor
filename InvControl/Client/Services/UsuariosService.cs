using InvControl.Client.Helpers;
using InvControl.Shared.DTO;
using InvControl.Shared.Models;
using Microsoft.AspNetCore.Components;
using System.Net;
using System.Net.Http.Json;

namespace InvControl.Client.Services
{
    public class UsuariosService
    {
        readonly HttpClient _httpClient;
        readonly ILogger<UsuariosService> _logger;
        readonly NavigationManager _navigationManager;
        const string BASE_REQUEST_URI = "api/usuarios";

        public UsuariosService(HttpClient httpClient, ILogger<UsuariosService> logger, NavigationManager navigationManager)
        {
            _httpClient = httpClient;
            _logger = logger;
            _navigationManager = navigationManager;
        }

        public async ValueTask<List<Usuario>> GetUsuarios(int? idUsuario = null)
        {
            string uri = $"{BASE_REQUEST_URI}";

            Dictionary<string, object> query = new();
            if (idUsuario != null) query["idUsuario"] = idUsuario;

            if (query.Any())
                uri = "?" + string.Join("&", query.Select(x => $"{x.Key}={x.Value}"));

            return (await _httpClient.GetFromJsonAsync<List<Usuario>>(uri))!;
        }

        public async ValueTask<Response> PostUsuario(Usuario usuario)
        {
            var res = await _httpClient.PostAsJsonAsync(BASE_REQUEST_URI, usuario);
            if (res.StatusCode == HttpStatusCode.OK)
            {
                var newUsu = await res.Content.ReadFromJsonAsync<Usuario>();
                usuario.IdUsuario = newUsu!.IdUsuario;
                return new(true);
            }
            else if (res.StatusCode == HttpStatusCode.BadRequest)
                return new(false, (await res.Content.ReadFromJsonAsync<Dictionary<string, List<string>>>())!);
            else
            {
                _logger.LogError("{msg}", await res.Content.ReadAsStringAsync());
                return new(false);
            }
        }

        public async ValueTask<Response> PutUsuario(Usuario usuario)
        {
            var res = await _httpClient.PutAsJsonAsync(BASE_REQUEST_URI, usuario);
            if (res.StatusCode == HttpStatusCode.OK)
                return new(true);
            else if (res.StatusCode == HttpStatusCode.BadRequest)
                return new(false, (await res.Content.ReadFromJsonAsync<Dictionary<string, List<string>>>())!);
            else
            {
                _logger.LogError("{msg}", await res.Content.ReadAsStringAsync());
                return new(false);
            }
        }

        public async ValueTask<bool> DeleteUsuario(int idUsuario)
        {
            var response = await _httpClient.DeleteAsync($"{BASE_REQUEST_URI}/{idUsuario}");
            return response.StatusCode == HttpStatusCode.OK;
        }

        public async ValueTask<Response> PostResetPassword(LoginUserResetPassword user)
        {
            var res = await _httpClient.PostAsJsonAsync($"{BASE_REQUEST_URI}/resetpassword", user);
            if (res.StatusCode == HttpStatusCode.OK)
                return new(true);
            else if (res.StatusCode == HttpStatusCode.BadRequest)
                return new(false, (await res.Content.ReadFromJsonAsync<Dictionary<string, List<string>>>())!);
            else
            {
                _logger.LogError("{msg}", await res.Content.ReadAsStringAsync());
                return new(false);
            }
        }

        public async ValueTask<bool> GetResetPassword(int idUsuario)
        {
            var response = await _httpClient.GetAsync($"{BASE_REQUEST_URI}/resetpassword/{idUsuario}");
            return response.StatusCode == HttpStatusCode.OK;
        }

        public async ValueTask<List<Permiso>> GetMenu()
        {
            return (await _httpClient.GetFromJsonAsync<List<Permiso>>($"{BASE_REQUEST_URI}/menu"))!;
        }

        public async ValueTask<(bool, List<string>)> GetValidarAcceso()
        {
            var url = _navigationManager.ToBaseRelativePath(_navigationManager.Uri);
            var response = await _httpClient.GetAsync($"{BASE_REQUEST_URI}/validar/acceso?url={url}");
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var acciones = await response.Content.ReadFromJsonAsync<List<string>>();
                return (true, acciones);
            }
            else
            {
                _navigationManager.NavigateTo("/authentication/accessdenied");
                return (false, null);
            }
        }

        public async ValueTask<(bool, List<string>)> GetValidarAcceso(string url)
        {
            var response = await _httpClient.GetAsync($"{BASE_REQUEST_URI}/validar/acceso?url={url}");
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var acciones = await response.Content.ReadFromJsonAsync<List<string>>();
                return (true, acciones);
            }
            else
            {
                _navigationManager.NavigateTo("/authentication/accessdenied");
                return (false, null);
            }
        }

        public async ValueTask<List<AuditoriaDTO>> GetAuditoria(int? idUsuario, int? idTipoEntidad)
        {
            string uri = $"{BASE_REQUEST_URI}/auditoria";

            Dictionary<string, object> query = new();
            if (idUsuario != null) query["idUsuario"] = idUsuario;
            if (idTipoEntidad != null) query["idTipoEntidad"] = idTipoEntidad;

            if (query.Any())
                uri = "?" + string.Join("&", query.Select(x => $"{x.Key}={x.Value}"));

            return (await _httpClient.GetFromJsonAsync<List<AuditoriaDTO>>(uri))!;
        }
    }
}
