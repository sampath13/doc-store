namespace DocStore.Analyzer.Net.Services.Queue;

public sealed class DocStoreMessage<TKey, TValue>
{
    public MessageHeader Header { get; set; }
    public TKey Key { get; set; }
    public TValue Payload { get; set; }
}