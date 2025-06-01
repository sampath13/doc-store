using MediatR;

namespace DocStore.Analyzer.Net.Features.List;

public class ListDocumentsRequest : IRequest<ListDocumentsResponse>
{
    
}

public class ListDocumentsResponse
{
    public ICollection<Document> Files { get; set; }
}

public class Document
{
    public Guid Id { get; set; }
    public string Name { get; set; }
}