namespace DocStore.Analyzer.Net.Features.Search;

public class SearchContentResponse
{
    public ICollection<MatchedDocument> Files { get; set; }

}

public class MatchedDocument
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string PageNumbers { get; set; }
}