using System.Text.Json;
using System.Text.Json.Serialization;
using DotTsArchitect.Core.Utils;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace DotTsArchitect.Core.Configuration;

public class GlobalConfig
{
    public string ProjectDir { get; set; } = default!;
    public CSharpCompilation Compilation { get; set; } = default!;
    public ClassDeclarationSyntax[] ClassDeclarations { get; set; } = default!;
    public FolderNameCase FolderNameCase { get; set; } = FolderNameCase.KebabCase;
    public string? TypeNamePrefix { get; set; }
    public string? TypeNameSuffix { get; set; }
    public string? FileNamePrefix { get; set; }
    public string? FileNameSuffix { get; set; }
    public ExportModelType ExportModelType { get; set; } = ExportModelType.Type;

    public PropertyNameCase PropertyNameCase { get; set; } = PropertyNameCase.CamelCase;

    public FileNameCase FileNameCase { get; set; } = FileNameCase.KebabCase;

    public bool GenerateIndexFile { get; set; } = true;

    public bool GroupByNamespace { get; set; } = false;

    public bool FilePerNamespace { get; set; } = false;
    public bool EndLinesWithSemicolon { get; set; } = false;
    public ConfigNameSpaceWithPath[] Namespaces { get; set; } = default!;
}

public class ConfigFile
{
    public string FolderNameCase { get; set; } = "KebabCase";
    public string? TypeNamePrefix { get; set; }
    public string? TypeNameSuffix { get; set; }
    public string? FileNamePrefix { get; set; }
    public string? FileNameSuffix { get; set; }
    public string FileNameCase { get; set; } = "KebabCase";
    public string ExportModelType { get; set; } = "Type";
    public string PropertyNameCase { get; set; } = "CamelCase";
    public bool GenerateIndexFile { get; set; }
    public bool GroupByNamespace { get; set; }
    public bool FilePerNamespace { get; set; }
    public bool EndLinesWithSemicolon { get; set; }
    public ConfigNameSpace[] Namespaces { get; set; } = default!;
}

public class ConfigNameSpace
{
    // [YamlMember(Alias = "name")]
    // [JsonPropertyName("name")]
    public string Name { get; set; } = default!;

    // [YamlMember(Alias = "output")]
    // [JsonPropertyName("output")]
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
