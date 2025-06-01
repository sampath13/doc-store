using MediatR;

namespace DocStore.Analyzer.Net.Features.Search;

public class SearchContentRequest : IRequest<SearchContentResponse>
{
    public string Query { get; set; }
}