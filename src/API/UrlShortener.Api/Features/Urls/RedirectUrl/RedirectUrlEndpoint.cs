using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using UrlShortener.Api.Data;
using UrlShortener.Api.Models;

namespace UrlShortener.Api.Features.Urls.RedirectUrl;

public class RedirectUrlEndpoint : EndpointWithoutRequest
{
    private readonly ApplicationDbContext _db;

    public RedirectUrlEndpoint(ApplicationDbContext db)
    {
        _db = db;
    }

    public override void Configure()
    {
        Get("/{shortCode}");
        Group<UrlsGroup>();
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var shortCode = Route<string>("shortCode")!;
        var url = await _db.ShortenedUrls
            .FirstOrDefaultAsync(x => x.ShortCode == shortCode, ct);

        if (url is null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        if (!url.IsActive)
        {
            await SendAsync(null, 410, ct);
            return;
        }

        if (url.ExpiresAt.HasValue && url.ExpiresAt.Value < DateTime.UtcNow)
        {
            await SendAsync(null, 410, ct);
            return;
        }

        // Track click
        url.ClickCount++;
        var clickEvent = new ClickEvent
        {
            ShortenedUrlId = url.Id,
            Referrer = HttpContext.Request.Headers.Referer.ToString(),
            UserAgent = HttpContext.Request.Headers.UserAgent.ToString(),
            IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString()
        };
        _db.ClickEvents.Add(clickEvent);
        await _db.SaveChangesAsync(ct);

        await SendRedirectAsync(url.LongUrl, isPermanent: false);
    }
}
