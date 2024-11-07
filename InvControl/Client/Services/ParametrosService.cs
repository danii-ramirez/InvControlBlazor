using InvControl.Client.Helpers;
using InvControl.Shared.Models;
using System.Net;
using System.Net.Http.Json;

namespace InvControl.Client.Services
{
    public class ParametrosService
    {
        readonly HttpClient _httpClient;
        const string BASE_REQUEST_URI = "api/parametros";

        public ParametrosService(HttpClient httpClient) => _httpClient = httpClient;

        public async ValueTask<List<ParametroBimbo>> GetParametrosBimbo()
        {
            return await _httpClient.GetFromJsonAsync<List<ParametroBimbo>>($"{BASE_REQUEST_URI}/bimbo");
        }

        public async ValueTask<Response> PostParametroBimbo(ParametroBimbo parametro)
        {
            var res = await _httpClient.PostAsJsonAsync($"{BASE_REQUEST_URI}/bimbo", parametro);
            if (res.StatusCode == HttpStatusCode.OK)
            {
                var newParametro = await res.Content.ReadFromJsonAsync<ParametroBimbo>();
                parametro.IdParametroBimbo = newParametro.IdParametroBimbo;
                return new(true);
            }
            else if (res.StatusCode == HttpStatusCode.BadRequest)
                return new(false, (await res.Content.ReadFromJsonAsync<Dictionary<string, List<string>>>())!);
            else
                return new(false);
        }

        public async ValueTask<Response> PutParametroBimbo(ParametroBimbo parametro)
        {
            var res = await _httpClient.PutAsJsonAsync($"{BASE_REQUEST_URI}/bimbo", parametro);
            if (res.StatusCode == HttpStatusCode.OK)
                return new(true);
            else if (res.StatusCode == HttpStatusCode.BadRequest)
                return new(false, (await res.Content.ReadFromJsonAsync<Dictionary<string, List<string>>>())!);
            else
                return new(false);
        }

        public async ValueTask<bool> DeleteParametroBimbo(int idParametroBimbo)
        {
            var res = await _httpClient.DeleteAsync($"{BASE_REQUEST_URI}/bimbo/{idParametroBimbo}");
            return res.StatusCode == HttpStatusCode.OK;
        }
    }
}
