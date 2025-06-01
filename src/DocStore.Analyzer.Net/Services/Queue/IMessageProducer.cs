namespace DocStore.Analyzer.Net.Services.Queue;

public interface IMessageProducer
{
    Task SendAsync<TKey, TValue>(DocStoreMessage<TKey, TValue> message, CancellationToken cancellationToken = default);
}