using Microsoft.CodeAnalysis.CSharp;
using Serilog;
using TypeForge.Core.Configuration;
using TypeForge.Core.Configuration.TypeForgeConfig;
using TypeForge.Core.Extensions;
using TypeForge.Core.Mapping;
using TypeForge.Core.Models;

namespace TypeForge.Core.Services;

public class WriterService
{
    private readonly TypeForgeConfig _config;
    private bool EndLinesWithSemicolon => _config.EndLinesWithSemicolon;
    private bool GenerateIndexFile => _config.GenerateIndexFile;
    private IEnumerable<ConfigDirectoryWithPath> ConfigDirectories => _config.ConfigDirectories;

    public WriterService(TypeForgeConfig config)
    {
        _config = config;
    }

    public void WriteFromConfig()
    {
        CSharpCompilation compilation = ConfigDirectories.GetCSharpCompilation();
        var typeScriptFolders = ConfigDirectories.GetTypeScriptFolders(compilation, _config);

        foreach (TypeScriptFolder typeScriptFolder in typeScriptFolders)
        {
            HandleTypeScriptFolderWrite(typeScriptFolder, typeScriptFolders);
        }

        if (GenerateIndexFile)
        {
            WriteIndexFilesForAllTopFolders(typeScriptFolders);
        }

        var amountOfFilesTotal = typeScriptFolders.Sum(x => x.Files.Count());
        Log.Information("Total amount of files: {AmountOfFilesTotal}", amountOfFilesTotal);
    }

    private void WriteIndexFilesForAllTopFolders(IEnumerable<TypeScriptFolder> typeScriptFolders)
    {
        var typeScriptTopFolders = GetAllTopFoldersUnique(typeScriptFolders);

        var topFoldersGroupedByOutputPath = typeScriptTopFolders
            .GroupBy(x => x.OutputPath)
            .Select(x => x.First().OutputPath)
            .ToList();

        foreach (var topFolder in topFoldersGroupedByOutputPath)
        {
            var foldersInTopFolder = GetTypeScriptFoldersByOutputPath(
                typeScriptTopFolders,
                topFolder
            );

            var outputPathsForTopFolders = foldersInTopFolder.Select(x => x.OutputPath).ToArray();

            IEnumerable<ConfigDirectoryWithPath> folderConfig = ConfigDirectories
                .Where(x => outputPathsForTopFolders.Contains(x.Output))
                .ToList();

            if (folderConfig.Any(x => x.KeepRootFolder is false))
                continue;

            var exportNames = foldersInTopFolder
                .Select(x => x.FolderName)
                .Select(x => $"{x}/index")
                .ToArray();

            WriteExportsForIndexFile(exportNames, topFolder);
        }
    }

    private void HandleTypeScriptFolderWrite(
        TypeScriptFolder typeScriptFolder,
        IEnumerable<TypeScriptFolder> typeScriptFolders
    )
    {
        ConfigDirectoryWithPath directoryConfig = GetConfigDirectoryByFolderName(
            typeScriptFolder.FolderName
        );
        if (directoryConfig.Flatten is false)
        {
            if (directoryConfig.KeepRootFolder is false)
            {
                HandleKeepRootFolderIsFalse(typeScriptFolder, typeScriptFolders);
                return;
            }

            var childFolders = GetChildFolders(typeScriptFolders, typeScriptFolder);
            var childFolderNames = childFolders.Select(x => x.FolderName);
            WriteTypeScriptFolderToPersonalOutput(typeScriptFolder, childFolderNames);
            return;
        }

        if (IsTopFolder(typeScriptFolder) is false)
            return;
        if (directoryConfig.KeepRootFolder is false)
        {
            HandleKeepRootFolderIsFalseAndFlatten(typeScriptFolder, typeScriptFolders);
            return;
        }
        var childTypeScriptFolders = GetChildTypeScriptFolders(typeScriptFolders, typeScriptFolder);
        WriteTypeScriptFolderToPersonalOutputFlatSubFolders(
            typeScriptFolder,
            childTypeScriptFolders
        );
    }

    private void HandleKeepRootFolderIsFalseAndFlatten(
        TypeScriptFolder typeScriptFolder,
        IEnumerable<TypeScriptFolder> typeScriptFolders
    )
    {
        var adjustedFolder = new TypeScriptFolder(typeScriptFolder) { FolderName = "" };
        var adjustedChildFolders = GetChildTypeScriptFolders(typeScriptFolders, adjustedFolder)
            .Select(x => x.ToRemoveFolderName());
        WriteTypeScriptFolderToPersonalOutputFlatSubFolders(adjustedFolder, adjustedChildFolders);
    }

    private void HandleKeepRootFolderIsFalse(
        TypeScriptFolder typeScriptFolder,
        IEnumerable<TypeScriptFolder> typeScriptFolders
    )
    {
        var adjustedFolder = new TypeScriptFolder(typeScriptFolder) { FolderName = "" };
        var adjustedChildFolders = GetChildFolders(typeScriptFolders, typeScriptFolder)
            .Select(x => x.ToRemoveFolderName());
        var adjustedChildFolderNames = adjustedChildFolders.Select(x => x.FolderName);
        WriteTypeScriptFolderToPersonalOutput(adjustedFolder, adjustedChildFolderNames);
    }

