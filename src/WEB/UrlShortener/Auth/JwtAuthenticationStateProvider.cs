using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

namespace UrlShortener.Auth;

public class JwtAuthenticationStateProvider(TokenProvider tokenProvider) : AuthenticationStateProvider
{
    private readonly ClaimsPrincipal _anonymous = new(new ClaimsIdentity());

    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            var token = tokenProvider.GetToken();
            if (string.IsNullOrWhiteSpace(token))
                return Task.FromResult(new AuthenticationState(_anonymous));

            var handler = new JwtSecurityTokenHandler();

            if (!handler.CanReadToken(token))
                return Task.FromResult(new AuthenticationState(_anonymous));

            var jwt = handler.ReadJwtToken(token);

            if (jwt.ValidTo < DateTime.UtcNow)
            {
                tokenProvider.ClearToken();
                return Task.FromResult(new AuthenticationState(_anonymous));
            }

            var identity = new ClaimsIdentity(jwt.Claims, "jwt");
            var user = new ClaimsPrincipal(identity);
            return Task.FromResult(new AuthenticationState(user));
        }
        catch
        {
            return Task.FromResult(new AuthenticationState(_anonymous));
        }
    }

    public void NotifyUserAuthentication(string token)
    {
        tokenProvider.SetToken(token);
        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token);
        var identity = new ClaimsIdentity(jwt.Claims, "jwt");
        var user = new ClaimsPrincipal(identity);
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
    }

    public void NotifyUserLogout()
    {
        tokenProvider.ClearToken();
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_anonymous)));
    }
}
