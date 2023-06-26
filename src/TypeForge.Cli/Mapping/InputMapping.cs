using TypeForge.Core.Configuration;
using TypeForge.Core.StartUp;

namespace TypeForge.Cli.Mapping;

public static class InputMapping
{
    public static InputGlobalConfig ToInputGlobalConfig(this TypeForgeOptions input)
    {
        return new InputGlobalConfig
        {
            Directory = input.Directory,
            Output = input.Output,
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
