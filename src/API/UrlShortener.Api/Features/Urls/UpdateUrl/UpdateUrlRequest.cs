namespace UrlShortener.Api.Features.Urls.UpdateUrl;

public class UpdateUrlRequest
{
    public long Id { get; set; }
    public string LongUrl { get; set; } = string.Empty;
    public DateTime? ExpiresAt { get; set; }
}
