namespace UrlShortener.Api.Features.Urls.ListUrls;

public class ListUrlsResponse
{
    public long Id { get; set; }
    public string LongUrl { get; set; } = string.Empty;
    public string ShortCode { get; set; } = string.Empty;
    public string? CustomAlias { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public bool IsActive { get; set; }
    public int ClickCount { get; set; }
    public DateTime CreatedAt { get; set; }
}
