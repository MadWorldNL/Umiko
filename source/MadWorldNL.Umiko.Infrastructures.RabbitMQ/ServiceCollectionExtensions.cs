using MadWorldNL.Umiko.ServiceBus;
using MadWorldNL.Umiko.Status;
using Microsoft.Extensions.DependencyInjection;

namespace MadWorldNL.Umiko;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRabbitMqServices(this IServiceCollection services)
    {
        services.AddSingleton<IMessageBus, RabbitMqMessageBus>();
        services.AddSingleton<IMessageBusStatusRepository, MessageBusStatusRepository>();
        return services;
    }
}