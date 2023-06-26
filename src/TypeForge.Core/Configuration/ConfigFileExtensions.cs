using System.Text.Json;
using TypeForge.Core.Utils;
using YamlDotNet.Serialization;

namespace TypeForge.Core.Configuration;

public static class ConfigFileExtensions
{
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
        var deserializer = new DeserializerBuilder().Build();

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

    public static GlobalConfig ToGlobalConfig(this ConfigFile configFile, string projectDir)
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
        return new GlobalConfig
        {
            ProjectDir = projectDir,
            Compilation = compilation,
            ClassDeclarations = classDeclarations,
            FolderNameCase = configFile.FolderNameCase.ToFolderNameCase(),
            TypeNamePrefix = configFile.TypeNamePrefix,
            TypeNameSuffix = configFile.TypeNameSuffix,
            ExportModelType = configFile.ExportModelType.ToExportModelType(),
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
