using DocStore.Analyzer.Net.Search;
using MediatR;

namespace DocStore.Analyzer.Net.Features.Search;

public sealed class SearchContentHandler(ISearchClient searchClient)
    : IRequestHandler<SearchContentRequest, SearchContentResponse>
{
    public async Task<SearchContentResponse> Handle(SearchContentRequest request, CancellationToken cancellationToken)
    {
        //TODO: enable full-text search. ATM it is 'LIKE' search
        var pages = await searchClient.SearchAsync(request.Query, 10, cancellationToken);
        var validPages = pages
            .Where(x => x.DocumentId != Guid.Empty)
            .ToList();
        
        var files = new List<MatchedDocument>();

        foreach (var doc in validPages.GroupBy(x => new { x.DocumentId, x.DocumentName }))
        {
            var matchingPages = validPages
                .Where(x => x.DocumentId == doc.Key.DocumentId)
                .Select(x => x.PageNumber)
                .ToList();

            files.Add(new MatchedDocument
            {
                Id = doc.Key.DocumentId,
                Name = doc.Key.DocumentName,
                PageNumbers = string.Join(",", matchingPages)
            });
        }
        
        
        return new SearchContentResponse
        {
            Files = files
        };
    }
}