using TypeForge.AspNetCore.Configuration;
using TypeForge.Core.Configuration;
using TypeForge.Core.Configuration.TypeForgeConfig;
using TypeForge.Core.Models;

namespace TypeForge.AspNetCore.Mapping;

public static class AspNetCoreConfigMapping
{
    public static TypeForgeConfig ToTypeForgeConfig(
        this AspNetCoreConfigFile configFile,
        string projectDir
    )
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
            // GroupByNamespace = configFile.GroupByNameSpace,
            // NameSpaceInOneFile = configFile.NameSpaceInOneFile,
            ConfigDirectories = configNamespaces
        };
    }

    private static ConfigDirectoryWithPath ToConfigNameSpaceWithPath(
        this AspNetCoreNameSpace configNameSpace,
        string projectDir
    )
    {
        var path = $"{projectDir}{configNameSpace.Name}";
        return new ConfigDirectoryWithPath
        {
            Input = configNameSpace.Name,
            Path = path,
            Output = configNameSpace.Output
        };
    }
}
