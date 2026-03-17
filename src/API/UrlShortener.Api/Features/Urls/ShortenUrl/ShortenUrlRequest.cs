namespace UrlShortener.Api.Features.Urls.ShortenUrl;

public class ShortenUrlRequest
{
    public string LongUrl { get; set; } = string.Empty;
    public string? CustomAlias { get; set; }
    public DateTime? ExpiresAt { get; set; }
}
