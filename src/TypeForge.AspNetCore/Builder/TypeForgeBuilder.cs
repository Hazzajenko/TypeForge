using Microsoft.Extensions.DependencyInjection;
using TypeForge.AspNetCore.Configuration;
using TypeForge.Core.Configuration.TypeForgeConfig;

namespace TypeForge.AspNetCore.Builder;

public static class TypeForgeBuilderExtensions
{
    public static IServiceCollection AddTypeForge(
        this IServiceCollection services,
        Action<TypeForgeConfig> configure
    )
    {
        var typeForge = new TypeForgeBuilder();
        typeForge.UseConfig("typeforge.json").ConfigureTypeForgeSettings(configure).Build(services);
        return services;
    }
}

public class TypeForgeBuilder
{
    public TypeForgeBuilder() { }

    public TypeForgeAppSettingsBuilder UseAppSettings(string appSettingsPath)
    {
        return new TypeForgeAppSettingsBuilder(appSettingsPath);
    }

    public TypeForgeConfigBuilder UseConfig(string configPath)
    {
        return new TypeForgeConfigBuilder(configPath);
    }
}

public class TypeForgeAppSettingsBuilder
{
    private string _appSettingsPath;

    public TypeForgeAppSettingsBuilder(string appSettingsPath)
    {
        _appSettingsPath = appSettingsPath;
    }

    public TypeForgeAppSettingsBuilder ConfigureTypeForgeSettings(Action<TypeForgeConfig> configure)
    {
        return this;
    }

    public IServiceCollection Build(IServiceCollection services)
    {
        return services;
    }
}

public class TypeForgeConfigBuilder
{
    private readonly string _configPath;

    public TypeForgeConfigBuilder(string configPath)
    {
        _configPath = configPath;
    }

    public TypeForgeConfigBuilder ConfigureTypeForgeSettings(Action<TypeForgeConfig> configure)
    {
        return this;
    }

    public IServiceCollection Build(IServiceCollection services)
    {
        services.AddTypeForgeConfigFile(_configPath);
        return services;
    }
}
