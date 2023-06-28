using TypeForge.Core.Configuration;
using TypeForge.Core.Configuration.TypeForgeConfig;
using TypeForge.Core.Extensions;
using TypeForge.Core.Models;

namespace TypeForge.Core.Mapping;

public static class ConfigMapping
{
    public static TypeForgeConfig ToTypeForgeConfig(this ConfigFile configFile, string projectDir)
    {
        var configNamespaces = configFile.NameSpaces.Select(
            x => x.ToConfigNameSpaceWithPath(projectDir)
        );
        return new TypeForgeConfig
        {
            ProjectDir = projectDir,
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

    private static ConfigNameSpaceWithPath ToConfigNameSpaceWithPath(
        this ConfigNameSpace configNameSpace,
        string projectDir
    )
    {
        var path = $"{projectDir}{configNameSpace.Name}";
        return new ConfigNameSpaceWithPath
        {
            Name = configNameSpace.Name,
            Path = path,
            Output = configNameSpace.Output
        };
    }
}
