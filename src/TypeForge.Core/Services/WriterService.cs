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
    private readonly bool _endLinesWithSemicolon;
    private readonly bool _generateIndexFile;
    private readonly bool _groupByNamespace;
    private readonly bool _nameSpaceInOneFile;

    public WriterService(TypeForgeConfig config)
    {
        _config = config;
        _endLinesWithSemicolon = config.EndLinesWithSemicolon;
        _generateIndexFile = config.GenerateIndexFile;
        _groupByNamespace = config.GroupByNamespace;
        _nameSpaceInOneFile = config.NameSpaceInOneFile;
    }

    public void WriteFromConfig()
    {
        CSharpCompilation compilation = _config.NameSpaces.GetCSharpCompilation();
        var typeScriptFolders = _config.NameSpaces.GetTypeScriptFolders(compilation, _config);
        var outputDir = Path.Combine(_config.ProjectDir, "output");
        Log.Information("Writing to {OutputDir}", outputDir);

        if (_nameSpaceInOneFile)
        {
            WriteAllFilesIntoOneFile(typeScriptFolders, compilation);
            return;
        }

        foreach (TypeScriptFolder typeScriptFolder in typeScriptFolders)
        {
            WriteTypeScriptFolder(typeScriptFolder, outputDir);
        }

        if (_generateIndexFile)
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
        if (_groupByNamespace is false)
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

    private void WriteAllFilesIntoOneFile(
        IEnumerable<TypeScriptFolder> typeScriptFolders,
        CSharpCompilation compilation
    )
    {
        var outputDir = Path.Combine(_config.ProjectDir, "output\\");
        var fileInfo = new FileInfo($"{outputDir}all.ts");
        using FileStream fs = fileInfo.CreateFileSafe();
        var types = typeScriptFolders.GetTypesFromFolders();
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

    private void WriteTsFile(FileStream fs, TypeScriptType typeScriptType)
    {
        var exportModel = typeScriptType.GetExportModelType(_config);
        fs.WriteLine(exportModel, false);

        foreach (var typeProperty in typeScriptType.Properties)
        {
            var property = $"{typeProperty.Name}: {typeProperty.Type}";
            fs.WriteLineWithTab(property, _endLinesWithSemicolon);
        }

        fs.WriteLine("}", false);
    }
}
