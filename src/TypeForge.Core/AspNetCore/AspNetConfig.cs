using TypeForge.Core.Configuration;

namespace TypeForge.Core.AspNetCore;

public class AspNetConfig
{
    public string Directory { get; set; } = default!;
    public string Output { get; set; } = default!;
    public string? TypeNamePrefix { get; set; }
    public string? TypeNameSuffix { get; set; }
    public string? FileNamePrefix { get; set; }
    public string? FileNameSuffix { get; set; }
    public FolderNameCase FolderNameCase { get; set; } = FolderNameCase.KebabCase;
    public TypeModel TypeModel { get; set; } = TypeModel.Type;
    public TypeNameCase TypeNameCase { get; set; } = TypeNameCase.PascalCase;
    public PropertyNameCase PropertyNameCase { get; set; } = PropertyNameCase.CamelCase;
    public FileNameCase FileNameCase { get; set; } = FileNameCase.KebabCase;
    public NullableType NullableType { get; set; } = NullableType.QuestionMark;
    public bool GenerateIndexFile { get; set; } = true;
    public bool GroupByNamespace { get; set; } = false;
    public bool NameSpaceInOneFile { get; set; } = false;
    public bool EndLinesWithSemicolon { get; set; } = false;
}

public class NameSpaceToMap
{
    public string Name { get; set; } = default!;
    public string Path { get; set; } = default!;
}
