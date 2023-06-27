using System.Text.Json;
using System.Text.Json.Serialization;
using TypeForge.Core.Utils;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace TypeForge.Core.Configuration;

public class TypeForgeConfig
{
    public string ProjectDir { get; set; } = default!;

    // public CSharpCompilation Compilation { get; set; } = default!;
    // public ClassDeclarationSyntax[] ClassDeclarations { get; set; } = default!;
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
    public ConfigNameSpaceWithPath[] NameSpaces { get; set; } = default!;
}

public class ConfigFile
{
    public string FolderNameCase { get; set; } = "KebabCase";
    public string? TypeNamePrefix { get; set; }
    public string? TypeNameSuffix { get; set; }
    public string? FileNamePrefix { get; set; }
    public string? FileNameSuffix { get; set; }
    public string FileNameCase { get; set; } = "KebabCase";
    public string TypeModel { get; set; } = "Type";
    public string TypeNameCase { get; set; } = "PascalCase";
    public string PropertyNameCase { get; set; } = "CamelCase";
    public string NullableType { get; set; } = "QuestionMark";
    public bool GenerateIndexFile { get; set; }
    public bool GroupByNameSpace { get; set; }
    public bool NameSpaceInOneFile { get; set; }
    public bool EndLinesWithSemicolon { get; set; }
    public ConfigNameSpace[] NameSpaces { get; set; } = default!;
}

public class ConfigNameSpace
{
    public string Name { get; set; } = default!;
    public string Output { get; set; } = default!;
}

public class ConfigNameSpaceWithPath : ConfigNameSpace
{
    public string Path { get; set; } = default!;
}

public enum ConfigFileType
{
    Json,
    Yml,
    None
}
