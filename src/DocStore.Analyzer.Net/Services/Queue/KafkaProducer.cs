using Confluent.Kafka;
using DocStore.Analyzer.Net.Kafka;
using DocStore.Data.Model;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DocStore.Analyzer.Net.Services.Queue;

public sealed class KafkaProducer : IMessageProducer
{
    private readonly ILogger<KafkaProducer> _logger;
    private readonly AppSettings _appSettings;

    public KafkaProducer(IOptions<AppSettings> appSettingsOptions, ILogger<KafkaProducer> logger)
    {
        _logger = logger;
        _appSettings = appSettingsOptions.Value;
    }
    
    public async Task SendAsync<TKey, TValue>(DocStoreMessage<TKey, TValue> message, CancellationToken cancellationToken = default)
    {
        var producerConfig = new ProducerConfig
        {
            BootstrapServers = $"{_appSettings.Kafka.BrokerHost}:{_appSettings.Kafka.BrokerPort}"
        };
        
        using var producer = new ProducerBuilder<TKey, TValue>(producerConfig)
            .SetLogHandler((pr, x) =>
            {
                switch (x.Level)
                {
                    case SyslogLevel.Debug:
                        _logger.LogDebug("{Message}", x.Message);
                        break;
                    case SyslogLevel.Error:
                        _logger.LogError("{Message}", x.Message);
                        break;
                    case SyslogLevel.Warning:
                        _logger.LogWarning("{Message}", x.Message);
                        break;
                    default:
                        _logger.LogInformation("{Message}", x.Message);
                        break;
                }
                
            })
            .Build();
        await producer.ProduceAsync(_appSettings.Kafka.TopicIndexDocs, 
            new Message<TKey, TValue> { Key = message.Key, Value = message.Payload }, cancellationToken);    
    }
}