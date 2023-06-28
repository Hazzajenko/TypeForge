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

    // private readonly bool _groupByNamespace;
    // private readonly bool _nameSpaceInOneFile;

    public WriterService(TypeForgeConfig config)
    {
        _config = config;
        // EndLinesWithSemicolon = config.EndLinesWithSemicolon;
        // GenerateIndexFile = config.GenerateIndexFile;
        // _groupByNamespace = config.GroupByNamespace;
        // _nameSpaceInOneFile = config.NameSpaceInOneFile;
    }

    public void WriteFromConfig()
    {
        CSharpCompilation compilation = _config.ConfigDirectories.GetCSharpCompilation();
        var typeScriptFolders = _config.ConfigDirectories.GetTypeScriptFolders(
            compilation,
            _config
        );
        // var outputDir = Path.Combine(_config.ProjectDir, "output");
        //
        // if (_nameSpaceInOneFile)
        // {
        //     WriteAllFilesIntoOneFile(typeScriptFolders);
        //     return;
        // }

        foreach (TypeScriptFolder typeScriptFolder in typeScriptFolders)
        {
            HandleTypeScriptFolderWrite(typeScriptFolder, typeScriptFolders);
        }

        if (GenerateIndexFile)
        {
            WriteIndexFilesForAllTopFolders(typeScriptFolders);
        }
        //
        // if (_config is not { GenerateIndexFile: true, GroupByNamespace: false })
        //     return;
        //
        // WriteExportsForIndexFile(typeScriptFolders, outputDir);
    }

    private void WriteIndexFilesForAllTopFolders(IEnumerable<TypeScriptFolder> typeScriptFolders)
    {
        var typeScriptTopFolders = GetAllTopFolders(typeScriptFolders);
        var typeScriptTopFolders2 = GetAllTopFoldersUnique(typeScriptFolders);
        foreach (TypeScriptFolder typeScriptTopFolder in typeScriptTopFolders2)
        {
            Log.Information("TopFolder: {TopFolder}", typeScriptTopFolder.OutputPath);
        }

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

            IEnumerable<ConfigDirectoryWithPath> folderConfig = _config.ConfigDirectories
                .Where(x => outputPathsForTopFolders.Contains(x.Output))
                .ToList();

            if (folderConfig.Any(x => x.KeepRootFolder is false))
                continue;

            var exportNames = foldersInTopFolder
                .Select(x => x.FolderName)
                .Select(x => $"{x}/index")
                .ToArray();

            if (exportNames.Contains("export * from './other-name-space/index'"))
            {
                Log.Information("Found other name space");
            }

            WriteExportsForIndexFile(exportNames, topFolder);
        }
    }

    private void HandleTypeScriptFolderWrite(
        TypeScriptFolder typeScriptFolder,
        IEnumerable<TypeScriptFolder> typeScriptFolders
    )
    {
        ConfigDirectoryWithPath configD = GetConfigDirectoryByFolderName(
            typeScriptFolder.FolderName
        );
        if (configD.Flatten is false)
        {
            if (configD.KeepRootFolder is false)
            {
                var adjustedFolder = new TypeScriptFolder(typeScriptFolder) { FolderName = "" };
                var adjustedChildFolders = GetChildFolders(typeScriptFolders, typeScriptFolder)
                    .Select(
                        x =>
                            new TypeScriptFolder(x)
                            {
                                FolderName = x.FolderName.Replace(
                                    $"{typeScriptFolder.FolderName}{Path.DirectorySeparatorChar}",
                                    ""
                                )
                            }
                    );
                var adjustedChildFolderNames = adjustedChildFolders.Select(x => x.FolderName);
                WriteTypeScriptFolderToPersonalOutput(adjustedFolder, adjustedChildFolderNames);
                return;
            }

            var childFolders = GetChildFolders(typeScriptFolders, typeScriptFolder);
            var childFolderNames = childFolders.Select(x => x.FolderName);
            WriteTypeScriptFolderToPersonalOutput(typeScriptFolder, childFolderNames);
        }
        else
        {
            if (IsTopFolder(typeScriptFolder) is false)
                return;
            if (configD.KeepRootFolder is false)
            {
                // TypeScriptFolder adjustedFolder = TypeScriptFolder.RemoveFolderName(
                //     typeScriptFolder
                // );
                // Log.Information("AdjustedFolder: {AdjustedFolder}", adjustedFolder.FolderName);
                var adjustedFolder = new TypeScriptFolder(typeScriptFolder) { FolderName = "" };
                var adjustedChildFolders = GetChildTypeScriptFolders(
                        typeScriptFolders,
                        adjustedFolder
                    )
                    .Select(
                        x =>
                            new ChildTypeScriptFolder(x)
                            {
                                FolderName = x.FolderName.Replace(
                                    $"{typeScriptFolder.FolderName}{Path.DirectorySeparatorChar}",
                                    ""
                                )
                            }
                    );
                WriteTypeScriptFolderToPersonalOutputFlatSubFolders(
                    adjustedFolder,
                    adjustedChildFolders
                );

                return;
            }
            var childFolders = GetChildTypeScriptFolders(typeScriptFolders, typeScriptFolder);
            WriteTypeScriptFolderToPersonalOutputFlatSubFolders(typeScriptFolder, childFolders);
        }
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
            )
            .Where(x =>
            {
                var amountOfSlashesForParent = typeScriptFolder.FolderName.Count(
                    c => c == Path.DirectorySeparatorChar
                );
                var amountOfSlashesForChild = x.FolderName.Count(
                    c => c == Path.DirectorySeparatorChar
                );
                if (typeScriptFolder.FolderName == "")
                {
                    return amountOfSlashesForChild == 0;
                }

                return amountOfSlashesForChild - amountOfSlashesForParent == 1;
            })
            .Select(x =>
            {
                if (x.FolderName.Contains(Path.DirectorySeparatorChar))
                {
                    return new TypeScriptFolder
                    {
                        FolderName = x.FolderName.Split(Path.DirectorySeparatorChar).Last(),
                        Files = x.Files
                    };
                }

                return x;
            })
            .DistinctBy(x => x.FolderName)
            .ToList();
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
            )
            .Where(x =>
            {
                var amountOfSlashesForParent = typeScriptFolder.FolderName.Count(
                    c => c == Path.DirectorySeparatorChar
                );
                var amountOfSlashesForChild = x.FolderName.Count(
                    c => c == Path.DirectorySeparatorChar
                );
                if (typeScriptFolder.FolderName == "")
                {
                    return amountOfSlashesForChild == 0;
                }

                return amountOfSlashesForChild - amountOfSlashesForParent == 1;
            })
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
            // .Select(x =>
            // {
            //     if (x.FolderName.Contains(Path.DirectorySeparatorChar))
            //     {
            //         return new ChildTypeScriptFolder
            //         {
            //             FolderName = x.FolderName.Split(Path.DirectorySeparatorChar).Last(),
            //             Files = x.Files
            //         };
            //     }
            //
            //     return new ChildTypeScriptFolder { FolderName = x.FolderName, Files = x.Files };
            // })
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
        // if (_groupByNamespace is false)
        // {
        //     path = typeScriptFolder.OutputPath;
        // }
        foreach (TypeScriptFile typeScriptFile in typeScriptFolder.Files)
        {
            WriteTypeScriptFile(typeScriptFile, path);
        }

        if (GenerateIndexFile is false)
            return;

        // if (_config is not { GenerateIndexFile: true, GroupByNamespace: true })
        // return;

        WriteExportsForIndexFile(typeScriptFolder.Files, path, childFolderNames);
    }

    private void WriteTypeScriptFolderToPersonalOutputFlatSubFolders(
        TypeScriptFolder typeScriptFolder,
        IEnumerable<ChildTypeScriptFolder> childFolders
    )
    {
        var path = Path.Combine(typeScriptFolder.OutputPath, typeScriptFolder.FolderName);
        // if (_groupByNamespace is false)
        // {
        //     path = typeScriptFolder.OutputPath;
        // }
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

        // if (_config is not { GenerateIndexFile: true, GroupByNamespace: true })
        //     return;

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

    // private void WriteTypeScriptFolder(
    //     TypeScriptFolder typeScriptFolder,
    //     string outputDir,
    //     IEnumerable<string> childFolderNames
    // )
    // {
    //     var path = Path.Combine(outputDir, typeScriptFolder.FolderName);
    //     if (_groupByNamespace is false)
    //     {
    //         path = outputDir;
    //     }
    //     foreach (TypeScriptFile typeScriptFile in typeScriptFolder.Files)
    //     {
    //         WriteTypeScriptFile(typeScriptFile, path);
    //     }
    //
    //     if (_config is not { GenerateIndexFile: true, GroupByNamespace: true })
    //         return;
    //
    //     WriteExportsForIndexFile(typeScriptFolder.Files, path, childFolderNames);
    // }

    // private void WriteAllFilesIntoOneFile(IEnumerable<TypeScriptFolder> typeScriptFolders)
    // {
    //     var outputDir = Path.Combine(_config.ProjectDir, "output\\");
    //     var fileInfo = new FileInfo($"{outputDir}all.ts");
    //     using FileStream fs = fileInfo.CreateFileSafe();
    //     var types = typeScriptFolders.GetTypesFromFolders();
    //     foreach (TypeScriptType typeScriptType in types)
    //     {
    //         WriteTsFile(fs, typeScriptType);
    //         if (typeScriptType != types.Last())
    //         {
    //             fs.WriteEmptyLine();
    //         }
    //     }
    //
    //     if (_config is not { GenerateIndexFile: true, GroupByNamespace: false })
    //         return;
    //
    //     var fileNames = new List<string> { fileInfo.Name };
    //     WriteExportsForIndexFile(fileNames, outputDir);
    // }

    private void WriteTypeScriptFile(TypeScriptFile typeScriptFile, string outputDir)
    {
        var fileName = Path.Combine(outputDir, typeScriptFile.FileName);
        Log.Logger.Information("Creating file {FileName}", fileName);
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
            if (line.Contains("./other-name-space-test/index"))
            {
                Log.Logger.Information("Found");
            }
            indexFs.WriteLine(line, EndLinesWithSemicolon);
        }
    }

    private void WriteExportsForIndexFile(
        IEnumerable<TypeScriptFile> typeScriptFiles,
        string outputDir
    )
    {
        string indexFile = Path.Combine(outputDir, "index.ts");
        // Log.Logger.Information("Creating index file {IndexFile}", indexFile);
        var indexTemplateFile = new FileInfo(indexFile);
        using FileStream indexFs = indexTemplateFile.CreateFileSafe();

        foreach (TypeScriptFile typeScriptFile in typeScriptFiles)
        {
            string fileName = typeScriptFile.FileName.TakeOutTsExtension();
            string exportModel = $"export * from './{fileName}'";
            if (exportModel.Contains("./other-name-space-test/index"))
            {
                Log.Logger.Information("Found");
            }
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
            if (exportModel.Contains("./other-name-space-test/index"))
            {
                Log.Logger.Information("Found");
            }
            indexFs.WriteLine(exportModel, EndLinesWithSemicolon);
        }

        foreach (string childFolderName in childFolderNames)
        {
            string exportModel = $"export * from './{childFolderName}/index'";
            if (exportModel.Contains("./other-name-space-test/index"))
            {
                Log.Logger.Information("Found");
            }
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
            if (exportModel.Contains("./other-name-space-test/index"))
            {
                Log.Logger.Information("Found");
            }
            indexFs.WriteLine(exportModel, EndLinesWithSemicolon);
        }
    }

    private void WriteTsFile(FileStream fs, TypeScriptType typeScriptType)
    {
        var exportModel = typeScriptType.GetExportModelType(_config);
        fs.WriteLine(exportModel, false);

        foreach (var typeProperty in typeScriptType.Properties)
        {
            var property = $"{typeProperty.Name}: {typeProperty.Type}";
            fs.WriteLineWithTab(property, EndLinesWithSemicolon);
        }

        fs.WriteLine("}", false);
    }

    public ConfigDirectoryWithPath GetConfigDirectoryByFolderName(string folderName)
    {
        return _config.ConfigDirectories
                .Where(x =>
                {
                    var xFolderName = x.Input
                        .GetLastDirectoryInPath()
                        .ToCaseOfOption(_config.FolderNameCase);
                    var bFolderName = folderName.Contains(Path.DirectorySeparatorChar)
                        ? folderName.GetStartOfPath()
                        : folderName;
                    // attempted.Add((bFolderName, xFolderName));
                    return xFolderName == bFolderName;
                })
                .SingleOrDefault()
            ?? throw new ArgumentNullException(nameof(GetConfigDirectoryByFolderName));
    }

    public IEnumerable<TypeScriptFolder> GetTypeScriptFoldersByOutputPath(
        IEnumerable<TypeScriptFolder> typeScriptFolders,
        string outputPath
    )
    {
        return typeScriptFolders.Where(x => x.OutputPath == outputPath).ToList();
    }
}
