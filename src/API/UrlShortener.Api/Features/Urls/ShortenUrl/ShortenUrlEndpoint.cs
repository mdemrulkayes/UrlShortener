using System.Security.Claims;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using UrlShortener.Api.Common;
using UrlShortener.Api.Data;
using UrlShortener.Api.Models;

namespace UrlShortener.Api.Features.Urls.ShortenUrl;

public class ShortenUrlEndpoint(ApplicationDbContext db) : Endpoint<ShortenUrlRequest, ShortenUrlResponse>
{
    public override void Configure()
    {
        Post("/api/urls");
        Group<UrlsGroup>();
    }

    public override async Task HandleAsync(ShortenUrlRequest req, CancellationToken ct)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        // Check if custom alias is already taken
        if (!string.IsNullOrEmpty(req.CustomAlias))
        {
            var aliasExists = await db.ShortenedUrls
                .AnyAsync(x => x.ShortCode == req.CustomAlias, ct);
            if (aliasExists)
            {
                AddError("Custom alias is already taken");
                await SendErrorsAsync(cancellation: ct);
                return;
            }
        }

        // Check if user already shortened this URL
        var existing = await db.ShortenedUrls
            .FirstOrDefaultAsync(x => x.LongUrl.ToLower() == req.LongUrl.ToLower() && x.UserId == userId, ct);

        if (existing is not null)
        {
            await SendAsync(new ShortenUrlResponse
            {
                Id = existing.Id,
                ShortCode = existing.ShortCode,
                LongUrl = existing.LongUrl,
                CreatedAt = existing.CreatedAt
            }, cancellation: ct);
            return;
        }

        var shortenedUrl = new ShortenedUrl
        {
            LongUrl = req.LongUrl,
            ShortCode = req.CustomAlias ?? Utils.GenerateShortCode(6),
            CustomAlias = req.CustomAlias,
            ExpiresAt = req.ExpiresAt,
            UserId = userId
        };

        await db.ShortenedUrls.AddAsync(shortenedUrl, ct);
        await db.SaveChangesAsync(ct);

        await SendCreatedAtAsync<ShortenUrlEndpoint>(
            routeValues: null,
            responseBody: new ShortenUrlResponse
            {
                Id = shortenedUrl.Id,
                ShortCode = shortenedUrl.ShortCode,
                LongUrl = shortenedUrl.LongUrl,
                CreatedAt = shortenedUrl.CreatedAt
            },
            cancellation: ct);
    }
}
