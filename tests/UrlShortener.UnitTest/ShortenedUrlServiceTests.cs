using Shouldly;
using UrlShortener.Api.Db;
using UrlShortener.Api.Services;
using Microsoft.EntityFrameworkCore;
using UrlShortener.Api.Models;

namespace UrlShortener.UnitTest;

public class ShortenedUrlServiceTests
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ShortenedUrlService _service;

    public ShortenedUrlServiceTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "UrlShortenerTestDb")
            .Options;
        _dbContext = new ApplicationDbContext(options);
        _service = new ShortenedUrlService(_dbContext);
    }

    [Fact]
    public async Task ShortenUrlAsync_ShouldReturnShortCode()
    {
        // Arrange
        var longUrl = "https://example.com";

        // Act
        var shortCode = await _service.ShortenUrlAsync(longUrl);

        // Assert
        shortCode.ShouldNotBeNullOrEmpty();
        shortCode.Length.ShouldBe(6);
    }

    [Fact]
    public async Task GetLongUrlAsync_ShouldReturnLongUrl()
    {
        // Arrange
        var shortCode = "abc123";
        var longUrl = "https://example.com";

        await _dbContext.ShortenedUrls.AddAsync(new ShortenedUrl { ShortCode = shortCode, LongUrl = longUrl });
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _service.GetLongUrlAsync(shortCode);

        // Assert
        result.ShouldBe(longUrl);
    }

    [Fact]
    public async Task GetLongUrlAsync_ShouldReturnNull_WhenShortCodeNotFound()
    {
        // Arrange
        var shortCode = "nonexistent";

        // Act
        var result = await _service.GetLongUrlAsync(shortCode);

        // Assert
        result.ShouldBeNull();
    }

    [Fact]
    public async Task GetUrls_ShouldReturnAllUrls()
    {
        // Arrange
        var urls = new List<ShortenedUrl>
        {
            new() { ShortCode = "abc123", LongUrl = "https://example.com" },
            new() { ShortCode = "def456", LongUrl = "https://example.org" }
        };
        await _dbContext.ShortenedUrls.AddRangeAsync(urls);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _service.GetUrls();

        // Assert
        result.ShouldNotBeNull();
        result.Count().ShouldBe(2);
    }
}
