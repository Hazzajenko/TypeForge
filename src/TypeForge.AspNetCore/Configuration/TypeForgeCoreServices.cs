using Microsoft.Extensions.DependencyInjection;
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
        this IServiceCollection services,
        Action<DotTsArchitectConfig> configure
    )
    {
        // string projectDir =
        string projectDir = DirectoryExtensions.GetProjectDirectory();
        /*services.AddSingleton(_ =>
        {
            var config = new DotTsArchitectConfig();
            configure?.Invoke(config);
            return new TypeForgeService();
        });*/

        // var logger = Log.Logger.ForContext<DotTsArchitectCoreServices>();

        var configFileType = projectDir.GetConfigFileTypeIfExist();
        // logger.Information("Config file type: {ConfigFileType}", configFileType);
        if (configFileType is ConfigFileType.None)
        {
            throw new Exception("Config file not found");
        }

        ConfigFile config = projectDir.GetConfigFile(configFileType);

        if (config.NameSpaces is null)
        {
            throw new Exception("Config file namespaces is null");
        }
        var globalConfig = config.ToGlobalConfig(projectDir);
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
        var globalConfig = config.ToGlobalConfig(projectDir);
        var typeForgeService = new TypeForgeService(globalConfig);
        services.AddSingleton(_ => typeForgeService);

        typeForgeService.WriteFromConfig();

        // globalConfig.Map();

        // services.ConfigureOptions<TypeForgeConfig>();

        // services.Configure<TypeForgeConfig>(options => options.Map(globalConfig));

        // services.Configure<TypeForgeConfig>(options =>
        // {
        //     options = globalConfig;
        // });

        // var uiLogger = Log.Logger.ForContext<TypeForgeUiService>();
        var typeForgeUiService = new TypeForgeUiService(globalConfig);
        // services.AddSingleton(_ => typeForgeUiService);
        // typeForgeUiService.ReadHtml();
        // services.AddRazorPages();

        // services.AddSingleton<TypeForgeConfig>();
        services.AddSingleton(typeForgeUiService);
        // services.AddSingleton(_ => new TypeForgeUiService(globalConfig));
        // var serviceProvider = services.BuildServiceProvider();
        // var typeForgeUiService = serviceProvider.GetService<TypeForgeUiService>();
        // ArgumentNullException.ThrowIfNull(typeForgeUiService);
        typeForgeUiService.WriteFromConfig();
        // typeForgeUiService.ReadHtml();

        // services.



        return services;
    }

    /*private void HandleUseConfig()
    {
        string projectDir = GetProjectDirectory();
        var configFileType = projectDir.GetConfigFileTypeIfExist();
        Log.Logger.Information("Config file type: {ConfigFileType}", configFileType);
        if (configFileType is ConfigFileType.None)
        {
            throw new Exception("Config file not found");
        }

        ConfigFile config = projectDir.GetConfigFile(configFileType);

        if (config.NameSpaces is null)
        {
            throw new Exception("Config file namespaces is null");
        }
        var globalConfig = config.ToGlobalConfig(projectDir);
        var writer = new WriterService(globalConfig);
        writer.WriteFromConfig();
    }*/
}
