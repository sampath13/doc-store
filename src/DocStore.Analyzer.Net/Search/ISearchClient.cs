using DocStore.Analyzer.Net.Models;
using Elastic.Clients.Elasticsearch;

namespace DocStore.Analyzer.Net.Search;

public interface ISearchClient
{
    Task<BulkResponse> IndexAsync(DocumentData document, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<PageData>> SearchAsync(string query, int pageSize = 10,
        CancellationToken cancellationToken = default);
}