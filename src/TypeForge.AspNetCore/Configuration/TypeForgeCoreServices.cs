using Microsoft.Extensions.DependencyInjection;
using TypeForge.AspNetCore.Services;
using TypeForge.Core.Configuration;
using TypeForge.Core.Extensions;
using TypeForge.Core.Services;

namespace TypeForge.AspNetCore.Configuration;

public static class TypeForgeCoreServices
{
    public static IServiceCollection AddDotTsArchitectCore(
        this IServiceCollection services,
        Action<DotTsArchitectConfig> configure
    )
    {
        // string projectDir =
        string projectDir = DirectoryExtensions.GetProjectDirectory();
        services.AddSingleton(_ =>
        {
            var config = new DotTsArchitectConfig();
            configure?.Invoke(config);
            return new TypeForgeService(config);
        });

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
}
