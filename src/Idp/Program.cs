using Idp;
using Idp.Components;
using Idp.Data;
using Idp.Model;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

services.AddDbContext<ApplicationDbContext>(opts =>
    opts.UseInMemoryDatabase(":inMemory:"));

services.AddIdentityCore<ApplicationUser>(opts => opts.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

services.AddIdentityServer(opts =>
        opts.Authentication.CookieAuthenticationScheme = CookieAuthenticationDefaults.AuthenticationScheme)
    .AddInMemoryIdentityResources(Config.IdentityResources)
    .AddInMemoryApiScopes(Config.ApiScopes)
    .AddInMemoryApiResources(Config.ApiResources)
    .AddInMemoryClients(Config.Clients)
    .AddAspNetIdentity<ApplicationUser>();

services.AddAuthentication()
    .AddCookie(opts =>
    {
        opts.Cookie.Name = "Idp";
    });

services.AddAuthorization();

services.AddRazorComponents();

services.AddCascadingAuthenticationState();
services.AddScoped<AuthenticationStateProvider, ServerAuthenticationStateProvider>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
    app.UseDeveloperExceptionPage();
else
    app.UseHsts();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseIdentityServer();
app.UseAuthorization();
app.UseAntiforgery();

app.MapRazorComponents<App>();

app.Run();