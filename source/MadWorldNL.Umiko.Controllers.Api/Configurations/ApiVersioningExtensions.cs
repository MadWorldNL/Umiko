using Asp.Versioning;
using Asp.Versioning.Builder;

namespace MadWorldNL.Umiko.Configurations;

internal static class ApiVersioningExtensions
{
    internal static ApiVersionSet GetDefaultApiVersionSet(this IEndpointRouteBuilder app)
    {
        return app.NewApiVersionSet()
            .HasApiVersion(new ApiVersion(1, 0))
            .Build();
    }
}