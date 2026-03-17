using System.Security.Claims;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using UrlShortener.Api.Data;

namespace UrlShortener.Api.Features.Analytics.GetDashboard;

public class GetDashboardEndpoint : EndpointWithoutRequest<GetDashboardResponse>
{
    private readonly ApplicationDbContext _db;

    public GetDashboardEndpoint(ApplicationDbContext db)
    {
        _db = db;
    }

    public override void Configure()
    {
        Get("/api/analytics/dashboard");
        Group<AnalyticsGroup>();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        var urls = _db.ShortenedUrls.Where(x => x.UserId == userId);

        var totalUrls = await urls.CountAsync(ct);
        var totalClicks = await urls.SumAsync(x => x.ClickCount, ct);
        var activeUrls = await urls.CountAsync(x => x.IsActive, ct);

        var topUrls = await urls
            .OrderByDescending(x => x.ClickCount)
            .Take(5)
            .Select(x => new TopUrlItem
            {
                Id = x.Id,
                ShortCode = x.ShortCode,
                LongUrl = x.LongUrl,
                ClickCount = x.ClickCount
            })
            .ToListAsync(ct);

        await SendAsync(new GetDashboardResponse
        {
            TotalUrls = totalUrls,
            TotalClicks = totalClicks,
            ActiveUrls = activeUrls,
            TopUrls = topUrls
        }, cancellation: ct);
    }
}
