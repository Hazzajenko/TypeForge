using Ardalis.SmartEnum;

namespace DotTsArchitect.Core.Configuration;

public class DotTsArchitectConfig
{
    public DotTsArchitectConfig(
        string path,
        ExportModelType exportModelType = ExportModelType.Type,
        PropertyNameCase propertyNameCase = PropertyNameCase.CamelCase,
        FileNameCase fileNameCase = FileNameCase.KebabCase,
        bool generateIndexFile = true,
        bool groupByNamespace = false,
        bool filePerNamespace = false
    )
    {
        Path = path;
        ExportModelType = exportModelType;
        PropertyNameCase = propertyNameCase;
        FileNameCase = fileNameCase;
        GenerateIndexFile = generateIndexFile;
        GroupByNamespace = groupByNamespace;
        FilePerNamespace = filePerNamespace;
    }

    public DotTsArchitectConfig()
    {
        Path = "src";
        ExportModelType = ExportModelType.Type;
        PropertyNameCase = PropertyNameCase.CamelCase;
        FileNameCase = FileNameCase.KebabCase;
    }

    public string Path { get; set; }
    public string? TypeNamePrefix { get; set; }
    public string? TypeNameSuffix { get; set; }
    public ExportModelType ExportModelType { get; set; }
    public PropertyNameCase PropertyNameCase { get; set; }
    public FileNameCase FileNameCase { get; set; }
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

public enum ExportModelType
{
    Type,
    Interface
}

public static class ExportModelTypeExtensions
{
    public static ExportModelType ToExportModelType(this string exportModelType)
    {
        return exportModelType switch
        {
            "Type" => ExportModelType.Type,
            "type" => ExportModelType.Type,
            "Interface" => ExportModelType.Interface,
            "interface" => ExportModelType.Interface,
            _
                => throw new ArgumentOutOfRangeException(
                    nameof(exportModelType),
                    exportModelType,
                    null
                )
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



/*public sealed class ExportModelType : SmartEnum<ExportModelType>
{
    public static readonly ExportModelType Type = new(1, "Type");
    public static readonly ExportModelType Interface = new(2, "Interface");

    private ExportModelType(int id, string name) : base(name, id)
    {
    }
}*/
