using InvControl.Client.Helpers;
using InvControl.Shared.Models;
using System.Net;
using System.Net.Http.Json;

namespace InvControl.Client.Services
{
    public class RolesService
    {
        readonly HttpClient _httpClient;
        readonly ILogger<RolesService> _logger;
        const string BASE_REQUEST_URI = "api/roles";

        public RolesService(HttpClient httpClient, ILogger<RolesService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async ValueTask<List<Rol>> GetRoles(int? idRol = null, string descripcion = null)
        {
            string uri = $"{BASE_REQUEST_URI}";

            Dictionary<string, object> query = new();
            if (idRol != null) query["idRol"] = idRol;
            if (descripcion != null) query["descripcion"] = descripcion;

            if (query.Any())
                uri += "?" + string.Join("&", query.Select(x => $"{x.Key}={x.Value}"));

            return (await _httpClient.GetFromJsonAsync<List<Rol>>(uri))!;
        }

        public async ValueTask<List<Permiso>> GetPermisos(bool jerarquico = false)
        {
            string uri = $"{BASE_REQUEST_URI}/permisos?jerarquico={jerarquico}";
            return (await _httpClient.GetFromJsonAsync<List<Permiso>>(uri))!;
        }

        public async ValueTask<Response> PostRol(Rol rol)
        {
            var res = await _httpClient.PostAsJsonAsync(BASE_REQUEST_URI, rol);
            if (res.StatusCode == HttpStatusCode.OK)
            {
                var newRol = await res.Content.ReadFromJsonAsync<Rol>();
                rol.IdRol = newRol!.IdRol;
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

        public async ValueTask<Response> PutRol(Rol rol)
        {
            var res = await _httpClient.PutAsJsonAsync(BASE_REQUEST_URI, rol);
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

        public async ValueTask<bool> DeleteRol(int idRol)
        {
            var res = await _httpClient.DeleteAsync($"{BASE_REQUEST_URI}/{idRol}");
            return res.StatusCode == HttpStatusCode.OK;
        }
    }
}
