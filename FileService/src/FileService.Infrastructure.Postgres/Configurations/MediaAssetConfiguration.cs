using System.Text.Json;
using FileService.Domain;
using FileService.Domain.Assets;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FileService.Infrastructure.Postgres.Configurations;

public class MediaAssetConfiguration : IEntityTypeConfiguration<MediaAsset>
{
    public void Configure(EntityTypeBuilder<MediaAsset> builder)
    {
        builder.ToTable("media_asset");

        builder.HasKey(ma => ma.Id);
        builder.Property(ma => ma.Id).HasColumnName("id");

        builder.HasDiscriminator<string>("media_type")
            .HasValue<VideoAsset>("video")
            .HasValue<PreviewAsset>("preview");

        builder.OwnsOne(ma => ma.MediaData, mab =>
        {
            mab.ToJson("media_data");

            mab.OwnsOne(md => md.FileName, mdb =>
            {
                mdb.Property(fn => fn.Value);
                mdb.Property(fn => fn.Extension);
            });

            mab.OwnsOne(md => md.ContentType, mdb =>
            {
                mdb.Property(ct => ct.Value);
                mdb.Property(ct => ct.Category).HasConversion<string>();
            });

            mab.Property(s => s.Size);
            mab.Property(e => e.ExpectedChuncksCount);
        });

        builder.Property(ma => ma.AssetType).HasConversion<string>().HasColumnName("asset_type");
        builder.Property(ma => ma.MediaStatus).HasConversion<string>().HasColumnName("media_status");

        builder.Property(ma => ma.CreatedAt).HasColumnName("created_at");
        builder.Property(ma => ma.UpdatedAt).HasColumnName("updated_at");

        builder.Property(ma => ma.RawKey)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<StorageKey>(v, (JsonSerializerOptions?)null)!)
            .HasColumnName("raw_key")
            .HasColumnType("jsonb");

        builder.Property(ma => ma.FinalKey)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<StorageKey>(v, (JsonSerializerOptions?)null)!)
            .HasColumnName("final_key")
            .HasColumnType("jsonb");

        builder.OwnsOne(ma => ma.MediaOwner, mab =>
        {
            mab.ToJson("media_owner");

            mab.Property(c => c.Context);
            mab.Property(e => e.EntityId);
        });
    }
}