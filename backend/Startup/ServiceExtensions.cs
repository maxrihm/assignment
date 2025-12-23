using Assignment.Api.Interfaces;
using Assignment.Api.Services;

namespace Assignment.Api.Startup;

public static class ServiceExtensions
{
    public static IServiceCollection RegisterServices(
        this IServiceCollection services)
    {
        services.AddSingleton<IAuthService, AuthService>();
        services.AddSingleton<ITokenService, TokenService>();
        services.AddSingleton<IAuthValidator, AuthValidator>();

        return services;
    }
}
