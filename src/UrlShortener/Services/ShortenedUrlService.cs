using Microsoft.EntityFrameworkCore;
using UrlShortener.Db;
using UrlShortener.Models;

namespace UrlShortener.Services;

internal sealed class ShortenedUrlService(ApplicationDbContext dbContext)
{
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
            ShortCode = GenerateShortCode()
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

    private string GenerateShortCode()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var random = new Random();
        return new string(Enumerable.Repeat(chars, 6)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }
}
