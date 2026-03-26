using Microsoft.AspNetCore.Components.Server.Circuits;

namespace UrlShortener.Auth;

public class TokenPopulatingCircuitHandler(
    IHttpContextAccessor httpContextAccessor,
    TokenProvider tokenProvider) : CircuitHandler
{
    public override Task OnCircuitOpenedAsync(Circuit circuit, CancellationToken cancellationToken)
    {
        var token = httpContextAccessor.HttpContext?.Request.Cookies["auth_token"];
        if (!string.IsNullOrEmpty(token))
            tokenProvider.SetToken(token);

        return base.OnCircuitOpenedAsync(circuit, cancellationToken);
    }
}
