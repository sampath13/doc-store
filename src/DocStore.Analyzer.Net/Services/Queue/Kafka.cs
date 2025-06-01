namespace DocStore.Analyzer.Net.Services.Queue;

public sealed class Kafka
{
    public string BrokerHost { get; init; }
    public string BrokerPort { get; init; }
    public string TopicIndexDocs { get; init; }
}