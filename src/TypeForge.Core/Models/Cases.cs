namespace TypeForge.Core.Models;

public enum FolderNameCase
{
    KebabCase,
    CamelCase,
    PascalCase,
}

public static class FolderNameCaseExtensions
{
    public static FolderNameCase ToFolderNameCase(this string folderNameCase)
    {
        return folderNameCase switch
        {
            "KebabCase" => FolderNameCase.KebabCase,
            "kebab-case" => FolderNameCase.KebabCase,
            "kebabCase" => FolderNameCase.KebabCase,
            "CamelCase" => FolderNameCase.CamelCase,
            "camelCase" => FolderNameCase.CamelCase,
            "PascalCase" => FolderNameCase.PascalCase,
            "pascalCase" => FolderNameCase.PascalCase,
            _ => throw new ArgumentOutOfRangeException(nameof(folderNameCase), folderNameCase, null)
        };
    }
}

public enum TypeModel
{
    Type,
    Interface
}

public static class ExportModelTypeExtensions
{
    public static TypeModel ToExportModelType(this string exportModelType)
    {
        return exportModelType switch
        {
            "Type" => TypeModel.Type,
            "type" => TypeModel.Type,
            "Interface" => TypeModel.Interface,
            "interface" => TypeModel.Interface,
            _
                => throw new ArgumentOutOfRangeException(
                    nameof(exportModelType),
                    exportModelType,
                    null
                )
        };
    }
}

public enum TypeNameCase
{
    CamelCase,
    PascalCase,
}

public static class TypeNameCaseExtensions
{
    public static TypeNameCase ToTypeNameCase(this string typeNameCase)
    {
        return typeNameCase switch
        {
            "CamelCase" => TypeNameCase.CamelCase,
            "camelCase" => TypeNameCase.CamelCase,
            "PascalCase" => TypeNameCase.PascalCase,
            "pascalCase" => TypeNameCase.PascalCase,
            _ => throw new ArgumentOutOfRangeException(nameof(typeNameCase), typeNameCase, null)
        };
    }
}

public enum PropertyNameCase
{
    CamelCase,
    PascalCase,
}

public static class PropertyNameCaseExtensions
{
    public static PropertyNameCase ToPropertyNameCase(this string propertyNameCase)
    {
        return propertyNameCase switch
        {
            "CamelCase" => PropertyNameCase.CamelCase,
            "camelCase" => PropertyNameCase.CamelCase,
            "PascalCase" => PropertyNameCase.PascalCase,
            "pascalCase" => PropertyNameCase.PascalCase,
            _
                => throw new ArgumentOutOfRangeException(
                    nameof(propertyNameCase),
                    propertyNameCase,
                    null
                )
        };
    }
}

public enum FileNameCase
{
    KebabCase,
    CamelCase,
    PascalCase,
}

public static class FileNameCaseExtensions
{
    public static FileNameCase ToFileNameCase(this string fileNameCase)
    {
        return fileNameCase switch
        {
            "KebabCase" => FileNameCase.KebabCase,
            "kebab-case" => FileNameCase.KebabCase,
            "kebabCase" => FileNameCase.KebabCase,
            "CamelCase" => FileNameCase.CamelCase,
            "camelCase" => FileNameCase.CamelCase,
            "PascalCase" => FileNameCase.PascalCase,
            "pascalCase" => FileNameCase.PascalCase,
            _ => throw new ArgumentOutOfRangeException(nameof(fileNameCase), fileNameCase, null)
        };
    }
}

public enum NullableType
{
    None,
    QuestionMark,
    Null,
    Undefined
}

public static class NullableTypeExtensions
{
    public static NullableType ToNullableType(this string nullableType)
    {
        return nullableType switch
        {
            "None" => NullableType.None,
            "none" => NullableType.None,
            "QuestionMark" => NullableType.QuestionMark,
            "questionMark" => NullableType.QuestionMark,
            "?" => NullableType.QuestionMark,
            "Null" => NullableType.Null,
            "null" => NullableType.Null,
            "Undefined" => NullableType.Undefined,
            "undefined" => NullableType.Undefined,
            _ => throw new ArgumentOutOfRangeException(nameof(nullableType), nullableType, null)
        };
    }
}
