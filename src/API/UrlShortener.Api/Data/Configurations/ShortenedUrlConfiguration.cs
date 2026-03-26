using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UrlShortener.Api.Models;

namespace UrlShortener.Api.Data.Configurations;

public class ShortenedUrlConfiguration : IEntityTypeConfiguration<ShortenedUrl>
{
    public void Configure(EntityTypeBuilder<ShortenedUrl> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.LongUrl).IsRequired();
        builder.HasIndex(x => x.ShortCode).IsUnique();
        builder.Property(x => x.ShortCode).HasMaxLength(50);
        builder.Property(x => x.CustomAlias).HasMaxLength(50);
        builder.Property(x => x.IsActive).HasDefaultValue(true);
        builder.Property(x => x.ClickCount).HasDefaultValue(0);

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.ClickEvents)
            .WithOne(x => x.ShortenedUrl)
            .HasForeignKey(x => x.ShortenedUrlId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
