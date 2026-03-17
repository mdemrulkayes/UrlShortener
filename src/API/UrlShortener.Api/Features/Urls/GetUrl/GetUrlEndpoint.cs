using System.Security.Claims;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using UrlShortener.Api.Data;

namespace UrlShortener.Api.Features.Urls.GetUrl;

public class GetUrlEndpoint : EndpointWithoutRequest<GetUrlResponse>
{
    private readonly ApplicationDbContext _db;

    public GetUrlEndpoint(ApplicationDbContext db)
    {
        _db = db;
    }

    public override void Configure()
    {
        Get("/api/urls/{id}");
        Group<UrlsGroup>();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = Route<long>("id");
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        var url = await _db.ShortenedUrls
            .FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId, ct);

        if (url is null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        await SendAsync(new GetUrlResponse
        {
            Id = url.Id,
            LongUrl = url.LongUrl,
            ShortCode = url.ShortCode,
            CustomAlias = url.CustomAlias,
            ExpiresAt = url.ExpiresAt,
            IsActive = url.IsActive,
            ClickCount = url.ClickCount,
            CreatedAt = url.CreatedAt
        }, cancellation: ct);
    }
}
