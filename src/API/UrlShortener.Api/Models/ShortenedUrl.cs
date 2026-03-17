namespace UrlShortener.Api.Models;

public sealed class ShortenedUrl
{
    public long Id { get; set; }
    public string LongUrl { get; set; } = string.Empty;
    public string ShortCode { get; set; } = string.Empty;
    public string? CustomAlias { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public bool IsActive { get; set; } = true;
    public int ClickCount { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // User ownership
    public string UserId { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = null!;

    // Navigation
    public ICollection<ClickEvent> ClickEvents { get; set; } = [];
}
