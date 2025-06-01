namespace DocStore.Analyzer.Net.Models;

public sealed class DocumentData {
    public IList<PageData> Pages { get; set; } = null!;
    public int TotalPages { get; set; }
    public string Title { get; set; } = null!;
    public IList<string> Authors { get; set; } = null!;
    public IList<string> Genre { get; set; } = null!;
    public IList<string> Tags { get; set; } = null!;

}