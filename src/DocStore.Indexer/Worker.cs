using Confluent.Kafka;
using DocStore.Analyzer.Net.Reader;
using DocStore.Analyzer.Net.Search;
using DocStore.Analyzer.Net.Services.Queue;
using DocStore.Data.Context;
using DocStore.Data.Model;
using Microsoft.EntityFrameworkCore;

namespace DocStore.Indexer;

public class Worker(ILogger<Worker> logger, ISearchClient searchClient, AppSettings appSettings, IServiceProvider serviceProvider)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumerConfig = new ConsumerConfig
        {
            BootstrapServers = $"{appSettings.Kafka.BrokerHost}:{appSettings.Kafka.BrokerPort}",
            GroupId = "docstore-group",
            AutoOffsetReset = AutoOffsetReset.Earliest
        };
        
        using var consumer = new ConsumerBuilder<string, string>(consumerConfig)
            .SetLogHandler((_, x) =>
            {
                switch (x.Level)
                {
                    case SyslogLevel.Debug:
                        logger.LogDebug("{Message}", x.Message);
                        break;
                    case SyslogLevel.Error:
                        logger.LogError("{Message}", x.Message);
                        break;
                    case SyslogLevel.Warning:
                        logger.LogWarning("{Message}", x.Message);
                        break;
                    default:
                        logger.LogInformation("{Message}", x.Message);
                        break;
                }
                
            })
            .Build();
        consumer.Subscribe(appSettings.Kafka.TopicIndexDocs);
        
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = serviceProvider.CreateScope();
            var docStoreContext = scope.ServiceProvider.GetRequiredService<DocStoreContext>();
            
            var consumeResult = consumer.Consume(stoppingToken); // Long polling
            if (consumeResult != null)
            {
               var docId = Guid.Parse(consumeResult.Message.Value);
               var document = await docStoreContext.Documents.FirstOrDefaultAsync(d => d.Id == docId, stoppingToken);
               if (document == null)
               {
                   logger.LogWarning("Document {@DocId} not found", docId);
                   continue;
               }

               try
               {
                   using var pdfDocReader = new PdfDocReader(document.StorePath);
                   var docData = pdfDocReader.ReadDocument(document.StorePath, docId);
                   document.TotalPages = docData.TotalPages;
                   document.Stages.Add(new DocumentProcessingStage()
                   {
                       Stage = Stage.Processing
                   });
               
                   await docStoreContext.SaveChangesAsync(stoppingToken);
                   
                   var response = await searchClient.IndexAsync(docData, stoppingToken);
                   document.TotalPages = docData.TotalPages;
                   document.Pages ??= new List<DocumentPage>();

                   foreach (var item in response.Items)
                   {
                       var page = new DocumentPage
                       {
                           SearchIndexPageId = item.Id!,
                       };
                       document.Pages.Add(page);
                   }
                   document.Stages.Add(new DocumentProcessingStage()
                   {
                       Stage = Stage.Processed
                   });
                   
                   document.Status = Status.Visible;
                   await docStoreContext.SaveChangesAsync(stoppingToken);
               }
               catch (Exception e)
               {
                   logger.LogError(e, "Error processing document {@DocId}", docId);
                   document.Stages.Add(new DocumentProcessingStage()
                   {
                       Stage = Stage.Failed,
                       Message = e.Message
                   });
                   
                   await docStoreContext.SaveChangesAsync(stoppingToken);
               }
            }
            else
            {
                logger.LogInformation("No message consumed");
            }

            await Task.Delay(1000, stoppingToken);
        }
    }
}