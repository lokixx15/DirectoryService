using System.Text.Json;
using FileService.Domain;
using FileService.Domain.MediaProcessing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FileService.Infrastructure.Postgres.Configurations;

public class VideoProcessConfiguration : IEntityTypeConfiguration<VideoProcess>
{
    public void Configure(EntityTypeBuilder<VideoProcess> builder)
    {
        builder.ToTable("video_processes");

        builder.HasKey(vp => vp.Id).HasName("pk_video_process_id");
        builder.Property(vp => vp.Id).HasColumnName("id");

        builder.Property(vp => vp.RawKey)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<StorageKey>(v, (JsonSerializerOptions?)null)!)
            .HasColumnName("raw_key")
            .HasColumnType("jsonb");

        builder.Property(vp => vp.HlsKey)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<StorageKey>(v, (JsonSerializerOptions?)null)!)
            .HasColumnName("hls_key")
            .HasColumnType("jsonb");

        builder.OwnsMany(vp => vp.Steps, sb =>
        {
            sb.ToTable("video_process_steps");

            sb.WithOwner().HasForeignKey("ProcessId");
            sb.Property<Guid>("ProcessId").HasColumnName("process_id");

            sb.HasKey(vp => vp.Id).HasName("pk_video_process_step_id");
            sb.Property(vps => vps.Id).HasColumnName("id");

            sb.Property(vps => vps.ProcessId).HasColumnName("process_id");
            sb.Property(vps => vps.Order).IsRequired().HasColumnName("order");
            sb.Property(vps => vps.StepType).HasConversion<string>().HasColumnName("step_type");
            sb.Property(vps => vps.Status).HasConversion<string>().HasColumnName("status");
            sb.Property(vps => vps.Progress).IsRequired().HasColumnName("progress").HasDefaultValue(0);

            sb.Property(vps => vps.StartedAt).IsRequired(false).HasColumnName("started_at").HasColumnType("timestamp with time zone");
            sb.Property(vps => vps.CompletedAt).IsRequired(false).HasColumnName("completed_at").HasColumnType("timestamp with time zone");

            sb.Property(vps => vps.ErrorMessage).IsRequired(false).HasColumnName("error_message");
            sb.Property(vps => vps.CreatedAt).IsRequired().HasColumnName("created_at").HasColumnType("timestamp with time zone");
            sb.Property(vps => vps.UpdatedAt).IsRequired().HasColumnName("updated_at").HasColumnType("timestamp with time zone");
        });

        builder.OwnsOne(vp => vp.Metadata, mb =>
        {
            mb.ToJson("metadata");

            mb.Property(m => m.Duration).IsRequired(false).HasColumnName("duration");
            mb.Property(m => m.Width).IsRequired(false).HasColumnName("width");
            mb.Property(m => m.Height).IsRequired(false).HasColumnName("height");
            mb.Property(m => m.Codec).IsRequired(false).HasColumnName("codec");
        });

        builder.Navigation(vp => vp.Metadata).IsRequired(false);

        builder.Property(vp => vp.TotalProgress).IsRequired().HasColumnName("total_progress").HasDefaultValue(0);
        builder.Property(vp => vp.Status).HasConversion<string>().HasColumnName("status");
        builder.Property(vp => vp.ErrorMessage).IsRequired(false).HasColumnName("error_message");
        builder.Property(vp => vp.CurrentStepOrder).IsRequired(false).HasColumnName("current_step_order");
        builder.Property(vp => vp.CurrentStepType).HasConversion<string>().IsRequired(false).HasColumnName("current_step_type");
        builder.Property(vp => vp.CurrentStepProgress).IsRequired().HasColumnName("current_step_progress").HasDefaultValue(0);
        builder.Property(vp => vp.CreatedAt).IsRequired().HasColumnName("created_at").HasColumnType("timestamp with time zone");
        builder.Property(vp => vp.UpdatedAt).IsRequired().HasColumnName("updated_at").HasColumnType("timestamp with time zone");

        builder.HasIndex(vp => vp.Status).HasDatabaseName("ix_video_process_status");
        builder.HasIndex(vp => vp.CreatedAt).HasDatabaseName("ix_video_process_created_at");
    }
}