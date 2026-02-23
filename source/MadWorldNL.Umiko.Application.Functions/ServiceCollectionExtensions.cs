using System.Xml.Schema;
using MadWorldNL.Umiko.Developer;
using MadWorldNL.Umiko.ServiceBus;
using MadWorldNL.Umiko.Status;
using Microsoft.Extensions.DependencyInjection;

namespace MadWorldNL.Umiko;

public static class FunctionsServiceCollectionExtensions
{
    public static IServiceCollection AddFunctionsServices(this IServiceCollection services)
    {
        services.AddScoped<ICommandHandler<ProcessTestCommand>, ProcessTestCommandFunction>();
        
        services.AddScoped<IQueryHandler<GetDatabaseStatusQuery, GetDatabaseStatusResult>, GetDatabaseStatusFunction>();
        services.AddScoped<IQueryHandler<GetMessagingStatusQuery, GetMessagingStatusResult>, GetMessagingStatusFunction>();
        
        // TODO: Add when implemented a first command handler
        //services.Decorate(typeof(ICommandHandler<,>), typeof(LoggingCommandHandler<,>));
        
        services.Decorate(typeof(ICommandHandler<>), typeof(LoggingCommandHandler<>));
        services.Decorate(typeof(IQueryHandler<,>), typeof(LoggingQueryHandler<,>));

        return services;
    }
}