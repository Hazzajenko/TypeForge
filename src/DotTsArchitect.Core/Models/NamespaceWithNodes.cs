using DotTsArchitect.Core.Configuration;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DotTsArchitect.Core.Models;

public class NamespaceWithNodes
{
    public ConfigNameSpaceWithPath Namespace { get; set; } = default!;
    public IEnumerable<ClassDeclarationSyntax> Nodes { get; set; } = default!;
}
