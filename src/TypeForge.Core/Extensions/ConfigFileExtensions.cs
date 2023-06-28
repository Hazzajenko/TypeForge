using System.Text.Json;
using TypeForge.Core.Configuration;
using YamlDotNet.Serialization;

namespace TypeForge.Core.Extensions;

public static class ConfigFileExtensions
{
    public static ConfigFile GetConfigFile(
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

    public static ConfigFileType GetConfigFileTypeIfExist(
        this string projectDir,
        string configName = "forge"
    )
    {
        var configFilePath = Path.Combine(projectDir, $"{configName}.json");
        if (File.Exists(configFilePath))
        {
            return ConfigFileType.Json;
        }
        configFilePath = Path.Combine(projectDir, $"{configName}.yml");
        return File.Exists(configFilePath) ? ConfigFileType.Yml : ConfigFileType.None;
    }

    public static ConfigFile GetConfigFile(this string projectDir, string configName)
    {
        return projectDir.GetConfigFileJson(configName);
    }

    public static ConfigFile GetConfigFile(this string projectDir, ConfigFileType configFileType)
    {
        return configFileType switch
        {
            ConfigFileType.Json => projectDir.GetConfigFileJson(),
            ConfigFileType.Yml => projectDir.GetConfigFileYml(),
            _ => throw new ArgumentOutOfRangeException(nameof(configFileType), configFileType, null)
        };
    }

    private static ConfigFile GetConfigFileYml(
        this string projectDir,
        string configName = "forge.yml"
    )
    {
        IDeserializer deserializer = new DeserializerBuilder().Build();

        var configFilePath = Path.Combine(projectDir, configName);
        var config = deserializer.Deserialize<ConfigFile>(File.ReadAllText(configFilePath));
        if (config is null)
        {
            throw new Exception("Config file is null");
        }
        return config;
    }

    private static ConfigFile GetConfigFileJson(
        this string projectDir,
        string configName = "forge.json"
    )
    {
        var configFilePath = Path.Combine(projectDir, configName);
        var config = JsonSerializer.Deserialize<ConfigFile>(
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
