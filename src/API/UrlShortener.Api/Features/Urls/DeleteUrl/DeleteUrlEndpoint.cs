using System.Security.Claims;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using UrlShortener.Api.Data;

namespace UrlShortener.Api.Features.Urls.DeleteUrl;

public class DeleteUrlEndpoint : EndpointWithoutRequest
{
    private readonly ApplicationDbContext _db;

    public DeleteUrlEndpoint(ApplicationDbContext db)
    {
        _db = db;
    }

    public override void Configure()
    {
        Delete("/api/urls/{id}");
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

        _db.ShortenedUrls.Remove(url);
        await _db.SaveChangesAsync(ct);
        await SendNoContentAsync(ct);
    }
}
