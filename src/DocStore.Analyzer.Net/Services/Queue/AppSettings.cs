namespace DocStore.Analyzer.Net.Services.Queue;

public sealed class AppSettings
{
    public Kafka Kafka { get; init; }
    public Store Store { get; init; }
    public Search Search { get; init; }
}

public sealed class Search
{
    public string IndexName { get; set; }
    public Uri Uri { get; set; }
}

public sealed class Store
{
    public string Path { get; init; }
}