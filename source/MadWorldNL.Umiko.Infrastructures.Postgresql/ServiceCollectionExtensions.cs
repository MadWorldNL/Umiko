using MadWorldNL.Umiko.Status;
using Microsoft.Extensions.DependencyInjection;

namespace MadWorldNL.Umiko;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPostgresqlServices(this IServiceCollection services)
    {
        services.AddScoped<IStatusRepository, StatusRepository>();
        return services;
    }
}