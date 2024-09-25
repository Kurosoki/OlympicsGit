using Microsoft.AspNetCore.Components.Authorization;
using Blazored.LocalStorage;
using System.Security.Claims;
using System.Threading.Tasks;

public class CustomAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly ILocalStorageService _localStorage;
    private ClaimsPrincipal anonymous = new ClaimsPrincipal(new ClaimsIdentity());

    public CustomAuthenticationStateProvider(ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var savedUsername = await _localStorage.GetItemAsync<string>("username");
        if (!string.IsNullOrWhiteSpace(savedUsername))
        {
            var identity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, savedUsername),
                new Claim(ClaimTypes.Email, "testuser@example.com"),
            }, "Fake authentication type");

            var user = new ClaimsPrincipal(identity);
            return new AuthenticationState(user);
        }

        return new AuthenticationState(anonymous);
    }

    public async Task MarkUserAsAuthenticated(ClaimsPrincipal user)
    {
        var username = user.Identity?.Name;
        if (username != null)
        {
            await _localStorage.SetItemAsync("username", username);
        }

        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public async Task MarkUserAsLoggedOut()
    {
        await _localStorage.RemoveItemAsync("username");

        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }
}
