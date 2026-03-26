using System.Security.Claims;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using UrlShortener.Api.Data;

namespace UrlShortener.Api.Features.Urls.ToggleUrl;

public class ToggleUrlEndpoint(ApplicationDbContext db) : EndpointWithoutRequest
{
    public override void Configure()
    {
        Patch("/api/urls/{id}/toggle");
        Group<UrlsGroup>();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = Route<long>("id");
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        var url = await db.ShortenedUrls
            .FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId, ct);

        if (url is null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        url.IsActive = !url.IsActive;
        await db.SaveChangesAsync(ct);
        await SendNoContentAsync(ct);
    }
}
