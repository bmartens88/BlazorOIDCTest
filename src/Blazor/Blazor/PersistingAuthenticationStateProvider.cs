using Blazor.Client;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;

namespace Blazor;

internal sealed class PersistingAuthenticationStateProvider : AuthenticationStateProvider,
    IHostEnvironmentAuthenticationStateProvider, IDisposable
{
    private readonly PersistentComponentState _persistentComponentState;
    private readonly PersistingComponentStateSubscription _subscription;
    private Task<AuthenticationState>? _authenticationStateTask;

    public PersistingAuthenticationStateProvider(PersistentComponentState state)
    {
        _persistentComponentState = state;
        _subscription = state.RegisterOnPersisting(OnPersistingAsync, RenderMode.InteractiveWebAssembly);
    }

    public void Dispose()
    {
        _subscription.Dispose();
    }

    public void SetAuthenticationState(Task<AuthenticationState> task)
    {
        _authenticationStateTask = task;
    }

    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        return _authenticationStateTask ??
               throw new InvalidOperationException(
                   $"Do not call {nameof(GetAuthenticationStateAsync)} outside of the DI scope for a Razor component. Typically, this means you can call it only within a Razor component or inside another DI service that is resolved for a Razor component.");
    }

    private async Task OnPersistingAsync()
    {
        var authenticationState = await GetAuthenticationStateAsync();
        var principal = authenticationState.User;

        if (principal.Identity?.IsAuthenticated == true)
            _persistentComponentState.PersistAsJson(nameof(UserInfo), UserInfo.FromClaimsPrincipal(principal));
    }
}