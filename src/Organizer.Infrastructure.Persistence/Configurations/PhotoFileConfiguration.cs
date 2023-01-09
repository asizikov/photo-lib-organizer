using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Organizer.Domain.Entities;

namespace Organizer.Infrastructure.Persistence.Configurations;

public class PhotoFileConfiguration : IEntityTypeConfiguration<PhotoFile>
{
    public void Configure(EntityTypeBuilder<PhotoFile> builder)
    {
        builder.HasAlternateKey(e => e.FilePath);
        builder.HasIndex(e => e.Hash);
        builder.HasIndex(e => e.Location);

        builder.Property(e => e.Id)
            .HasDefaultValueSql("NEWID()");
        
        builder.Property(e => e.FileName)
            .IsRequired()
            .HasMaxLength(200);
        
        builder.Property(e => e.FilePath)
            .IsRequired()
            .HasMaxLength(500);
        
        builder.Property(e => e.FileExtension)
            .IsRequired()
            .HasMaxLength(15);
        
        builder.Property(e => e.Hash).HasMaxLength(500);
        
        builder.Property(e => e.Longitude).HasColumnType("decimal(9,6)");
        builder.Property(e => e.Latitude).HasColumnType("decimal(9,6)");
        builder.Property(e => e.Location).HasMaxLength(500);
    }
}