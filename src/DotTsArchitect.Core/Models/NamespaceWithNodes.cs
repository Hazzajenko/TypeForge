using DotTsArchitect.Core.Configuration;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DotTsArchitect.Core.Models;

public class NamespaceWithNodes
{
    public ConfigNameSpaceWithPath Namespace { get; set; } = default!;
    public IEnumerable<ClassDeclarationSyntax> Nodes { get; set; } = default!;
}

public class NamespaceWithNodesAndFileName
{
    public ConfigNameSpaceWithPath Namespace { get; set; } = default!;
    public string FileName { get; set; } = default!;
    public string PathFromParentNamespace { get; set; } = default!;
    public IEnumerable<ClassDeclarationSyntax> Nodes { get; set; } = default!;
}

public class TypeScriptFolder
{
    public string FolderName { get; set; } = default!;

    // public string PathFromParentNamespace { get; set; } = default!;
    public IEnumerable<TypeScriptFile> Files { get; set; } = default!;
}

public class TypeScriptFile
{
    public string FileName { get; set; } = default!;
    public string PathFromParentNamespace { get; set; } = default!;
    public IEnumerable<TypeScriptType> Types { get; set; } = default!;
}

public class TypeScriptType
{
    public string Name { get; set; } = default!;
    public IEnumerable<TypeProperty> Properties { get; set; } = default!;
}

public class TypeProperty
{
    public string Name { get; set; } = default!;
    public string Type { get; set; } = default!;
}
