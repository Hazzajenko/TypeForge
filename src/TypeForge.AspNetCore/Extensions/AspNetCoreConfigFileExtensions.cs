using System.Text.Json;
using TypeForge.AspNetCore.Configuration;
using TypeForge.Core.Configuration;
using YamlDotNet.Serialization;
using ConfigFileType = TypeForge.Core.Configuration.ConfigFileType;

namespace TypeForge.AspNetCore.Extensions;

public static class AspNetCoreConfigFileExtensions
{
    public static AspNetCoreConfigFile GetConfigFile(
        this string projectDir,
        ConfigFileType configFileType,
        string configFilePath
    )
    {
        return configFileType switch
        {
            ConfigFileType.Json => projectDir.GetConfigFileJson(),
            ConfigFileType.Yml => projectDir.GetConfigFileYml(),
            ConfigFileType.None
                => throw new ArgumentException($"Config file {configFilePath} does not exist"),
            _ => throw new ArgumentOutOfRangeException(nameof(configFileType), configFileType, null)
        };
    }

    public static AspNetCoreConfigFile GetAspNetCoreConfigFile(
        this string projectDir,
        string configName
    )
    {
        return projectDir.GetConfigFileJson(configName);
    }

    public static AspNetCoreConfigFile GetAspNetCoreConfigFile(
        this string projectDir,
        ConfigFileType configFileType
    )
    {
        return configFileType switch
        {
            ConfigFileType.Json => projectDir.GetConfigFileJson(),
            ConfigFileType.Yml => projectDir.GetConfigFileYml(),
            _ => throw new ArgumentOutOfRangeException(nameof(configFileType), configFileType, null)
        };
    }

    private static AspNetCoreConfigFile GetConfigFileYml(
        this string projectDir,
        string configName = "forge.yml"
    )
    {
        IDeserializer deserializer = new DeserializerBuilder().Build();

        var configFilePath = Path.Combine(projectDir, configName);
        var config = deserializer.Deserialize<AspNetCoreConfigFile>(
            File.ReadAllText(configFilePath)
        );
        if (config is null)
        {
            throw new Exception("Config file is null");
        }
        return config;
    }

    private static AspNetCoreConfigFile GetConfigFileJson(
        this string projectDir,
        string configName = "forge.json"
    )
    {
        var configFilePath = Path.Combine(projectDir, configName);
        var config = JsonSerializer.Deserialize<AspNetCoreConfigFile>(
            File.ReadAllText(configFilePath),
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        );
        if (config is null)
        {
            throw new Exception("Config file is null");
        }
        return config;
    }
}
