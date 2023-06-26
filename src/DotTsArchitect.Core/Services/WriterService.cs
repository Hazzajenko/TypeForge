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
        // _config = config;
    }

    public void WriteFromConfig()
    {
        var data = GetNamespaceData();
        var grouped = GetNamespaceDataGrouped();
        grouped.DumpObjectJson();
        var outputDir = _config.ProjectDir + "\\output";

        foreach (var typeScriptFolder in grouped)
        {
            WriteTypeScriptFolder(typeScriptFolder, outputDir);
        }
        //
        // foreach (var typeScriptFile in data)
        // {
        //     WriteTypeScriptFile(typeScriptFile, outputDir);
        // }
    }

    private void WriteTypeScriptFolder(TypeScriptFolder typeScriptFolder, string outputDir)
    {
        string path = outputDir + "\\" + typeScriptFolder.FolderName;
        foreach (var typeScriptFile in typeScriptFolder.Files)
        {
            WriteTypeScriptFile(typeScriptFile, outputDir);
        }

        if (_config.GenerateIndexFile)
        {
            string indexFile = $"{path}index.ts";
            FileInfo indexTemplateFile = new FileInfo(indexFile);
            using FileStream indexFs = indexTemplateFile.CreateFileSafe();
            foreach (var typeScriptFile in typeScriptFolder.Files)
            {
                string fileName = typeScriptFile.FileName.TakeOutTsExtension();
                string exportModel = $"export * from './{fileName}'";
                indexFs.WriteLine(exportModel, _endLinesWithSemicolon);
            }
        }
    }

    private void WriteTypeScriptFile(TypeScriptFile typeScriptFile, string outputDir)
    {
        Log.Logger.Information(
            "Writing file {FileName}, Path From Namespace: {Path}, {Count} types",
            typeScriptFile.FileName,
            typeScriptFile.PathFromParentNamespace,
            typeScriptFile.Types.Count()
        );

        string path = outputDir + "\\" + typeScriptFile.PathFromParentNamespace;
        Log.Logger.Information("Creating directory {Path}", path);

        FileInfo fileInfo = new FileInfo($"{path}{typeScriptFile.FileName}");
        using FileStream fs = fileInfo.CreateFileSafe();
        foreach (var typeScriptType in typeScriptFile.Types)
        {
            WriteTsFile(fs, typeScriptType);
            if (typeScriptType != typeScriptFile.Types.Last())
            {
                fs.WriteEmptyLine();
            }
        }
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

    private void WriteTsFilesForNamespace(NamespaceWithNodesAndFileName @namespace)
    {
        string fileName = @namespace.FileName.GetFileName(_config);
        string file =
            $"{@namespace.Namespace.Path}\\Generated\\{@namespace.PathFromParentNamespace}\\{fileName}";
        var classes = @namespace.Nodes;
        FileInfo componentTemplateFile = new FileInfo(file);
        using FileStream fs = componentTemplateFile.CreateFileSafe();
        foreach (var classDeclarationSyntax in classes)
        {
            WriteTsFile(fs, classDeclarationSyntax);
            if (classDeclarationSyntax != classes.Last())
            {
                fs.WriteEmptyLine();
            }
        }

        if (_config.GenerateIndexFile)
        {
            string indexFile =
                $"{@namespace.Namespace.Path}\\Generated\\{@namespace.PathFromParentNamespace}\\index.ts";
            FileInfo indexTemplateFile = new FileInfo(indexFile);
            using FileStream indexFs = indexTemplateFile.CreateFileSafe();
            foreach (var classDeclarationSyntax in classes)
            {
                string typeName = classDeclarationSyntax.Identifier.Text.GetTypeName(_config);
                string exportModel = $"export * from './{typeName}'";
                indexFs.WriteLine(exportModel, _endLinesWithSemicolon);
            }
        }
    }

    private void WriteTsFile(FileStream fs, TypeScriptType typeScriptType)
    {
        var exportModel = GetExportModelType(_config.ExportModelType, typeScriptType.Name);
        fs.WriteLine(exportModel, false);

        foreach (var typeProperty in typeScriptType.Properties)
        {
            var property = $"{typeProperty.Name}: {typeProperty.Type};";
            fs.WriteLineWithTab(property, _endLinesWithSemicolon);
        }

        fs.WriteLine("}", false);
    }

    private void WriteTsFile(FileStream fs, ClassDeclarationSyntax classDeclarationSyntax)
    {
        var typeName = classDeclarationSyntax.Identifier.Text.GetTypeName(_config);
        var exportModel = GetExportModelType(_config.ExportModelType, typeName);
        fs.WriteLine(exportModel, false);

        foreach (
            var property in classDeclarationSyntax
                .DescendantNodes()
                .OfType<PropertyDeclarationSyntax>()
        )
        {
            string name = property.Identifier.ValueText.ToCaseOfOption(_config.PropertyNameCase);
            var comp = _config.Compilation.AddSyntaxTrees(classDeclarationSyntax.SyntaxTree);
            var semanticModel = comp.GetSemanticModel(classDeclarationSyntax.SyntaxTree);
            var typeSymbol = semanticModel.GetTypeInfo(property.Type).Type;
            var typeSymbolName = typeSymbol!.Name;
            string type = typeSymbolName.Convert();
            fs.WriteLine($"\t{name}: {type};", _endLinesWithSemicolon);
        }

        fs.WriteLine("}", false);
    }

    public void Write(WriteRequest request)
    {
        string path = request.Options.Path;
        string fileName = request.Type.Name.ToCaseOfOption(request.Options.FileNameCase);
        string file = $"{path}\\{fileName}.ts";
        FileInfo componentTemplateFile = new FileInfo(file);
        using FileStream fs = componentTemplateFile.Create();
        var typeName = request.Type.Name;
        var exportModel = GetExportModelType(request.Options.ExportModelType, typeName);
        fs.WriteLine(exportModel, false);

        foreach (var property in request.Type.GetProperties())
        {
            string name = property.Name.ToCaseOfOption(request.Options.PropertyNameCase);
            string type = property.PropertyType.Convert();
            fs.WriteLine($"\t{name}: {type};", _endLinesWithSemicolon);
        }

        fs.WriteLine("}", false);
    }

    // public void WriteFile(ClassDeclarationSyntax @class) { }

    public void WriteNamespaceRequest(WriteNamespaceRequest request)
    {
        string path = request.Options.Path;
        string fileName = request.Type.Identifier.Text.ToCaseOfOption(request.Options.FileNameCase);
        // string fileName = request.Type.Name.ToCaseOfOption(request.Options.FileNameCase);
        string file = $"{path}\\{fileName}.ts";
        FileInfo componentTemplateFile = new FileInfo(file);
        using FileStream fs = componentTemplateFile.Create();
        var typeName = request.Type.Identifier.Text;
        var exportModel = GetExportModelType(request.Options.ExportModelType, typeName);
        fs.WriteLine(exportModel, false);

        foreach (var property in request.Type.DescendantNodes().OfType<PropertyDeclarationSyntax>())
        {
            string name = property.Identifier.ValueText.ToCaseOfOption(
                request.Options.PropertyNameCase
            );
            var semanticModel = _config.Compilation.GetSemanticModel(request.Type.SyntaxTree);
            var typeSymbol = semanticModel.GetTypeInfo(property.Type).Type;
            var typeSymbolName = typeSymbol!.Name;
            typeSymbolName.Log();
            string type = typeSymbolName.Convert();
            fs.WriteLine($"\t{name}: {type};", _endLinesWithSemicolon);
        }

        fs.WriteLine("}", false);
    }

    // private void WriteLine(FileStream fileStream, string content)
    // {
    //     Byte[] txt = new UTF8Encoding(true).GetBytes(content + Environment.NewLine);
    //     fileStream.Write(txt, 0, txt.Length);
    // }

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

    private string GetTypeFromCompilation(
        ClassDeclarationSyntax typeSyntax,
        PropertyDeclarationSyntax property
    )
    {
        var comp = _config.Compilation.AddSyntaxTrees(typeSyntax.SyntaxTree);
        var semanticModel = comp.GetSemanticModel(typeSyntax.SyntaxTree);
        var typeSymbol = semanticModel.GetTypeInfo(property.Type).Type;
        var typeSymbolName = typeSymbol!.Name;
        return typeSymbolName.Convert();
    }
}
// var comp = _config.Compilation.AddSyntaxTrees(classDeclarationSyntax.SyntaxTree);
// var semanticModel = comp.GetSemanticModel(classDeclarationSyntax.SyntaxTree);
// var typeSymbol = semanticModel.GetTypeInfo(property.Type).Type;
// var typeSymbolName = typeSymbol!.Name;
// string type = typeSymbolName.Convert();
