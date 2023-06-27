using System.Text.Json;
using TypeForge.Core.Utils;
using YamlDotNet.Serialization;

namespace TypeForge.Core.Configuration;

public static class ConfigFileExtensions
{
    public static ConfigFile GetConfigFile(
        this string projectDir,
        ConfigFileType configFileType,
        string configFilePath
    )
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
        if (File.Exists(configFilePath))
        {
            return ConfigFileType.Yml;
        }
        return ConfigFileType.None;
    }

    public static ConfigFile GetConfigFile(this string projectDir, string configName)
    {
        return projectDir.GetConfigFileJson(configName);
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

    private static ConfigFile GetConfigFileYml(
        this string projectDir,
        string configName = "forge.yml"
    )
    {
        var deserializer = new DeserializerBuilder().Build();

        var configFilePath = Path.Combine(projectDir, configName);
        // var text = File.ReadAllText(configFilePath);
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
        // var text = File.ReadAllText(configFilePath);
        var config = JsonSerializer.Deserialize<ConfigFile>(File.ReadAllText(configFilePath));
        if (config is null)
        {
            throw new Exception("Config file is null");
        }
        return config;
    }

    public static TypeForgeConfig ToGlobalConfig(this ConfigFile configFile, string projectDir)
    {
        var configNamespaces = configFile.NameSpaces
            .Select(x =>
            {
                var path = $"{projectDir}{x.Name}";
                return new ConfigNameSpaceWithPath
                {
                    Name = x.Name,
                    Path = path,
                    Output = x.Output
                };
            })
            .ToArray();
        var syntaxTrees = configNamespaces.GetSyntaxTrees();
        var compilation = syntaxTrees.CreateCompilation();
        var classDeclarations = configNamespaces.GetClassDeclarations();
        return new TypeForgeConfig
        {
            ProjectDir = projectDir,
            // Compilation = compilation,
            // ClassDeclarations = classDeclarations,
            FolderNameCase = configFile.FolderNameCase.ToFolderNameCase(),
            TypeNamePrefix = configFile.TypeNamePrefix,
            TypeNameSuffix = configFile.TypeNameSuffix,
            TypeModel = configFile.TypeModel.ToExportModelType(),
            TypeNameCase = configFile.TypeNameCase.ToTypeNameCase(),
            PropertyNameCase = configFile.PropertyNameCase.ToPropertyNameCase(),
            FileNameCase = configFile.FileNameCase.ToFileNameCase(),
            NullableType = configFile.NullableType.ToNullableType(),
            GenerateIndexFile = configFile.GenerateIndexFile,
            GroupByNamespace = configFile.GroupByNameSpace,
            NameSpaceInOneFile = configFile.NameSpaceInOneFile,
            NameSpaces = configNamespaces
        };
    }
}
