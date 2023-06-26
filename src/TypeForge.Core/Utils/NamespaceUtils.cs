using TypeForge.Core.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Serilog;
using TypeForge.Core.Configuration;
using TypeForge.Core.Mapping;
using TypeForge.Core.Models;

namespace TypeForge.Core.Utils;

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

    // ConfigNameSpaceWithPath

    public static IEnumerable<SyntaxTree> GetSyntaxTrees(this string directoryPath)
    {
        var filesInNamespace = Directory.GetFiles(
            directoryPath,
            "*.cs",
            SearchOption.AllDirectories
        );
        return filesInNamespace.Select(file => CSharpSyntaxTree.ParseText(File.ReadAllText(file)));
    }

    public static SyntaxTree[] GetSyntaxTrees(this ConfigNameSpaceWithPath[] nameSpaces)
    {
        var paths = nameSpaces.Select(x => x.Path);
        var syntaxTrees = paths.SelectMany(fileLocation =>
        {
            var filesInNamespace = Directory.GetFiles(
                fileLocation,
                "*.cs",
                SearchOption.AllDirectories
            );
            return filesInNamespace.Select(
                file => CSharpSyntaxTree.ParseText(File.ReadAllText(file))
            );
        });

        return syntaxTrees.ToArray();
    }

    public static SyntaxTree[] GetSyntaxTrees(this ConfigNameSpace[] nameSpaces)
    {
        var paths = nameSpaces.Select(x => x.Name);
        var syntaxTrees = paths.SelectMany(fileLocation =>
        {
            var filesInNamespace = Directory.GetFiles(
                fileLocation,
                "*.cs",
                SearchOption.AllDirectories
            );
            return filesInNamespace.Select(
                file => CSharpSyntaxTree.ParseText(File.ReadAllText(file))
            );
        });

        return syntaxTrees.ToArray();
    }

    public static SyntaxTree[] GetSyntaxTrees(string[] fileLocations)
    {
        var syntaxTrees = fileLocations.SelectMany(fileLocation =>
        {
            var filesInNamespace = Directory.GetFiles(
                fileLocation,
                "*.cs",
                SearchOption.AllDirectories
            );
            return filesInNamespace.Select(
                file => CSharpSyntaxTree.ParseText(File.ReadAllText(file))
            );
        });

        return syntaxTrees.ToArray();
    }

    public static IEnumerable<NameSpaceWithNodesAndFileName> GetClassDeclarationsForNamespace(
        this ConfigNameSpaceWithPath nameSpace,
        GlobalConfig config
    )
    {
        var fileLocation = nameSpace.Path;
        var filesInNamespace = Directory.GetFiles(
            fileLocation,
            "*.cs",
            SearchOption.AllDirectories
        );
        var namespaces = filesInNamespace.SelectMany(file =>
        {
            SyntaxTree tree = CSharpSyntaxTree.ParseText(File.ReadAllText(file));
            var root = (CompilationUnitSyntax)tree.GetRoot();
            var nodes = root.DescendantNodes().OfType<ClassDeclarationSyntax>();
            var fileName = file.GetFileNameFromPath();
            var pathFromParentNamespace = file.GetPathFromParentNamespace(nameSpace.Path, fileName);
            fileName = fileName.GetFileName(config);
            return nodes.Select(
                node =>
                    new NameSpaceWithNodesAndFileName
                    {
                        NameSpace = nameSpace,
                        FileName = fileName,
                        PathFromParentNamespace = pathFromParentNamespace,
                        Nodes = nodes,
                    }
            );
        });

        var namespaceDict = namespaces
            .GroupBy(n => n.FileName)
            .ToDictionary(g => g.Key, g => g.First());
        var distinctNamespaces = namespaceDict.Values.ToList();
        return distinctNamespaces;
    }

    private static string GetPathFromParentNamespace(
        this string fileLocation,
        string parentNamespace,
        string fileName
    )
    {
        var splitFromNamespace = fileLocation.Split(parentNamespace)[1];
        var path = fileLocation.Replace(splitFromNamespace, "");
        var takeOutFileName = path.Replace(fileName, "");
        var split = takeOutFileName.Split("\\")[takeOutFileName.Split("\\").Length - 1];
        return split;
    }

    private static string GetFileNameFromPath(this string path)
    {
        var split = path.Split("\\");
        var fileName = split[split.Length - 1];
        //
        // Log.Logger.Information("Path {Path}", path);
        // Log.Logger.Information("Split {Split}", split);
        // Log.Logger.Information("File name {FileName}", fileName);
        return fileName;
    }

    public static IEnumerable<TypeScriptFile> GetTypeScriptFilesForDirectory(
        this string directory,
        InputGlobalConfig config,
        CSharpCompilation compilation
    )
    {
        var filesInNamespace = Directory.GetFiles(directory, "*.cs", SearchOption.AllDirectories);

        return filesInNamespace.Select(file =>
        {
            SyntaxTree tree = CSharpSyntaxTree.ParseText(File.ReadAllText(file));
            var root = (CompilationUnitSyntax)tree.GetRoot();
            var fileName = file.GetFileNameFromPath();
            var pathRelativeToRoot = file.GetPathFromParentNamespace(directory, fileName);
            fileName = fileName.GetFileName(config);
            var types = root.DescendantNodes()
                .OfType<ClassDeclarationSyntax>()
                .Select(x => x.MapToTypeScriptType(config, compilation));
            return new TypeScriptFile
            {
                FileName = fileName,
                PathFromParentNamespace = pathRelativeToRoot,
                Types = types
            };
        });
    }

    public static ClassDeclarationSyntax[] GetClassDeclarations(
        this ConfigNameSpaceWithPath[] nameSpaces
    )
    {
        var paths = nameSpaces.Select(x => x.Path);
        var classList = paths.SelectMany(fileLocation =>
        {
            var filesInNamespace = Directory.GetFiles(
                fileLocation,
                "*.cs",
                SearchOption.AllDirectories
            );

            return filesInNamespace.Select(file =>
            {
                SyntaxTree tree = CSharpSyntaxTree.ParseText(File.ReadAllText(file));
                var root = (CompilationUnitSyntax)tree.GetRoot();
                return root.DescendantNodes().OfType<ClassDeclarationSyntax>();
            });
        });

        return classList.SelectMany(x => x).ToArray();
    }

    public static ClassDeclarationSyntax[] GetClassDeclarations(this ConfigNameSpace[] nameSpaces)
    {
        var paths = nameSpaces.Select(x => x.Name);
        var classList = paths.SelectMany(fileLocation =>
        {
            var filesInNamespace = Directory.GetFiles(
                fileLocation,
                "*.cs",
                SearchOption.AllDirectories
            );
            return filesInNamespace.Select(file =>
            {
                SyntaxTree tree = CSharpSyntaxTree.ParseText(File.ReadAllText(file));
                var root = (CompilationUnitSyntax)tree.GetRoot();
                return root.DescendantNodes().OfType<ClassDeclarationSyntax>();
            });
        });

        return classList.SelectMany(x => x).ToArray();
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
