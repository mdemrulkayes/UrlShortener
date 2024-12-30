using Microsoft.EntityFrameworkCore;
using UrlShortener.Models;

namespace UrlShortener.Db;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<ShortenedUrl> ShortenedUrls { get; set; }
}
