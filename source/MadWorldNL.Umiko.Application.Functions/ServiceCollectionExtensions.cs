using MadWorldNL.Umiko.ServiceBus;
using MadWorldNL.Umiko.Status;
using Microsoft.Extensions.DependencyInjection;

namespace MadWorldNL.Umiko;

public static class FunctionsServiceCollectionExtensions
{
    public static IServiceCollection AddFunctionsServices(this IServiceCollection services)
    {
        services.AddScoped<IQueryHandler<GetDatabaseStatusQuery, GetDatabaseStatusResult>, GetDatabaseStatusFunction>();
        return services;
    }
}