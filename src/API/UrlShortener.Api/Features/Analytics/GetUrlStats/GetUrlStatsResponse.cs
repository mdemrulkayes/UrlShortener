namespace UrlShortener.Api.Features.Analytics.GetUrlStats;

public class GetUrlStatsResponse
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
