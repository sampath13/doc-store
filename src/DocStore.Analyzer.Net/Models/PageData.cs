using System.Text.Json.Serialization;

namespace DocStore.Analyzer.Net.Models;

public sealed class PageData
{
    public Guid DocumentId { get; set; }
    public string DocumentName { get; set; }
    public int PageNumber { get; set; }
    public int TotalPages { get; set; }
    [JsonIgnore]
    public IList<string> Headers { get; set; } = null!;
    [JsonIgnore]
    public IList<string> Footers { get; set; }  = null!;
    public string Content { get; set; }  = null!;
}