using Microsoft.EntityFrameworkCore;
using UrlShortener.Db;
using UrlShortener.Models;

namespace UrlShortener.Services;

internal sealed class ShortenedUrlService(ApplicationDbContext dbContext)
{
    public async Task<string> ShortenUrlAsync(string longUrl)
    {
        var shortenedUrl = new ShortenedUrl
        {
            LongUrl = longUrl,
            ShortCode = GenerateShortCode()
        };
        await dbContext.ShortenedUrls.AddAsync(shortenedUrl);
        await dbContext.SaveChangesAsync();
        return shortenedUrl.ShortCode;
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
        //Generate a random string of 6 characters
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var random = new Random();
        var theCode = new string(Enumerable.Repeat(chars, 6)
            .Select(s => s[random.Next(s.Length)]).ToArray());

        //Check if the code is unique
        var isUnique = dbContext.ShortenedUrls
            .Any(x => x.ShortCode == theCode);

        //If the code is not unique, generate a new one
        return isUnique ? GenerateShortCode() : theCode;
    }
}
