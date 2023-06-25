using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace DotTsArchitect.Core.Configuration;

public class GlobalConfig
{
    public string ProjectDir { get; set; } = default!;
    public CSharpCompilation Compilation { get; set; } = default!;
    public ClassDeclarationSyntax[] ClassDeclarations { get; set; } = default!;
    public string? TypeNamePrefix { get; set; }
    public string? TypeNameSuffix { get; set; }
    public ExportModelType ExportModelType { get; set; } = ExportModelType.Type;

    public PropertyNameCase PropertyNameCase { get; set; } = PropertyNameCase.CamelCase;

    public FileNameCase FileNameCase { get; set; } = FileNameCase.KebabCase;

    public bool GenerateIndexFile { get; set; } = true;

    public bool GroupByNamespace { get; set; } = false;

    public bool FilePerNamespace { get; set; } = false;
    public ConfigNameSpaceWithPath[] Namespaces { get; set; } = default!;
}

public class ConfigFile
{
    [YamlMember(Alias = "path")]
    [JsonPropertyName("path")]
    public string? TypeNamePrefix { get; set; }

    [YamlMember(Alias = "typeNameSuffix")]
    [JsonPropertyName("typeNameSuffix")]
    public string? TypeNameSuffix { get; set; }

    // ["System.Text.Json.Serialization.JsonConverter(typeof(JsonStringEnumConverter))"]
    [YamlMember(Alias = "exportModelType")]
    [JsonPropertyName("exportModelType")]
    public ExportModelType ExportModelType { get; set; }

    [YamlMember(Alias = "propertyNameCase")]
    [JsonPropertyName("propertyNameCase")]
    public PropertyNameCase PropertyNameCase { get; set; }

    [YamlMember(Alias = "fileNameCase")]
    [JsonPropertyName("fileNameCase")]
    public FileNameCase FileNameCase { get; set; }

    [YamlMember(Alias = "generateIndexFile")]
    [JsonPropertyName("generateIndexFile")]
    public bool GenerateIndexFile { get; set; }

    [YamlMember(Alias = "groupByNamespace")]
    [JsonPropertyName("groupByNamespace")]
    public bool GroupByNamespace { get; set; }

    [YamlMember(Alias = "filePerNamespace")]
    [JsonPropertyName("filePerNamespace")]
    public bool FilePerNamespace { get; set; }

    [YamlMember(Alias = "namespaces")]
    [JsonPropertyName("namespaces")]
    public ConfigNameSpace[] Namespaces { get; set; } = default!;
}

public class ConfigNameSpace
{
    [YamlMember(Alias = "name")]
    [JsonPropertyName("name")]
    public string Name { get; set; } = default!;

    [YamlMember(Alias = "output")]
    [JsonPropertyName("output")]
    public string Output { get; set; } = default!;
}

public class ConfigNameSpaceWithPath : ConfigNameSpace
{
    public string Path { get; set; } = default!;
}

public enum ConfigFileType
{
    Json,
    Yml,
    None
}

public static class ConfigFileExtensions
{
    /*public static ConfigFileType GetConfigFileTypeIfExist(this string projectDir)
    {
        var configFilePath = Path.Combine(projectDir, "dot-ts-architect.yml");
        if (File.Exists(configFilePath))
        {
            return ConfigFileType.Yml;
        }
        configFilePath = Path.Combine(projectDir, "dot-ts-architect.json");
        if (File.Exists(configFilePath))
        {
            return ConfigFileType.Json;
        }
        return ConfigFileType.None;
    }*/

    public static ConfigFileType GetConfigFileTypeIfExist(this string projectDir)
    {
        var configFilePath = Path.Combine(projectDir, "dot-ts-architect.json");
        if (File.Exists(configFilePath))
        {
            return ConfigFileType.Json;
        }
        configFilePath = Path.Combine(projectDir, "dot-ts-architect.yml");
        if (File.Exists(configFilePath))
        {
            return ConfigFileType.Yml;
        }
        return ConfigFileType.None;
    }

    public static ConfigFile GetConfigFile(this string projectDir, ConfigFileType configFileType)
    {
        switch (configFileType)
        {
            case ConfigFileType.Json:
                return projectDir.GetConfigFileJson();
            case ConfigFileType.Yml:
                return projectDir.GetConfigFileYml();
            default:
                throw new ArgumentOutOfRangeException(nameof(configFileType), configFileType, null);
        }
    }

    private static ConfigFile GetConfigFileYml(this string projectDir)
    {
        var deserializer = new DeserializerBuilder()
        // .WithNamingConvention(UnderscoredNamingConvention.Instance)
        .Build();

        var configFilePath = Path.Combine(projectDir, "dot-ts-architect.yml");
        var text = File.ReadAllText(configFilePath);
        text.Log();
        var config = deserializer.Deserialize<ConfigFile>(File.ReadAllText(configFilePath));
        if (config is null)
        {
            throw new Exception("Config file is null");
        }
        return config;
    }

    private static ConfigFile GetConfigFileJson(this string projectDir)
    {
        var configFilePath = Path.Combine(projectDir, "dot-ts-architect.json");
        var text = File.ReadAllText(configFilePath);
        text.Log();
        var config = JsonSerializer.Deserialize<ConfigFile>(File.ReadAllText(configFilePath));
        if (config is null)
        {
            throw new Exception("Config file is null");
        }
        return config;
    }
}
