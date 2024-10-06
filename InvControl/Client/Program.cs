using InvControl.Client;
using InvControl.Client.Provider;
using InvControl.Client.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Radzen;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddScoped<AuthenticationStateProvider, CookieAuthenticationStateProvider>();
builder.Services.AddAuthorizationCore();

builder.Services.AddRadzenComponents();

Services(builder.Services);

await builder.Build().RunAsync();

static void Services(IServiceCollection services)
{
    services.AddScoped<UsuariosService>();
    services.AddScoped<RolesService>();
    services.AddScoped<CanalesVentasService>();
    services.AddScoped<ChoferesService>();
    services.AddScoped<TransportesService>();
}
