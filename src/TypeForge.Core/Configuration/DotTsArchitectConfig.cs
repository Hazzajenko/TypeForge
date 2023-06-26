using Ardalis.SmartEnum;

namespace TypeForge.Core.Configuration;

public class DotTsArchitectConfig
{
    public DotTsArchitectConfig(
        string path,
        TypeModel typeModel = TypeModel.Type,
        PropertyNameCase propertyNameCase = PropertyNameCase.CamelCase,
        FileNameCase fileNameCase = FileNameCase.KebabCase,
        bool generateIndexFile = true,
        bool groupByNamespace = false,
        bool filePerNamespace = false
    )
    {
        Path = path;
        TypeModel = typeModel;
        PropertyNameCase = propertyNameCase;
        FileNameCase = fileNameCase;
        GenerateIndexFile = generateIndexFile;
        GroupByNamespace = groupByNamespace;
        FilePerNamespace = filePerNamespace;
    }

    public DotTsArchitectConfig()
    {
        Path = "src";
        TypeModel = TypeModel.Type;
        PropertyNameCase = PropertyNameCase.CamelCase;
        FileNameCase = FileNameCase.KebabCase;
    }

    public string Path { get; set; }
    public string? TypeNamePrefix { get; set; }
    public string? TypeNameSuffix { get; set; }
    public TypeModel TypeModel { get; set; }
    public PropertyNameCase PropertyNameCase { get; set; }
    public FileNameCase FileNameCase { get; set; }
    public NullableType NullableType { get; set; }
    public bool GenerateIndexFile { get; set; }
    public bool GroupByNamespace { get; set; }
    public bool FilePerNamespace { get; set; }
}

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


/*public sealed class ExportModelType : SmartEnum<ExportModelType>
{
    public static readonly ExportModelType Type = new(1, "Type");
    public static readonly ExportModelType Interface = new(2, "Interface");

    private ExportModelType(int id, string name) : base(name, id)
    {
    }
}*/
