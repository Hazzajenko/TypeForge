using DotTsArchitect.AspNetCore.Services;
using DotTsArchitect.Core.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DotTsArchitect.AspNetCore.Configuration;

public static class DotTsArchitectCoreServices
{
    public static IServiceCollection AddDotTsArchitectCore(
        this IServiceCollection services,
        Action<DotTsArchitectConfig> configure
    )
    {
        services.AddSingleton(_ =>
        {
            var config = new DotTsArchitectConfig();
            configure?.Invoke(config);
            return new DotTsArchitectService(config);
        });
        return services;
    }
}
