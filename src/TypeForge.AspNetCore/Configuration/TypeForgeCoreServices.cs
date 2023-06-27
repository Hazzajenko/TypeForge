using System.Reflection;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Serilog;
using TypeForge.AspNetCore.Services;
using TypeForge.Core.Configuration;
using TypeForge.Core.Extensions;
using TypeForge.Core.Mapping;
using TypeForge.Core.Services;

namespace TypeForge.AspNetCore.Configuration;

public static class TypeForgeCoreServices
{
    public static IServiceCollection AddTypeForge(
        this IServiceCollection services
    // Action<DotTsArchitectConfig> configure
    )
    {
        string projectDir = DirectoryExtensions.GetProjectDirectory();
        var configFileType = projectDir.GetConfigFileTypeIfExist();
        if (configFileType is ConfigFileType.None)
        {
            throw new Exception("Config file not found");
        }

        ConfigFile config = projectDir.GetConfigFile(configFileType);

        if (config.NameSpaces is null)
        {
            throw new Exception("Config file namespaces is null");
        }
        var globalConfig = config.ToTypeForgeConfig(projectDir);
        var writer = new WriterService(globalConfig);
        writer.WriteFromConfig();

        return services;
    }

    public static IServiceCollection AddTypeForgeConfigFile(
        this IServiceCollection services,
        string configName = "forge.json"
    )
    {
        string projectDir = DirectoryExtensions.GetProjectDirectory();
        ConfigFile config = projectDir.GetConfigFile(configName);

        if (config.NameSpaces is null)
        {
            throw new Exception("Config file namespaces is null");
        }
        var globalConfig = config.ToTypeForgeConfig(projectDir);
        var typeForgeService = new TypeForgeService(globalConfig);
        services.AddSingleton(_ => typeForgeService);

        typeForgeService.WriteFromConfig();
        var typeForgeUiService = new TypeForgeUiService(globalConfig);
        services.AddSingleton(typeForgeUiService);
        typeForgeUiService.WriteFromConfig();
        return services;
    }
}
