using System.Security.Claims;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using UrlShortener.Api.Data;

namespace UrlShortener.Api.Features.Urls.ListUrls;

public class ListUrlsEndpoint(ApplicationDbContext db) : EndpointWithoutRequest<IEnumerable<ListUrlsResponse>>
{
    public override void Configure()
    {
        Get("/api/urls");
        Group<UrlsGroup>();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        var urls = await db.ShortenedUrls
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new ListUrlsResponse
            {
                Id = x.Id,
                LongUrl = x.LongUrl,
                ShortCode = x.ShortCode,
                CustomAlias = x.CustomAlias,
                ExpiresAt = x.ExpiresAt,
                IsActive = x.IsActive,
                ClickCount = x.ClickCount,
                CreatedAt = x.CreatedAt
            })
            .ToListAsync(ct);

        await SendAsync(urls, cancellation: ct);
    }
}
