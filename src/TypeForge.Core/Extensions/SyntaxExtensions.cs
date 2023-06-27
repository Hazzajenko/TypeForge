using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using TypeForge.Core.Configuration;

namespace TypeForge.Core.Extensions;

public static class SyntaxExtensions
{
    public static IEnumerable<SyntaxTree> GetSyntaxTrees(
        this IEnumerable<ConfigNameSpaceWithPath> nameSpaces
    )
    {
        var paths = nameSpaces.Select(x => x.Path);
        return paths.SelectMany(ToSyntaxTree);
    }

    public static IEnumerable<SyntaxTree> GetSyntaxTrees(this string directory)
    {
        var paths = directory.GetFilesInNamespace();
        return paths.SelectMany(ToSyntaxTree);
    }

    private static IEnumerable<SyntaxTree> ToSyntaxTree(string file) =>
        file.GetFilesInNamespace().Select(CreateSyntaxTree);

    private static SyntaxTree CreateSyntaxTree(string file) =>
        CSharpSyntaxTree.ParseText(File.ReadAllText(file));

    public static CSharpCompilation GetCSharpCompilation(
        this IEnumerable<ConfigNameSpaceWithPath> nameSpaces
    ) => nameSpaces.GetSyntaxTrees().CreateCompilation();

    public static CSharpCompilation GetCSharpCompilation(this string directory) =>
        directory.GetSyntaxTrees().CreateCompilation();

    private static CSharpCompilation CreateCompilation(this IEnumerable<SyntaxTree> syntaxTrees)
    {
        var references = AppDomain.CurrentDomain
            .GetAssemblies()
            .Where(a => !a.IsDynamic)
            .Select(a => MetadataReference.CreateFromFile(a.Location));
        CSharpCompilation compilation = CSharpCompilation
            .Create("MyCompilation")
            .AddReferences(references)
            .AddSyntaxTrees(syntaxTrees);

        return compilation;
    }
}
