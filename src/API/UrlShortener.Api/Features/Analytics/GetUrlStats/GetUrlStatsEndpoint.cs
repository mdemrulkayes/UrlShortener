using System.Security.Claims;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using UrlShortener.Api.Data;

namespace UrlShortener.Api.Features.Analytics.GetUrlStats;

public class GetUrlStatsEndpoint(ApplicationDbContext db) : EndpointWithoutRequest<GetUrlStatsResponse>
{
    public override void Configure()
    {
        Get("/api/analytics/urls/{id}");
        Group<AnalyticsGroup>();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = Route<long>("id");
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        var url = await db.ShortenedUrls
            .Include(x => x.ClickEvents.OrderByDescending(c => c.ClickedAt).Take(20))
            .FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId, ct);

        if (url is null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        await SendAsync(new GetUrlStatsResponse
        {
            UrlId = url.Id,
            ShortCode = url.ShortCode,
            TotalClicks = url.ClickCount,
            LastClickedAt = url.ClickEvents.MaxBy(c => c.ClickedAt)?.ClickedAt,
            RecentClicks = url.ClickEvents.Select(c => new ClickDetail
            {
                ClickedAt = c.ClickedAt,
                Referrer = c.Referrer,
                UserAgent = c.UserAgent,
                IpAddress = c.IpAddress
            })
        }, cancellation: ct);
    }
}
