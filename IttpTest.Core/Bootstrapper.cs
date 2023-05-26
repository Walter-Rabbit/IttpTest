using Microsoft.Extensions.DependencyInjection;

namespace IttpTest.Core;

public static class Bootstrapper
{
    public static IServiceCollection AddCore(this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        return services;
    }
}