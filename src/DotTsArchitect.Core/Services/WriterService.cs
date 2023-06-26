using System.Runtime.InteropServices.ComTypes;
using System.Text;
using DotTsArchitect.Core.Configuration;
using DotTsArchitect.Core.Extensions;
using DotTsArchitect.Core.Mapping;
using DotTsArchitect.Core.Models;
using DotTsArchitect.Core.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Options;
using Serilog;

namespace DotTsArchitect.Core.Services;

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
        var outputDir = Path.Combine(_config.ProjectDir, "output\\");
        Log.Information("Writing to {OutputDir}", outputDir);
        foreach (TypeScriptFolder typeScriptFolder in typeScriptFolders)
        {
            WriteTypeScriptFolder(typeScriptFolder, outputDir);
        }

        if (_config is not { GenerateIndexFile: true, GroupByNamespace: false })
            return;

        WriteExportsForIndexFile(typeScriptFolders, outputDir);
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
        // string indexFile = $"{outputDir}index.ts";
        var indexTemplateFile = new FileInfo(indexFile);
        using FileStream indexFs = indexTemplateFile.CreateFileSafe();

        foreach (TypeScriptFile typeScriptFile in typeScriptFiles)
        {
            string fileName = typeScriptFile.FileName.TakeOutTsExtension();
            string exportModel = $"export * from './{fileName}'";
            indexFs.WriteLine(exportModel, _endLinesWithSemicolon);
        }
    }

    private void WriteTypeScriptFolder(TypeScriptFolder typeScriptFolder, string outputDir)
    {
        var path = Path.Combine(outputDir, typeScriptFolder.FolderName.TrimStart('\\'));
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

    private void WriteTypeScriptFile(TypeScriptFile typeScriptFile, string outputDir)
    {
        Log.Logger.Information(
            "Writing file {FileName}, Path From Namespace: {Path}, {Count} types",
            typeScriptFile.FileName,
            typeScriptFile.PathFromParentNamespace,
            typeScriptFile.Types.Count()
        );

        Log.Logger.Information("Creating directory {Path}", outputDir);

        var fileInfo = new FileInfo($"{outputDir}{typeScriptFile.FileName}");
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

    private static string GetExportStatement(TypeScriptFile file)
    {
        string fileName = file.FileName.TakeOutTsExtension();
        return $"export * from './{fileName}'";
    }

    private IEnumerable<TypeScriptFile> GetNamespaceData()
    {
        return _config.Namespaces.SelectMany(configNameSpaceWithPath =>
        {
            var @classes = configNameSpaceWithPath.GetClassDeclarationsForNamespace(_config);
            return @classes.Select(CreateTypeScriptFileConfig);
        });
    }

    private IEnumerable<TypeScriptFolder> GetNamespaceDataGrouped()
    {
        return _config.Namespaces
            .SelectMany(configNameSpaceWithPath =>
            {
                var @classes = configNameSpaceWithPath.GetClassDeclarationsForNamespace(_config);
                return @classes.Select(CreateTypeScriptFileConfig);
            })
            .GroupBy(x => x.PathFromParentNamespace)
            .Select(x => new TypeScriptFolder { FolderName = x.Key, Files = x.ToList() });
    }

    private TypeScriptFile CreateTypeScriptFileConfig(NamespaceWithNodesAndFileName @namespace)
    {
        string fileName = @namespace.FileName;
        string pathFromParentNamespace = @namespace.PathFromParentNamespace;
        var types = @namespace.Nodes.Select(c => c.MapToTypeScriptType(_config));
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
