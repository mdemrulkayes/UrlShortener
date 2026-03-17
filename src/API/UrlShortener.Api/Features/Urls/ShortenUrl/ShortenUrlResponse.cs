namespace UrlShortener.Api.Features.Urls.ShortenUrl;

public class ShortenUrlResponse
{
    public long Id { get; set; }
    public string ShortCode { get; set; } = string.Empty;
    public string LongUrl { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
