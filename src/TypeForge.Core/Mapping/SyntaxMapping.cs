using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using TypeForge.Core.Configuration;
using TypeForge.Core.Configuration.TypeForgeConfig;
using TypeForge.Core.Extensions;
using TypeForge.Core.Models;
using TypeForge.Core.Services;
using TypeForge.Core.TypeConverting;

namespace TypeForge.Core.Mapping;

public static class SyntaxMapping
{
    public static IEnumerable<TypeScriptFolder> GetTypeScriptFolders(
        this IEnumerable<ConfigNameSpaceWithPath> nameSpaces,
        CSharpCompilation compilation,
        TypeForgeConfig config
    ) =>
        nameSpaces
            .GetTypeScriptFilesForDirectory(compilation, config)
            .GroupBy(x => x.PathFromParentNamespace)
            .Select(x => x.ToTypeScriptFolder());

    private static TypeScriptFolder ToTypeScriptFolder(
        this IGrouping<string, TypeScriptFile> filesGroupedByPath
    ) => new() { FolderName = filesGroupedByPath.Key, Files = filesGroupedByPath };

    public static IEnumerable<TypeScriptFile> GetTypeScriptFilesForDirectory(
        this IEnumerable<ConfigNameSpaceWithPath> nameSpaces,
        CSharpCompilation compilation,
        TypeForgeConfig config
    )
    {
        return nameSpaces.SelectMany(@nameSpace =>
        {
            var filesInNamespace = @nameSpace.Path.GetFilesInNamespace();

            return filesInNamespace.Select(
                file => file.ToTypeScriptFile(compilation, config, nameSpace)
            );
        });
    }

    private static TypeScriptFile ToTypeScriptFile(
        this string file,
        CSharpCompilation compilation,
        TypeForgeConfig config,
        ConfigNameSpaceWithPath nameSpace
    )
    {
        SyntaxTree tree = CSharpSyntaxTree.ParseText(File.ReadAllText(file));
        var root = (CompilationUnitSyntax)tree.GetRoot();
        var fileName = file.GetFileNameFromPath();
        var pathRelativeToRoot = file.GetPathFromParentNamespace(
            @nameSpace.Path,
            fileName,
            config.FolderNameCase
        );
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
    }
}
