namespace UrlShortener.Models;

public class ShortenedUrl
{
    public long Id { get; set; }
    public string? LongUrl { get; set; }
    public string? ShortCode { get; set; }
    public string? CustomAlias { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public bool IsActive { get; set; }
    public int ClickCount { get; set; }
    public DateTime CreatedAt { get; set; }
}
