using Microsoft.Extensions.DependencyInjection;

namespace DocStore.Analyzer.Net.Services.Queue;

public static class ServiceExtensions
{
    public static IServiceCollection AddMessageProducer(this IServiceCollection services)
        => services.AddScoped<IMessageProducer, KafkaProducer>();
}