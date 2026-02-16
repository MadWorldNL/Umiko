using System.Threading.RateLimiting;

namespace MadWorldNL.Umiko.Configurations;

public static class RateLimiterExtensions
{
    public static IServiceCollection AddRateLimiterPolicy(this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
            {
                var configuration = context.RequestServices.GetRequiredService<IConfiguration>();
                var permitLimit = configuration.GetValue("RateLimiter:PermitLimit", 100);

                return RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: context.Request.Headers["X-Forwarded-For"].FirstOrDefault()
                        ?? context.Connection.RemoteIpAddress?.ToString()
                        ?? "unknown",
                    factory: _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = permitLimit,
                        Window = TimeSpan.FromMinutes(1),
                        QueueLimit = 0
                    });
            });
        });

        return services;
    }
}
