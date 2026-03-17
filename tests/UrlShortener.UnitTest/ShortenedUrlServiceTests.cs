using Shouldly;
using UrlShortener.Api.Data;
using UrlShortener.Api.Common;
using Microsoft.EntityFrameworkCore;
using UrlShortener.Api.Models;

namespace UrlShortener.UnitTest;

public class ShortenedUrlTests
{
    private ApplicationDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new ApplicationDbContext(options);
    }

    [Fact]
    public async Task ShortenUrl_ShouldCreateNewRecord()
    {
        // Arrange
        var db = CreateDbContext();
        var shortCode = Utils.GenerateShortCode(6);
        var url = new ShortenedUrl
        {
            LongUrl = "https://example.com",
            ShortCode = shortCode,
            UserId = "test-user-id"
        };

        // Act
        await db.ShortenedUrls.AddAsync(url);
        await db.SaveChangesAsync();

        // Assert
        var saved = await db.ShortenedUrls.FirstOrDefaultAsync(x => x.ShortCode == shortCode);
        saved.ShouldNotBeNull();
        saved.LongUrl.ShouldBe("https://example.com");
        saved.ShortCode.Length.ShouldBe(6);
    }

    [Fact]
    public async Task GetLongUrl_ShouldReturnUrl_WhenShortCodeExists()
    {
        // Arrange
        var db = CreateDbContext();
        var shortCode = "abc123";
        var longUrl = "https://example.com";
        await db.ShortenedUrls.AddAsync(new ShortenedUrl { ShortCode = shortCode, LongUrl = longUrl, UserId = "test-user" });
        await db.SaveChangesAsync();

        // Act
        var result = await db.ShortenedUrls.FirstOrDefaultAsync(x => x.ShortCode == shortCode);

        // Assert
        result.ShouldNotBeNull();
        result.LongUrl.ShouldBe(longUrl);
    }

    [Fact]
    public async Task GetLongUrl_ShouldReturnNull_WhenShortCodeNotFound()
    {
        // Arrange
        var db = CreateDbContext();

        // Act
        var result = await db.ShortenedUrls.FirstOrDefaultAsync(x => x.ShortCode == "nonexistent");

        // Assert
        result.ShouldBeNull();
    }

    [Fact]
    public async Task GetUrls_ShouldReturnUserUrls_FilteredByUserId()
    {
        // Arrange
        var db = CreateDbContext();
        var userId = "user-1";
        await db.ShortenedUrls.AddRangeAsync(
            new ShortenedUrl { ShortCode = "abc123", LongUrl = "https://example.com", UserId = userId },
            new ShortenedUrl { ShortCode = "def456", LongUrl = "https://example.org", UserId = userId },
            new ShortenedUrl { ShortCode = "ghi789", LongUrl = "https://other.com", UserId = "user-2" }
        );
        await db.SaveChangesAsync();

        // Act
        var result = await db.ShortenedUrls.Where(x => x.UserId == userId).ToListAsync();

        // Assert
        result.Count.ShouldBe(2);
    }

    [Fact]
    public async Task ToggleUrl_ShouldFlipIsActive()
    {
        // Arrange
        var db = CreateDbContext();
        var url = new ShortenedUrl { ShortCode = "abc123", LongUrl = "https://example.com", UserId = "user-1", IsActive = true };
        await db.ShortenedUrls.AddAsync(url);
        await db.SaveChangesAsync();

        // Act
        url.IsActive = !url.IsActive;
        await db.SaveChangesAsync();

        // Assert
        var updated = await db.ShortenedUrls.FirstAsync(x => x.ShortCode == "abc123");
        updated.IsActive.ShouldBeFalse();
    }

    [Fact]
    public async Task ClickEvent_ShouldBeTracked()
    {
        // Arrange
        var db = CreateDbContext();
        var url = new ShortenedUrl { ShortCode = "abc123", LongUrl = "https://example.com", UserId = "user-1" };
        await db.ShortenedUrls.AddAsync(url);
        await db.SaveChangesAsync();

        // Act
        var click = new ClickEvent { ShortenedUrlId = url.Id, IpAddress = "127.0.0.1" };
        db.ClickEvents.Add(click);
        url.ClickCount++;
        await db.SaveChangesAsync();

        // Assert
        var clicks = await db.ClickEvents.Where(x => x.ShortenedUrlId == url.Id).ToListAsync();
        clicks.Count.ShouldBe(1);
        url.ClickCount.ShouldBe(1);
    }
}
