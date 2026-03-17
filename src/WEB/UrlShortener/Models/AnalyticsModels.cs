namespace UrlShortener.Models;

public class DashboardResponse
{
    public int TotalUrls { get; set; }
    public int TotalClicks { get; set; }
    public int ActiveUrls { get; set; }
    public IEnumerable<TopUrlItem> TopUrls { get; set; } = [];
}

public class TopUrlItem
{
    public long Id { get; set; }
    public string ShortCode { get; set; } = string.Empty;
    public string LongUrl { get; set; } = string.Empty;
    public int ClickCount { get; set; }
}

public class UrlStatsResponse
{
    public long UrlId { get; set; }
    public string ShortCode { get; set; } = string.Empty;
    public int TotalClicks { get; set; }
    public DateTime? LastClickedAt { get; set; }
    public IEnumerable<ClickDetail> RecentClicks { get; set; } = [];
}

public class ClickDetail
{
    public DateTime ClickedAt { get; set; }
    public string? Referrer { get; set; }
    public string? UserAgent { get; set; }
    public string? IpAddress { get; set; }
}
