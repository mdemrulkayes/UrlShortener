namespace UrlShortener.Auth;

public class TokenProvider(IHttpContextAccessor httpContextAccessor)
{
    public string? Token { get; private set; }

    public void SetToken(string token) => Token = token;

    public void ClearToken() => Token = null;

    public string? GetToken()
    {
        if (!string.IsNullOrWhiteSpace(Token))
            return Token;

        var cookieToken = httpContextAccessor.HttpContext?.Request.Cookies["auth_token"];
        if (!string.IsNullOrWhiteSpace(cookieToken))
            SetToken(cookieToken);

        return Token;
    }
}
