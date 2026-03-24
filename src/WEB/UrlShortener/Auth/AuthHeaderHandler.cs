using System.Net.Http.Headers;
using Blazored.LocalStorage;

namespace UrlShortener.Auth;

public class AuthHeaderHandler(ILocalStorageService localStorage) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = await localStorage.GetItemAsStringAsync("authToken", cancellationToken);
        if (!string.IsNullOrWhiteSpace(token))
        {
            token = token.Trim('"');
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
