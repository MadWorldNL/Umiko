using MadWorldNL.Umiko.Consumers;
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

    public static IServiceCollection AddCommandConsumer<TCommand>(this IServiceCollection services)
        where TCommand : ICommand
    {
        services.AddHostedService<CommandConsumer<TCommand>>();
        return services;
    }
}