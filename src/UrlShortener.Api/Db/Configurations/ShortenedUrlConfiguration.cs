using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UrlShortener.Api.Models;

namespace UrlShortener.Api.Db.Configurations;

public class ShortenedUrlConfiguration : IEntityTypeConfiguration<ShortenedUrl>
{
    public void Configure(EntityTypeBuilder<ShortenedUrl> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.Property(x => x.LongUrl).IsRequired();

        builder.HasIndex(x => x.ShortCode).IsUnique();
        builder.Property(x => x.ShortCode).HasMaxLength(6);
    }
}
