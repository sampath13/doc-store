namespace DocStore.Data.Model;

public sealed class Document
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int TotalPages { get; set; }
    public DateTimeOffset AddedDate { get; set; } = DateTimeOffset.UtcNow;
    public string StorePath { get; set; }
    public Status Status { get; set; }
    public Guid AddedBy { get; set; }
    public ICollection<DocumentPage> Pages { get; set; } =  new List<DocumentPage>();
    public ICollection<DocumentProcessingStage> Stages { get; set; } = new List<DocumentProcessingStage>();
}

public enum Status
{
    Draft = 0,
    Visible,
    Hidden,
    Deleted
}

public sealed class DocumentProcessingStage
{
    public Guid Id { get; set; }
    public Guid DocumentId { get; set; }
    public Stage Stage { get; set; }
    public string? Message { get; set; }
    public DateTimeOffset AddedDate { get; set; } = DateTimeOffset.UtcNow;
    public Document Document { get; set; }
}

public enum Stage
{
    None = 0,
    Received,
    InitProcessing,
    Processing,
    Processed,
    Failed
}

public sealed class DocumentPage
{
    public Guid Id { get; set; }
    public Guid DocumentId { get; set; }
    public string SearchIndexPageId { get; set; } =  string.Empty;
    public int PageNumber { get; set; }
    public Document Document { get; set; }
}