    private static IEnumerable<TypeScriptFolder> GetChildFolders(
        IEnumerable<TypeScriptFolder> typeScriptFolders,
        TypeScriptFolder typeScriptFolder
    )
    {
        return typeScriptFolders
            .Where(
                x =>
                    x.FolderName.StartsWith(typeScriptFolder.FolderName)
                    && x.FolderName != typeScriptFolder.FolderName
                    && DoesChildHaveOneMoreSlashThanParent(typeScriptFolder, x)
            )
            .Select(x => x.ToIndividualFolderName())
            .DistinctBy(x => x.FolderName)
            .ToList();
    }

    private static bool DoesChildHaveOneMoreSlashThanParent(
        TypeScriptFolder typeScriptFolder,
        TypeScriptFolder x
    )
    {
        var amountOfSlashesForParent = typeScriptFolder.FolderName.Count(
            c => c == Path.DirectorySeparatorChar
        );
        var amountOfSlashesForChild = x.FolderName.Count(c => c == Path.DirectorySeparatorChar);
        if (typeScriptFolder.FolderName == "")
        {
            return amountOfSlashesForChild == 0;
        }

        return amountOfSlashesForChild - amountOfSlashesForParent == 1;
    }

    private static IEnumerable<ChildTypeScriptFolder> GetChildTypeScriptFolders(
        IEnumerable<TypeScriptFolder> typeScriptFolders,
        TypeScriptFolder typeScriptFolder
    )
    {
        return typeScriptFolders
            .Where(
                x =>
                    x.FolderName.StartsWith(typeScriptFolder.FolderName)
                    && x.FolderName != typeScriptFolder.FolderName
                    && DoesChildHaveOneMoreSlashThanParent(typeScriptFolder, x)
            )
            .Select(x =>
            {
                var childFolders = GetChildTypeScriptFolders(typeScriptFolders, x);

                return new ChildTypeScriptFolder
                {
                    FolderName = x.FolderName,
                    Files = x.Files,
                    OutputPath = x.OutputPath,
                    ChildFolders = childFolders
                };
            })
            .ToList();
    }

    private static bool IsTopFolder(TypeScriptFolder typeScriptFolder)
    {
        return typeScriptFolder.FolderName.Contains(Path.DirectorySeparatorChar) is false;
    }

    private static IEnumerable<TypeScriptFolder> GetAllTopFolders(
        IEnumerable<TypeScriptFolder> typeScriptFolders
    )
    {
        return typeScriptFolders
            .Where(x => x.FolderName.Contains(Path.DirectorySeparatorChar) is false)
            .Distinct()
            .ToList();
    }

    private static IEnumerable<TypeScriptFolder> GetAllTopFoldersUnique(
        IEnumerable<TypeScriptFolder> typeScriptFolders
    )
    {
        return typeScriptFolders
            .Where(x => x.FolderName.Contains(Path.DirectorySeparatorChar) is false)
            .GroupBy(x => x.FolderName)
            .Select(x => x.OrderByDescending(c => c.OutputPath.Length).First())
            .Distinct()
            .ToList();
    }

    private void WriteTypeScriptFolderToPersonalOutput(
        TypeScriptFolder typeScriptFolder,
        IEnumerable<string> childFolderNames
    )
    {
        var path = Path.Combine(typeScriptFolder.OutputPath, typeScriptFolder.FolderName);
        foreach (TypeScriptFile typeScriptFile in typeScriptFolder.Files)
        {
            WriteTypeScriptFile(typeScriptFile, path);
        }

        if (GenerateIndexFile is false)
            return;

        WriteExportsForIndexFile(typeScriptFolder.Files, path, childFolderNames);
    }

    private void WriteTypeScriptFolderToPersonalOutputFlatSubFolders(
        TypeScriptFolder typeScriptFolder,
        IEnumerable<ChildTypeScriptFolder> childFolders
    )
    {
        var path = Path.Combine(typeScriptFolder.OutputPath, typeScriptFolder.FolderName);
        foreach (TypeScriptFile typeScriptFile in typeScriptFolder.Files)
        {
            WriteTypeScriptFile(typeScriptFile, path);
        }

        foreach (ChildTypeScriptFolder childFolder in childFolders)
        {
            foreach (TypeScriptFile typeScriptFile in childFolder.Files)
            {
                WriteTypeScriptFile(typeScriptFile, path);
            }

            WriteAllFilesInSubFolders(childFolder, path);
        }

        if (GenerateIndexFile is false)
            return;

        var allFiles = typeScriptFolder.Files.Concat(childFolders.SelectMany(x => x.Files));

        WriteExportsForIndexFile(allFiles, path);
    }

