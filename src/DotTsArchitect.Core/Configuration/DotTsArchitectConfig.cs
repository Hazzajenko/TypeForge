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

public enum ExportModelType
{
    Type,
    Interface
}

public enum PropertyNameCase
{
    CamelCase,
    PascalCase,
}

public enum FileNameCase
{
    KebabCase,
    CamelCase,
    PascalCase,
}
