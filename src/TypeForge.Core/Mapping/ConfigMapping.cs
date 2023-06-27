using TypeForge.Core.Configuration;

namespace TypeForge.Core.Mapping;

public static class ConfigMapping
{
    public static TypeForgeConfig Map(this TypeForgeConfig configFile, TypeForgeConfig configToMap)
    {
        return configToMap;
    }

    public static TypeForgeConfig Map(this ConfigFile configFile)
    {
        return new TypeForgeConfig
        {
            FolderNameCase = Enum.Parse<FolderNameCase>(configFile.FolderNameCase),
            TypeNamePrefix = configFile.TypeNamePrefix,
            TypeNameSuffix = configFile.TypeNameSuffix,
            FileNamePrefix = configFile.FileNamePrefix,
            FileNameSuffix = configFile.FileNameSuffix,
            FileNameCase = Enum.Parse<FileNameCase>(configFile.FileNameCase),
            TypeModel = Enum.Parse<TypeModel>(configFile.TypeModel),
            TypeNameCase = Enum.Parse<TypeNameCase>(configFile.TypeNameCase),
            PropertyNameCase = Enum.Parse<PropertyNameCase>(configFile.PropertyNameCase),
            NullableType = Enum.Parse<NullableType>(configFile.NullableType),
            GenerateIndexFile = configFile.GenerateIndexFile,
            GroupByNamespace = configFile.GroupByNameSpace,
            NameSpaceInOneFile = configFile.NameSpaceInOneFile,
            EndLinesWithSemicolon = configFile.EndLinesWithSemicolon,
            NameSpaces = configFile.NameSpaces
                .Select(x => new ConfigNameSpaceWithPath { Name = x.Name, Output = x.Output })
                .ToArray()
        };
    }
}