    private void WriteAllFilesInSubFolders(ChildTypeScriptFolder childFolder, string path)
    {
        foreach (ChildTypeScriptFolder childFolderChildFolder in childFolder.ChildFolders)
        {
            foreach (TypeScriptFile typeScriptFile in childFolderChildFolder.Files)
            {
                WriteTypeScriptFile(typeScriptFile, path);
            }
        }
    }

    private void WriteTypeScriptFile(TypeScriptFile typeScriptFile, string outputDir)
    {
        var fileName = Path.Combine(outputDir, typeScriptFile.FileName);
        var fileInfo = new FileInfo(fileName);
        using FileStream fs = fileInfo.CreateFileSafe();
        foreach (TypeScriptType typeScriptType in typeScriptFile.Types)
        {
            WriteTsFile(fs, typeScriptType);
            if (typeScriptType != typeScriptFile.Types.Last())
            {
                fs.WriteEmptyLine();
            }
        }
    }

    private void WriteExportsForIndexFile(
        IEnumerable<TypeScriptFolder> typeScriptFolders,
        string outputDir
    )
    {
        string indexFile = Path.Combine(outputDir, "index.ts");
        var indexTemplateFile = new FileInfo(indexFile);
        using FileStream indexFs = indexTemplateFile.CreateFileSafe();
        var linesToWrite = typeScriptFolders
            .SelectMany(folder => folder.Files)
            .Select(x => x.GetExportStatement());

        foreach (var line in linesToWrite)
        {
            indexFs.WriteLine(line, EndLinesWithSemicolon);
        }
    }

    private void WriteExportsForIndexFile(
        IEnumerable<TypeScriptFile> typeScriptFiles,
        string outputDir
    )
    {
        string indexFile = Path.Combine(outputDir, "index.ts");
        var indexTemplateFile = new FileInfo(indexFile);
        using FileStream indexFs = indexTemplateFile.CreateFileSafe();

        foreach (TypeScriptFile typeScriptFile in typeScriptFiles)
        {
            string fileName = typeScriptFile.FileName.TakeOutTsExtension();
            string exportModel = $"export * from './{fileName}'";
            indexFs.WriteLine(exportModel, EndLinesWithSemicolon);
        }
    }

    private void WriteExportsForIndexFile(
        IEnumerable<TypeScriptFile> typeScriptFiles,
        string outputDir,
        IEnumerable<string> childFolderNames
    )
    {
        string indexFile = Path.Combine(outputDir, "index.ts");
        var indexTemplateFile = new FileInfo(indexFile);
        using FileStream indexFs = indexTemplateFile.CreateFileSafe();

        foreach (TypeScriptFile typeScriptFile in typeScriptFiles)
        {
            string fileName = typeScriptFile.FileName.TakeOutTsExtension();
            string exportModel = $"export * from './{fileName}'";
            indexFs.WriteLine(exportModel, EndLinesWithSemicolon);
        }

        foreach (string childFolderName in childFolderNames)
        {
            string exportModel = $"export * from './{childFolderName}/index'";
            indexFs.WriteLine(exportModel, EndLinesWithSemicolon);
        }
    }

    private void WriteExportsForIndexFile(IEnumerable<string> fileNames, string outputDir)
    {
        string indexFile = Path.Combine(outputDir, "index.ts");
        var indexTemplateFile = new FileInfo(indexFile);
        using FileStream indexFs = indexTemplateFile.CreateFileSafe();

        foreach (string fileName in fileNames)
        {
            var exportFileName = fileName.TakeOutTsExtension();

            string exportModel = $"export * from './{exportFileName}'";
            indexFs.WriteLine(exportModel, EndLinesWithSemicolon);
        }
    }

    private void WriteTsFile(FileStream fs, TypeScriptType typeScriptType)
    {
        var exportModel = typeScriptType.GetExportModelType(_config);
        fs.WriteLine(exportModel, false);

        foreach (TypeProperty typeProperty in typeScriptType.Properties)
        {
            var property = $"{typeProperty.Name}: {typeProperty.Type}";
            fs.WriteLineWithTab(property, EndLinesWithSemicolon);
        }

        fs.WriteLine("}", false);
    }

    public ConfigDirectoryWithPath GetConfigDirectoryByFolderName(string folderName)
    {
        return ConfigDirectories
                .Where(x =>
                {
                    var xFolderName = x.Input
                        .GetLastDirectoryInPath()
                        .ToCaseOfOption(_config.FolderNameCase);
                    var bFolderName = folderName.Contains(Path.DirectorySeparatorChar)
                        ? folderName.GetStartOfPath()
                        : folderName;
                    return xFolderName == bFolderName;
                })
                .SingleOrDefault()
            ?? throw new ArgumentNullException(nameof(GetConfigDirectoryByFolderName));
    }

    private IEnumerable<TypeScriptFolder> GetTypeScriptFoldersByOutputPath(
        IEnumerable<TypeScriptFolder> typeScriptFolders,
        string outputPath
    )
    {
        return typeScriptFolders.Where(x => x.OutputPath == outputPath).ToList();
    }
}
