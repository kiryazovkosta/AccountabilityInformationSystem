using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace AccountabilityInformationSystem.Api.Shared.Extensions;

public sealed class BearerSecuritySchemeTransformer : IOpenApiDocumentTransformer
{
    private readonly IAuthenticationSchemeProvider _authenticationSchemeProvider;

    public BearerSecuritySchemeTransformer(IAuthenticationSchemeProvider authenticationSchemeProvider)
    {
        _authenticationSchemeProvider = authenticationSchemeProvider;
    }

    public async Task TransformAsync(
        OpenApiDocument document,
        OpenApiDocumentTransformerContext context,
        CancellationToken cancellationToken)
    {
        IEnumerable<AuthenticationScheme> authenticationSchemes = await _authenticationSchemeProvider.GetAllSchemesAsync();
        if (authenticationSchemes.All(a => a.Name != "Bearer"))
        {
            return;
        }

        OpenApiSecurityScheme bearerScheme = new()
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "JWT Authorization header using the Bearer scheme."
        };

        document.Components ??= new OpenApiComponents();

        document.AddComponent("Bearer", bearerScheme);

        OpenApiSecurityRequirement securityRequirement = new()
        {
            [new OpenApiSecuritySchemeReference("Bearer")] = []
        };

        foreach (IOpenApiPathItem path in document.Paths.Values)
        {
            foreach (OpenApiOperation operation in path.Operations.Values)
            {
                operation.Security ??= (List<OpenApiSecurityRequirement>)[];
                operation.Security.Add(securityRequirement);
            }
        }
    }
}
