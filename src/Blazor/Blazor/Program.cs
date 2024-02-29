using System.Net.Http.Headers;
using Blazor;
using Blazor.Client.Weather;
using Blazor.Components;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Yarp.ReverseProxy.Transforms;
using _Imports = Blazor.Client._Imports;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddOpenIdConnect(opts =>
    {
        opts.Authority = "https://localhost:5001";
        opts.ClientId = "web";
        opts.ClientSecret = "secret";
        opts.ResponseType = OpenIdConnectResponseType.Code;
        opts.Scope.Add("email");
        opts.Scope.Add("offline_access");
        opts.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        opts.SaveTokens = true;
        opts.UsePkce = true;
    })
    .AddCookie();

builder.Services.AddAuthorization();

builder.Services.AddCascadingAuthenticationState();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddScoped<AuthenticationStateProvider, PersistingAuthenticationStateProvider>();

builder.Services.AddHttpForwarder();
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient<IWeatherForecaster, ServerWeatherForecaster>(httpClient =>
{
    httpClient.BaseAddress = new Uri("https://localhost:5005");
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(_Imports).Assembly);

app.MapForwarder("/weatherforecast", "https://localhost:5005",
    transformBuilder =>
    {
        transformBuilder.AddRequestTransform(async context =>
        {
            var accessToken = await context.HttpContext.GetTokenAsync("access_token");
            context.ProxyRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        });
    }).RequireAuthorization();

app.MapGroup("/authentication").MapLoginAndLogout();

app.Run();