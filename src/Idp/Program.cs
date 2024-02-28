using Idp.Components;
using Idp.Data;
using Idp.Model;
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

services.AddAuthentication()
    .AddCookie();

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

// app.UseIdentityServer();
app.UseAuthorization();
app.UseAntiforgery();

app.MapRazorComponents<App>();

app.Run();