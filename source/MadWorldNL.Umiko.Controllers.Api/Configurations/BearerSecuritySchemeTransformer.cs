using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace MadWorldNL.Umiko.Configurations;

public sealed class BearerSecuritySchemeTransformer : IOpenApiDocumentTransformer
{
    public Task TransformAsync(OpenApiDocument document,
        OpenApiDocumentTransformerContext context,
        CancellationToken cancellationToken)
    {
        // 1. Ensure Components exists to hold the security scheme
        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
        // 2. Add the 'Bearer' security scheme definition
        document.Components.SecuritySchemes.Add(
            JwtBearerDefaults.AuthenticationScheme, // "Bearer" is the default scheme name
            new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Scheme = JwtBearerDefaults.AuthenticationScheme,
                BearerFormat = "JWT",
                Description = "Enter your JWT Bearer token *only* in the text box below. (Example: 'eyJhbGciOiJIUzI1Ni...')"
            }
        );

        // 3. Add a global security requirement: every operation will show the lock icon
        // This is what makes it "global" in the UI.
        document.Security ??= [];
        document.Security.Add(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecuritySchemeReference(JwtBearerDefaults.AuthenticationScheme),
                // Pass an empty list of scopes if you're not using OAuth2 scopes
                []
            }
        });

        return Task.CompletedTask;
    }
}