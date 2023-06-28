﻿using TypeForge.Core.Configuration;
using TypeForge.Core.Configuration.TypeForgeConfig;
using TypeForge.Core.Models;

namespace TypeForge.Cli.Mapping;

public static class InputMapping
{
    public static TypeForgeConfig ToTypeForgeConfig(this TypeForgeOptions input)
    {
        return new TypeForgeConfig
        {
            NameSpaces = new List<ConfigNameSpaceWithPath>
            {
                new()
                {
                    Name = input.Directory!,
                    Output = input.Output!,
                    Path = input.Directory!
                }
            },
            FolderNameCase = input.FolderNameCase.ToFolderNameCase(),
            TypeNamePrefix = input.TypeNamePrefix,
            TypeNameSuffix = input.TypeNameSuffix,
            TypeModel = input.TypeModel.ToExportModelType(),
            TypeNameCase = input.TypeNameCase.ToTypeNameCase(),
            PropertyNameCase = input.PropertyNameCase.ToPropertyNameCase(),
            FileNameCase = input.FileNameCase.ToFileNameCase(),
            NullableType = input.NullableType.ToNullableType(),
            GenerateIndexFile = input.GenerateIndexFile,
            GroupByNamespace = input.GroupByNameSpace,
            NameSpaceInOneFile = input.NameSpaceInOneFile,
        };
    }
}
