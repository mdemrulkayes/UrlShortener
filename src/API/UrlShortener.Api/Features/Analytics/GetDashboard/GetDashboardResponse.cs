namespace UrlShortener.Api.Features.Analytics.GetDashboard;

public class GetDashboardResponse
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
