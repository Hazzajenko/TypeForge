using TypeForge.Core.Models;

namespace TypeForge.Core.Configuration.TypeForgeConfig;

public class TypeForgeConfig
{
    public string ProjectDir { get; set; } = default!;
    public FolderNameCase FolderNameCase { get; set; } = FolderNameCase.KebabCase;
    public string? TypeNamePrefix { get; set; }
    public string? TypeNameSuffix { get; set; }
    public string? FileNamePrefix { get; set; }
    public string? FileNameSuffix { get; set; }
    public TypeModel TypeModel { get; set; } = TypeModel.Type;
    public TypeNameCase TypeNameCase { get; set; } = TypeNameCase.PascalCase;

    public PropertyNameCase PropertyNameCase { get; set; } = PropertyNameCase.CamelCase;

    public FileNameCase FileNameCase { get; set; } = FileNameCase.KebabCase;
    public NullableType NullableType { get; set; } = NullableType.QuestionMark;

    public bool GenerateIndexFile { get; set; } = true;

    public bool GroupByNamespace { get; set; } = false;

    public bool NameSpaceInOneFile { get; set; } = false;
    public bool EndLinesWithSemicolon { get; set; } = false;
    public IEnumerable<ConfigNameSpaceWithPath> NameSpaces { get; set; } = default!;
}
