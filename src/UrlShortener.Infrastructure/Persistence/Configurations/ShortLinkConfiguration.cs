using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UrlShortener.Domain.ShortLinks;

namespace UrlShortener.Infrastructure.Persistence.Configurations;

public sealed class ShortLinkConfiguration : IEntityTypeConfiguration<ShortLink>
{
    public void Configure(EntityTypeBuilder<ShortLink> builder)
    {
        builder.ToTable("short_links");

        builder.HasKey(shortLink => shortLink.Id);

        builder.Property(shortLink => shortLink.Id)
            .HasColumnName("id");

        builder.Property(shortLink => shortLink.Code)
            .HasColumnName("code")
            .HasMaxLength(32)
            .IsRequired();

        builder.Property(shortLink => shortLink.OriginalUrl)
            .HasColumnName("original_url")
            .HasMaxLength(2048)
            .IsRequired();

        builder.Property(shortLink => shortLink.CreatedAtUtc)
            .HasColumnName("created_at_utc")
            .IsRequired();

        builder.Property(shortLink => shortLink.ExpiresAtUtc)
            .HasColumnName("expires_at_utc");

        builder.Property(shortLink => shortLink.ClickCount)
            .HasColumnName("click_count")
            .IsRequired();

        builder.HasIndex(shortLink => shortLink.Code)
            .IsUnique();
    }
}
