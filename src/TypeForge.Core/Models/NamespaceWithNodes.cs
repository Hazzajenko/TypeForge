using Microsoft.CodeAnalysis.CSharp.Syntax;
using TypeForge.Core.Configuration;

namespace TypeForge.Core.Models;

public class NamespaceWithNodes
{
    public ConfigNameSpaceWithPath NameSpace { get; set; } = default!;
    public IEnumerable<ClassDeclarationSyntax> Nodes { get; set; } = default!;
}

public class NameSpaceWithNodesAndFileName
{
    public ConfigNameSpaceWithPath NameSpace { get; set; } = default!;
    public string FileName { get; set; } = default!;
    public string PathFromParentNamespace { get; set; } = default!;
    public IEnumerable<ClassDeclarationSyntax> Nodes { get; set; } = default!;
}

public class TypeScriptFileConfig
{
    public string NameSpace { get; set; } = default!;
    public TypeScriptFile File { get; set; } = default!;
}

public class TypeScriptFolder
{
    public string FolderName { get; set; } = default!;
    public IEnumerable<TypeScriptFile> Files { get; set; } = default!;
    // public List<TypeScriptFolder> ChildFolders { get; set; } = default!;
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
