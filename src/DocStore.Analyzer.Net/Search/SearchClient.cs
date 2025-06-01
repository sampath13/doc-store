using System.Text;
using DocStore.Analyzer.Net.Models;
using DocStore.Analyzer.Net.Services.Queue;
using Elastic.Clients.Elasticsearch;
using Microsoft.Extensions.Logging;

namespace DocStore.Analyzer.Net.Search;

public class SearchClient : ISearchClient
{
    private readonly ILogger<SearchClient> _logger;
    private readonly AppSettings _appSettings;
    private readonly ElasticsearchClient _client;

    public SearchClient(ILogger<SearchClient> logger, AppSettings appSettings)
    {
        _logger = logger;
        _appSettings = appSettings;
        var settings = new ElasticsearchClientSettings(appSettings.Search.Uri)
                .EnableDebugMode()
                .DisableDirectStreaming()
                .PrettyJson()
                .DefaultIndex(appSettings.Search.IndexName)
                .OnRequestCompleted(details =>
                {
                    if (details.RequestBodyInBytes == null) return;
                    
                    var requestJson = Encoding.UTF8.GetString(details.RequestBodyInBytes);
                    _logger.LogInformation("Request Url: {Url}", details.Uri);
                    _logger.LogInformation("Request Body: {@Body}", requestJson);
                    
                    if (details.ResponseBodyInBytes == null) return;
                    var responseJson = Encoding.UTF8.GetString(details.ResponseBodyInBytes);
                    _logger.LogInformation("Response Body: {@Body}", responseJson);
                });
        
        _client = new ElasticsearchClient(settings);
    }

    public async Task<BulkResponse> IndexAsync(DocumentData document, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _client.IndexManyAsync(document.Pages, _appSettings.Search.IndexName,  cancellationToken);
            if (!response.IsSuccess())
            {
                _logger.LogWarning("Unsuccessful response from indexing in elasticsearch");
                _logger.LogInformation("{DebugInformation}",response.DebugInformation);
                throw new OperationFailedException();
            }

            return response;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to index {IndexName}", _appSettings.Search.IndexName);
            throw;
        }
    }

    public async Task<IReadOnlyCollection<PageData>> SearchAsync(string query, int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var response = await _client.SearchAsync<PageData>(srch => srch
            .Index(_appSettings.Search.IndexName)
            // .From(0)
            // .Size(pageSize)
            .Query(q => q
                    .Match(m => m
                        .Field(f => f.Content)
                        .Query($"*{query}*")
                    )
            )
        ,  cancellationToken);
        
        return response.IsValidResponse ? response.Documents : Array.Empty<PageData>().AsReadOnly();
    }
}