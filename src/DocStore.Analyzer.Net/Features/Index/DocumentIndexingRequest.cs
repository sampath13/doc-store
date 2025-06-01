using MediatR;

namespace DocStore.Analyzer.Net.Features.Index;

public class DocumentIndexingRequest : IRequest<DocumentIndexingResponse>
{
    public string FileName { get; set; }
    public Stream Stream { get; set; }
}