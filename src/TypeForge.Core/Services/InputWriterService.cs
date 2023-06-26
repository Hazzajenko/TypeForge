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

public class InputWriterService
{
    private readonly InputGlobalConfig _config;
    private readonly bool _endLinesWithSemicolon;

    public InputWriterService(InputGlobalConfig config)
    {
        _config = config;
        _endLinesWithSemicolon = config.EndLinesWithSemicolon;
    }

    public void Generate()
    {
        CSharpCompilation compilation = GetCompilation();
        var typeScriptFolders = GetTypeScriptFolders(compilation);
        var outputDir = _config.Output;
        Log.Information("Writing to {OutputDir}", outputDir);

        if (_config.NameSpaceInOneFile)
        {
            WriteAllFilesIntoOneFile(typeScriptFolders);
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

    private void WriteAllFilesIntoOneFile(IEnumerable<TypeScriptFolder> typeScriptFolders)
    {
        // var typeScriptFolders = GetTypeScriptFolders();
        var outputDir = _config.Output;
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

    private CSharpCompilation GetCompilation()
    {
        var syntaxTrees = _config.Directory.GetSyntaxTrees();
        return syntaxTrees.CreateCompilation();
    }

    private IEnumerable<TypeScriptFolder> GetTypeScriptFolders(CSharpCompilation compilation)
    {
        return _config.Directory
            .GetTypeScriptFilesForDirectory(_config, compilation)
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

    private string GetExportModelType(TypeModel typeModel, string typeName)
    {
        string exportTypeString = $"export type {typeName} = {{";
        string exportInterfaceString = $"export interface {typeName} {{";

        return typeModel switch
        {
            TypeModel.Type => exportTypeString,
            TypeModel.Interface => exportInterfaceString,
            _ => throw new ArgumentOutOfRangeException(nameof(typeModel), typeModel, null)
        };
    }

    private string GetExportModelType(string typeName)
    {
        TypeModel typeModel = _config.TypeModel;
        typeName = typeName.GetTypeName(_config);
        string exportTypeString = $"export type {typeName} = {{";
        string exportInterfaceString = $"export interface {typeName} {{";

        return typeModel switch
        {
            TypeModel.Type => exportTypeString,
            TypeModel.Interface => exportInterfaceString,
            _ => throw new ArgumentOutOfRangeException(nameof(typeModel), typeModel, null)
        };
    }
}
