using System.Runtime.InteropServices.ComTypes;
using System.Text;
using DotTsArchitect.Core.Configuration;
using DotTsArchitect.Core.Extensions;
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

    public WriterService(IOptions<GlobalConfig> config)
    {
        _config = config.Value;
    }

    public void WriteFromConfig()
    {
        foreach (var @namespace in _config.Namespaces)
        {
            var @classes = @namespace.GetClassDeclarationsForNamespace();
            // @classes.DumpObjectJson();
            foreach (var namespaceWithNodes in @classes)
            {
                Log.Logger.Information(
                    "Writing namespace {Namespace}, {Count} classes",
                    namespaceWithNodes.Namespace.Name,
                    namespaceWithNodes.Nodes.Count()
                );
                WriteTsFilesForNamespace(namespaceWithNodes.Namespace, namespaceWithNodes.Nodes);
            }
            // request.
        }
    }

    private void WriteTsFilesForNamespace(
        ConfigNameSpaceWithPath @namespace,
        IEnumerable<ClassDeclarationSyntax> classes
    )
    {
        string file = $"{@namespace.Path}\\Generated\\{@namespace.Name}\\{@namespace.Name}.ts";
        // string file = $"{@namespace.Path}\\{@namespacee.Name}.ts";
        FileInfo componentTemplateFile = new FileInfo(file);
        using FileStream fs = componentTemplateFile.CreateFileSafe();
        foreach (var classDeclarationSyntax in classes)
        {
            var typeName = classDeclarationSyntax.Identifier.Text;
            var exportModel = GetExportModelType(_config.ExportModelType, typeName);
            fs.WriteLine(exportModel);

            foreach (
                var property in classDeclarationSyntax
                    .DescendantNodes()
                    .OfType<PropertyDeclarationSyntax>()
            )
            {
                string name = property.Identifier.ValueText.ToCaseOfOption(
                    _config.PropertyNameCase
                );
                // _config.Compilation.SyntaxTrees.
                var comp = _config.Compilation.AddSyntaxTrees(classDeclarationSyntax.SyntaxTree);
                var semanticModel = comp.GetSemanticModel(classDeclarationSyntax.SyntaxTree);
                var typeSymbol = semanticModel.GetTypeInfo(property.Type).Type;
                var typeSymbolName = typeSymbol!.Name;
                typeSymbolName.Log();
                string type = typeSymbolName.Convert();
                fs.WriteLine($"\t{name}: {type};");
            }

            fs.WriteLine("}");
        }
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
        fs.WriteLine(exportModel);

        foreach (var property in request.Type.GetProperties())
        {
            string name = property.Name.ToCaseOfOption(request.Options.PropertyNameCase);
            string type = property.PropertyType.Convert();
            fs.WriteLine($"\t{name}: {type};");
        }

        fs.WriteLine("}");
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
        fs.WriteLine(exportModel);

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
            fs.WriteLine($"\t{name}: {type};");
        }

        fs.WriteLine("}");
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
}
