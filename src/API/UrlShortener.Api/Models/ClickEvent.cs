namespace UrlShortener.Api.Models;

public sealed class ClickEvent
{
    public long Id { get; set; }
    public long ShortenedUrlId { get; set; }
    public DateTime ClickedAt { get; set; } = DateTime.UtcNow;
    public string? Referrer { get; set; }
    public string? UserAgent { get; set; }
    public string? IpAddress { get; set; }

    public ShortenedUrl ShortenedUrl { get; set; } = null!;
}
