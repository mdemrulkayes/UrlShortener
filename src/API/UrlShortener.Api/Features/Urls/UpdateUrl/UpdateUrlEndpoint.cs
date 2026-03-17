using System.Security.Claims;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using UrlShortener.Api.Data;

namespace UrlShortener.Api.Features.Urls.UpdateUrl;

public class UpdateUrlEndpoint : Endpoint<UpdateUrlRequest>
{
    private readonly ApplicationDbContext _db;

    public UpdateUrlEndpoint(ApplicationDbContext db)
    {
        _db = db;
    }

    public override void Configure()
    {
        Put("/api/urls/{id}");
        Group<UrlsGroup>();
    }

    public override async Task HandleAsync(UpdateUrlRequest req, CancellationToken ct)
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

        url.LongUrl = req.LongUrl;
        url.ExpiresAt = req.ExpiresAt;
        await _db.SaveChangesAsync(ct);

        await SendNoContentAsync(ct);
    }
}
