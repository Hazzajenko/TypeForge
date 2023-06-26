using System.Runtime.InteropServices.ComTypes;
using System.Text;
using TypeForge.Core.Extensions;
using TypeForge.Core.Mapping;
using TypeForge.Core.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Options;
using Serilog;
using TypeForge.Core.Configuration;
using TypeForge.Core.Models;

namespace TypeForge.Core.Services;

public record WriteRequest(Type Type, WriteOptions Options);

public record WriteNamespaceRequest(ClassDeclarationSyntax Type, WriteOptions Options);

public record WriteOptions(
    string Path,
    ExportModelType ExportModelType = ExportModelType.Type,
    PropertyNameCase PropertyNameCase = PropertyNameCase.CamelCase,
    FileNameCase FileNameCase = FileNameCase.KebabCase,
    bool GenerateIndexFile = true,
    bool GroupByNamespace = false,
    bool FilePerNamespace = false
);

public class WriterService
{
    private readonly GlobalConfig _config;
    private readonly bool _endLinesWithSemicolon;

    public WriterService(GlobalConfig config)
    {
        _config = config;
        _endLinesWithSemicolon = config.EndLinesWithSemicolon;
    }

    public void WriteFromConfig()
    {
        var typeScriptFolders = GetNamespaceDataGrouped();
        var outputDir = Path.Combine(_config.ProjectDir, "output");
        Log.Information("Writing to {OutputDir}", outputDir);

        if (_config.NameSpaceInOneFile)
        {
            WriteAllFilesIntoOneFile();
            return;
        }

        foreach (TypeScriptFolder typeScriptFolder in typeScriptFolders)
        {
            WriteTypeScriptFolder(typeScriptFolder, outputDir);
        }

        if (_config.GenerateIndexFile)
        {
            var stringNames = typeScriptFolders.Select(x => $"{x.FolderName}/index").ToArray();
            WriteExportsForIndexFile(stringNames, outputDir);
        }

        if (_config is not { GenerateIndexFile: true, GroupByNamespace: false })
            return;

        WriteExportsForIndexFile(typeScriptFolders, outputDir);
    }

    private void WriteTypeScriptFolder(TypeScriptFolder typeScriptFolder, string outputDir)
    {
        var path = Path.Combine(outputDir, typeScriptFolder.FolderName);
        // Directory.CreateDirectory(path);
        Log.Logger.Information("Output {O}, {P}", outputDir, typeScriptFolder.FolderName);
        Log.Logger.Information("Creating directory {Path}", path);
        if (_config.GroupByNamespace is false)
        {
            path = outputDir;
        }
        foreach (TypeScriptFile typeScriptFile in typeScriptFolder.Files)
        {
            WriteTypeScriptFile(typeScriptFile, path);
        }

        if (_config is not { GenerateIndexFile: true, GroupByNamespace: true })
            return;

        WriteExportsForIndexFile(typeScriptFolder.Files, path);
    }

    private void WriteAllFilesIntoOneFile()
    {
        var typeScriptFolders = GetNamespaceDataGrouped();
        var outputDir = Path.Combine(_config.ProjectDir, "output\\");
        var fileInfo = new FileInfo($"{outputDir}all.ts");
        using FileStream fs = fileInfo.CreateFileSafe();
        var types = typeScriptFolders.SelectMany(x => x.Files).SelectMany(x => x.Types);
        foreach (TypeScriptType typeScriptType in types)
        {
            WriteTsFile(fs, typeScriptType);
            if (typeScriptType != types.Last())
            {
                fs.WriteEmptyLine();
            }
        }

        if (_config is not { GenerateIndexFile: true, GroupByNamespace: false })
            return;

        var fileNames = new List<string> { fileInfo.Name };
        WriteExportsForIndexFile(fileNames, outputDir);
    }

    private void WriteTypeScriptFile(TypeScriptFile typeScriptFile, string outputDir)
    {
        var fileName = Path.Combine(outputDir, typeScriptFile.FileName);
        var fileInfo = new FileInfo(fileName);
        using FileStream fs = fileInfo.CreateFileSafe(_config.FolderNameCase);
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
            .Select(GetExportStatement);

        foreach (var line in linesToWrite)
        {
            indexFs.WriteLine(line, _endLinesWithSemicolon);
        }
    }

    private void WriteExportsForIndexFile(
        IEnumerable<TypeScriptFile> typeScriptFiles,
        string outputDir
    )
    {
        string indexFile = Path.Combine(outputDir, "index.ts");
        Log.Logger.Information("Creating index file {IndexFile}", indexFile);
        var indexTemplateFile = new FileInfo(indexFile);
        using FileStream indexFs = indexTemplateFile.CreateFileSafe();

        foreach (TypeScriptFile typeScriptFile in typeScriptFiles)
        {
            string fileName = typeScriptFile.FileName.TakeOutTsExtension();
            string exportModel = $"export * from './{fileName}'";
            indexFs.WriteLine(exportModel, _endLinesWithSemicolon);
        }
    }

