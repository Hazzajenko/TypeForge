using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Serilog;
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
        this IEnumerable<ConfigDirectoryWithPath> nameSpaces,
        CSharpCompilation compilation,
        TypeForgeConfig config
    ) =>
        nameSpaces
            .GetTypeScriptFilesForDirectory(compilation, config)
            .GroupBy(x => x.PathRelativeToRootOutput)
            .Select(x => x.ToTypeScriptFolder(config));

    private static TypeScriptFolder ToTypeScriptFolder(
        this IGrouping<string, TypeScriptFile> filesGroupedByPath
    ) => new() { FolderName = filesGroupedByPath.Key, Files = filesGroupedByPath };

    private static TypeScriptFolder ToTypeScriptFolder(
        this IGrouping<string, TypeScriptFile> filesGroupedByPath,
        TypeForgeConfig config
    )
    {
        var folderName = filesGroupedByPath.Key;
        var outputPath = config.ConfigDirectories
            .GetConfigDirectoryByFolderName(folderName, config.FolderNameCase)
            .Output;
        return new TypeScriptFolder
        {
            FolderName = folderName,
            Files = filesGroupedByPath,
            OutputPath = outputPath
        };
    }

    private static ConfigDirectoryWithPath GetConfigDirectoryByFolderName(
        this IEnumerable<ConfigDirectoryWithPath> configDirectories,
        string folderName,
        FolderNameCase folderNameCase
    )
    {
        // ReSharper disable once CollectionNeverQueried.Local
        var attempted = new List<ValueTuple<string, string>>();
        return configDirectories
                .Where(x =>
                {
                    var xFolderName = x.Input
                        .GetLastDirectoryInPath()
                        .ToCaseOfOption(folderNameCase);
                    var bFolderName = folderName.Contains(Path.DirectorySeparatorChar)
                        ? folderName.GetStartOfPath()
                        : folderName;
                    attempted.Add((bFolderName, xFolderName));
                    return xFolderName == bFolderName;
                })
                .SingleOrDefault() ?? throw new ArgumentNullException(nameof(configDirectories));
    }

    public static IEnumerable<TypeScriptFile> GetTypeScriptFilesForDirectory(
        this IEnumerable<ConfigDirectoryWithPath> nameSpaces,
        CSharpCompilation compilation,
        TypeForgeConfig config
    )
    {
        return nameSpaces.SelectMany(directoryWithPath =>
        {
            var filesInNamespace = directoryWithPath.IncludeChildren
                ? directoryWithPath.GetFilesInDirectoryAndSubDirectoriesByDepth()
                : directoryWithPath.GetFilesInOnlyTopDirectory();

            return filesInNamespace.Select(
                file => file.ToTypeScriptFile(compilation, config, directoryWithPath)
            );
        });
    }

    private static TypeScriptFile ToTypeScriptFile(
        this string file,
        CSharpCompilation compilation,
        TypeForgeConfig config,
        ConfigDirectoryWithPath directory
    )
    {
        SyntaxTree tree = CSharpSyntaxTree.ParseText(File.ReadAllText(file));
        var root = (CompilationUnitSyntax)tree.GetRoot();
        var fileName = file.GetFileNameFromPath();
        var pathRelativeToRoot = file.GetPathFromParentNamespaceKeepRootFolder(
            directory.Path,
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
            PathRelativeToRootOutput = pathRelativeToRoot,
            Types = types
        };
    }
}
