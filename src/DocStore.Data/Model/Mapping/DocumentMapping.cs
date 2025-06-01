using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DocStore.Data.Model.Mapping;

public sealed class DocumentMapping : IEntityTypeConfiguration<Document>
{
    public void Configure(EntityTypeBuilder<Document> builder)
    {
        builder.HasKey(d => d.Id);
        builder.Property(d => d.Id).ValueGeneratedOnAdd();
        builder.Property(d => d.Name).HasMaxLength(100).IsRequired();
        builder.Property(d => d.AddedDate).IsRequired();
        builder.Property(d => d.AddedBy).IsRequired();
        builder.HasMany(d => d.Pages).WithOne(p => p.Document);
        builder.HasMany(d => d.Stages).WithOne(p => p.Document);
        builder.Property(d => d.Status).HasConversion(new EnumToStringConverter<Status>());
    }
}

public sealed class DocumentPageMapping : IEntityTypeConfiguration<DocumentPage>
{
    public void Configure(EntityTypeBuilder<DocumentPage> builder)
    {
        builder.HasKey(d => d.Id);
        builder.Property(d => d.Id).ValueGeneratedOnAdd();
        builder.HasIndex(d => d.SearchIndexPageId).IsUnique();
        builder.HasOne(d => d.Document).WithMany(p => p.Pages).HasForeignKey(d => d.DocumentId);;
    }
}

public sealed class DocumentProcessingStageMapping : IEntityTypeConfiguration<DocumentProcessingStage>
{
    public void Configure(EntityTypeBuilder<DocumentProcessingStage> builder)
    {
        builder.HasKey(d => d.Id);
        builder.Property(d => d.Id).ValueGeneratedOnAdd();
        builder.Property(d => d.Stage).IsRequired().HasConversion<EnumToStringConverter<Stage>>();
        builder.HasOne(d => d.Document).WithMany(p => p.Stages).HasForeignKey(d => d.DocumentId);;
    }
}