    private void WriteExportsForIndexFile(IEnumerable<string> fileNames, string outputDir)
    {
        string indexFile = Path.Combine(outputDir, "index.ts");
        Log.Logger.Information("Creating index file {IndexFile}", indexFile);
        var indexTemplateFile = new FileInfo(indexFile);
        using FileStream indexFs = indexTemplateFile.CreateFileSafe();

        foreach (string fileName in fileNames)
        {
            var exportFileName = fileName.TakeOutTsExtension();
            string exportModel = $"export * from './{exportFileName}'";
            indexFs.WriteLine(exportModel, _endLinesWithSemicolon);
        }
    }

    private static string GetExportStatement(TypeScriptFile file)
    {
        string fileName = file.FileName.TakeOutTsExtension();
        return $"export * from './{fileName}'";
    }

    // private IEnumerable<TypeScriptFile> GetNamespaceData()
    // {
    //     return _config.NameSpaces.SelectMany(configNameSpaceWithPath =>
    //     {
    //         var @classes = configNameSpaceWithPath.GetClassDeclarationsForNamespace(_config);
    //         return @classes.Select(CreateTypeScriptFileConfig);
    //     });
    // }

    private IEnumerable<TypeScriptFolder> GetNamespaceDataGrouped()
    {
        return _config.NameSpaces
            .SelectMany(configNameSpaceWithPath =>
            {
                var @classes = configNameSpaceWithPath.GetClassDeclarationsForNamespace(_config);
                return @classes.Select(CreateTypeScriptFileConfig);
            })
            // .GroupBy(x => x.NameSpace)
            .GroupBy(x => x.PathFromParentNamespace)
            .Select(
                x =>
                    new TypeScriptFolder
                    {
                        FolderName = x.Key.ToCaseOfOption(_config.FolderNameCase),
                        Files = x
                    }
            );
    }

    // private IEnumerable<TypeScriptFolder> GetNamespaceDataGroupedByNameSpace()
    // {
    //     return _config.NameSpaces
    //         .SelectMany(configNameSpaceWithPath =>
    //         {
    //             var @classes = configNameSpaceWithPath.GetClassDeclarationsForNamespace(_config);
    //             return @classes.Select(CreateTypeScriptFileConfig);
    //         })
    //         .GroupBy(x => x.NameSpace)
    //         .Select(
    //             x =>
    //                 new TypeScriptFolder
    //                 {
    //                     FolderName = x.Key,
    //                     Files = x.Select(c => c.File).ToList()
    //                 }
    //         );
    // }

    private TypeScriptFile CreateTypeScriptFileConfig(NameSpaceWithNodesAndFileName @namespace)
    {
        string fileName = @namespace.FileName;
        string nameSpace = @namespace.NameSpace.Name;
        string pathFromParentNamespace = @namespace.PathFromParentNamespace;
        var types = @namespace.Nodes.Select(c => c.MapToTypeScriptType(_config));
        // return new TypeScriptFileConfig
        // {
        //     NameSpace = nameSpace,
        //     File = new TypeScriptFile
        //     {
        //         FileName = fileName,
        //         PathFromParentNamespace = pathFromParentNamespace,
        //         Types = types
        //     }
        // };
        return new TypeScriptFile
        {
            FileName = fileName,
            PathFromParentNamespace = pathFromParentNamespace,
            Types = types
        };
    }

    private void WriteTsFile(FileStream fs, TypeScriptType typeScriptType)
    {
        var exportModel = GetExportModelType(typeScriptType.Name);
        fs.WriteLine(exportModel, false);

        foreach (var typeProperty in typeScriptType.Properties)
        {
            var property = $"{typeProperty.Name}: {typeProperty.Type}";
            fs.WriteLineWithTab(property, _endLinesWithSemicolon);
        }

        fs.WriteLine("}", false);
    }

    private string GetExportModelType(ExportModelType exportModelType, string typeName)
    {
        string exportTypeString = $"export type {typeName} = {{";
        string exportInterfaceString = $"export interface {typeName} {{";

        return exportModelType switch
        {
            ExportModelType.Type => exportTypeString,
            ExportModelType.Interface => exportInterfaceString,
            _
                => throw new ArgumentOutOfRangeException(
                    nameof(exportModelType),
                    exportModelType,
                    null
                )
        };
    }

    private string GetExportModelType(string typeName)
    {
        ExportModelType exportModelType = _config.ExportModelType;
        typeName = typeName.GetTypeName(_config);
        string exportTypeString = $"export type {typeName} = {{";
        string exportInterfaceString = $"export interface {typeName} {{";

        return exportModelType switch
        {
            ExportModelType.Type => exportTypeString,
            ExportModelType.Interface => exportInterfaceString,
            _
                => throw new ArgumentOutOfRangeException(
                    nameof(exportModelType),
                    exportModelType,
                    null
                )
        };
    }
}
