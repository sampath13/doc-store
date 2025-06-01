using System.Text;
using DocStore.Analyzer.Net.Models;
using DocStore.Analyzer.Net.Services.Queue;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.Mapping;
using Microsoft.Extensions.Logging;

namespace DocStore.Analyzer.Net.Services.Search;

public sealed class IndexCheck
{
    private readonly AppSettings _appSettings;
    private readonly ILogger<IndexCheck> _logger;
    private readonly ElasticsearchClient _client;

    public IndexCheck(AppSettings appSettings, ILogger<IndexCheck> logger)
    {
        _appSettings = appSettings;
        _logger = logger;
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

    public async Task CreateIfNotExistsAsync(CancellationToken cancellationToken = default)
    {
        // DO THIS ONLY ONCE
        _logger.LogInformation("Creating index");
        var exists =
            await _client.Indices.ExistsAsync(_appSettings.Search.IndexName, cancellationToken: cancellationToken);
        if (!exists.Exists)
        {
            await _client.Indices.CreateAsync<PageData>(index =>
                    index.Index(_appSettings.Search.IndexName)
                        .Mappings(ms =>
                            ms.Properties(ps =>
                                ps.LongNumber(ln => ln.PageNumber)
                                    .LongNumber(ln => ln.TotalPages)
                                    .Text(t => t.DocumentId)
                                    .Keyword(k => k.Content,
                                        k => k.IgnoreAbove(8000).SplitQueriesOnWhitespace()
                                    )
                            )
                        )
                , cancellationToken);
        }
        _logger.LogInformation("Created index");
    }
}