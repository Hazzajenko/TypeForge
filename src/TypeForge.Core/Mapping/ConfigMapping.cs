using TypeForge.Core.Configuration;
using TypeForge.Core.Configuration.TypeForgeConfig;
using TypeForge.Core.Extensions;
using TypeForge.Core.Models;

namespace TypeForge.Core.Mapping;

public static class ConfigMapping
{
    public static TypeForgeConfig ToTypeForgeConfig(
        this CliConfigFile cliConfigFile,
        string projectDir
    )
    {
        var configNamespaces = cliConfigFile.Directories.Select(x => x.ToConfigNameSpaceWithPath());
        return new TypeForgeConfig
        {
            ProjectDir = projectDir,
            FolderNameCase = cliConfigFile.FolderNameCase.ToFolderNameCase(),
            TypeNamePrefix = cliConfigFile.TypeNamePrefix,
            TypeNameSuffix = cliConfigFile.TypeNameSuffix,
            TypeModel = cliConfigFile.TypeModel.ToExportModelType(),
            TypeNameCase = cliConfigFile.TypeNameCase.ToTypeNameCase(),
            PropertyNameCase = cliConfigFile.PropertyNameCase.ToPropertyNameCase(),
            FileNameCase = cliConfigFile.FileNameCase.ToFileNameCase(),
            NullableType = cliConfigFile.NullableType.ToNullableType(),
            GenerateIndexFile = cliConfigFile.GenerateIndexFile,
            ConfigDirectories = configNamespaces
        };
    }

    public static ConfigDirectoryWithPath ToConfigNameSpaceWithPath(
        this ConfigDirectories configNameSpace
    )
    {
        return new ConfigDirectoryWithPath
        {
            Input = configNameSpace.Input.ConvertSlashesToBackSlashes(),
            Path = configNameSpace.Input.ConvertSlashesToBackSlashes(),
            Depth = configNameSpace.Depth,
            IncludeChildren = configNameSpace.IncludeChildren,
            Flatten = configNameSpace.Flatten,
            KeepRootFolder = configNameSpace.KeepRootFolder,
            Output = configNameSpace.Output.ConvertSlashesToBackSlashes()
        };
    }
}
