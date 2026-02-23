using MadWorldNL.Umiko.ServiceBus;
using Microsoft.Extensions.DependencyInjection;

namespace MadWorldNL.Umiko;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRabbitMqServices(this IServiceCollection services)
    {
        services.AddSingleton<IMessageBus, RabbitMqMessageBus>();
        return services;
    }
}