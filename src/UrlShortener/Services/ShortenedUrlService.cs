﻿using Microsoft.EntityFrameworkCore;
using UrlShortener.Db;
using UrlShortener.Models;

namespace UrlShortener.Services;

internal sealed class ShortenedUrlService(ApplicationDbContext dbContext)
{
    private const int ShortCodeLength = 6;
    public async Task<string> ShortenUrlAsync(string longUrl)
    {
        var (isLongUrlExists, shortCode) = await LongUrlExistsAsync(longUrl);

        if (isLongUrlExists)
        {
            return shortCode!;
        }

        var shortenedUrl = new ShortenedUrl
        {
            LongUrl = longUrl,
            ShortCode = Utils.GenerateShortCode(ShortCodeLength)
        };
        await dbContext.ShortenedUrls.AddAsync(shortenedUrl);
        await dbContext.SaveChangesAsync();
        return shortenedUrl.ShortCode;
    }

    private async Task<(bool isExists, string? shortCode)> LongUrlExistsAsync(string longUrl)
    {
        var shortenedUrlDetails = await dbContext.ShortenedUrls
            .FirstOrDefaultAsync(x => x.LongUrl.ToLower() == longUrl.ToLower());
        return (shortenedUrlDetails != null, shortenedUrlDetails?.ShortCode);
    }

    public async Task<string?> GetLongUrlAsync(string shortCode)
    {
        var shortenedUrl = await dbContext.ShortenedUrls
            .FirstOrDefaultAsync(x => x.ShortCode == shortCode);
        return shortenedUrl?.LongUrl;
    }

    public async Task<IEnumerable<ShortenedUrl>> GetUrls()
    {
        return await dbContext.ShortenedUrls
            .OrderByDescending(x => x.Id)
            .ToListAsync();
    }
}
