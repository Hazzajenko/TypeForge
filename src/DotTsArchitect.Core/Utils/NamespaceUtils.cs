using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DotTsArchitect.Core.Utils;

public static class NamespaceUtils
{
    // public static string[] GetFiles

    public static string[] GetNamespaces(string[] fileLocations)
    {
        var namespacesList = new List<string>();
        foreach (var fileLocation in fileLocations)
        {
            namespacesList.AddRange(GetNamespaces(fileLocation));
        }
        return namespacesList.ToArray();
    }

    public static (ClassDeclarationSyntax[], SyntaxTree[]) GetClassesInNameSpace(
        string fullNamespacePath
    )
    {
        var filesInNamespace = Directory.GetFiles(
            fullNamespacePath,
            "*.cs",
            SearchOption.AllDirectories
        );
        var filesList = new List<string>();
        var classList = new List<ClassDeclarationSyntax>();
        var syntaxTrees = new List<SyntaxTree>();
        foreach (var file in filesInNamespace)
        {
            SyntaxTree tree = CSharpSyntaxTree.ParseText(File.ReadAllText(file));
            syntaxTrees.Add(tree);

            var root = (CompilationUnitSyntax)tree.GetRoot();
            foreach (var @class in root.DescendantNodes().OfType<ClassDeclarationSyntax>())
            {
                classList.Add(@class);
                // @class.Identifier.ValueText.Log();
                filesList.Add(@class.Identifier.ValueText);
                var properties = @class.DescendantNodes().OfType<PropertyDeclarationSyntax>();
                foreach (var property in properties)
                {
                    // property.Identifier.ValueText.Log();
                }
            }
        }
        return (classList.ToArray(), syntaxTrees.ToArray());
    }

    public static string[] GetNamespaces(string fullNamespacePath)
    {
        var filesInNamespace = Directory.GetFiles(
            fullNamespacePath,
            "*.cs",
            SearchOption.AllDirectories
        );
        var namespacesList = new List<string>();
        foreach (var file in filesInNamespace)
        {
            SyntaxTree tree = CSharpSyntaxTree.ParseText(File.ReadAllText(file));

            var root = (CompilationUnitSyntax)tree.GetRoot();

            var namespaces = root.DescendantNodes().OfType<NamespaceDeclarationSyntax>();
            foreach (var @namespace in namespaces)
            {
                namespacesList.Add(@namespace.Name.ToString());
            }
        }
        return namespacesList.ToArray();
    }

    public static ClassDeclarationSyntax GetClassDeclarationSyntax(string fileLocation)
    {
        SyntaxTree tree = CSharpSyntaxTree.ParseText(File.ReadAllText(fileLocation));

        var root = (CompilationUnitSyntax)tree.GetRoot();

        root.Log();
        // logger.Information("{Root}", root.ToFullString());

        var @class = root.DescendantNodes().OfType<ClassDeclarationSyntax>().FirstOrDefault();
        if (@class is null)
        {
            throw new Exception("Class is null");
        }
        return @class;
        /*
        foreach (var @class in root.DescendantNodes().OfType<ClassDeclarationSyntax>())
        {
            // Console.WriteLine(@class.Identifier.ValueText); // Print the class name
            // logger.Information("{Class}", @class.Identifier.ValueText);
            @class.Identifier.ValueText.Log();
            var properties = @class.DescendantNodes().OfType<PropertyDeclarationSyntax>();
            foreach (var property in properties)
            {
                // Console.WriteLine($" - Property: {property.Identifier.ValueText}");
                // logger.Information("{Property}", property.Identifier.ValueText);
                property.Identifier.ValueText.Log();
            }
        }*/
    }
}
