using System.Text.Json.Serialization;

using Microsoft.AspNetCore.Mvc;

namespace Assignment.Api.Startup;

public static class WebExtensions
{
public const string CorsPolicyName = "AssignmentCors";

    public static IServiceCollection ConfigureWeb(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddOptions<AuthOptions>()
            .Bind(configuration.GetSection(AuthOptions.SectionName))
            .Validate(options => !string.IsNullOrWhiteSpace(options.Username), "Auth.Username is required")
            .Validate(options => !string.IsNullOrWhiteSpace(options.Password), "Auth.Password is required")
            .ValidateOnStart();

        services.AddProblemDetails(options =>
        {
            options.CustomizeProblemDetails = context =>
            {
                var traceId = context.HttpContext.TraceIdentifier;
                if (!string.IsNullOrEmpty(traceId))
                {
                    context.ProblemDetails.Extensions["traceId"] = traceId;
                }
            };
        });

        services
            .AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            });

        var allowedOrigins = configuration
            .GetRequiredSection("Cors:AllowedOrigins")
            .Get<string[]>();

        if (allowedOrigins is null || allowedOrigins.Length == 0)
        {
            throw new InvalidOperationException("Cors:AllowedOrigins must be configured with at least one origin.");
        }

        services.AddCors(options =>
        {
            options.AddPolicy(
                CorsPolicyName,
                policy =>
                {
                    policy.WithOrigins(allowedOrigins)
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                }
            );
        });

        return services;
    }
}
