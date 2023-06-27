using System.Reflection;
using System.Text;
using AngleSharp;
using AngleSharp.Dom;
using Microsoft.CodeAnalysis.CSharp;
using Serilog;
using TypeForge.AspNetCore.Extensions;
using TypeForge.Core.Configuration;
using TypeForge.Core.Extensions;
using TypeForge.Core.Models;
using TypeForge.Core.StartUp;
using TypeForge.Core.Utils;

namespace TypeForge.AspNetCore.Services;

public class TypeForgeUiService
{
    private readonly TypeForgeConfig _config;

    private readonly ILogger _logger;
    private readonly bool _endLinesWithSemicolon;
    private readonly bool _generateIndexFile;
    private readonly bool _groupByNamespace;
    private readonly bool _nameSpaceInOneFile;

    // private readonly T
    public string Html { get; set; } = default!;
    private IDocument? _document;

    public TypeForgeUiService(TypeForgeConfig config)
    {
        _config = config;
        _endLinesWithSemicolon = _config.EndLinesWithSemicolon;
        _generateIndexFile = _config.GenerateIndexFile;
        _groupByNamespace = _config.GroupByNamespace;
        _nameSpaceInOneFile = _config.NameSpaceInOneFile;
        _logger = Log.Logger.ForContext<TypeForgeUiService>();
    }

    public string FetchHtmlFromDocument()
    {
        if (_document is null)
        {
            throw new NullReferenceException("Document is null");
        }

        return _document.DocumentElement.OuterHtml;
    }

    public async void WriteFromConfig()
    {
        IConfiguration config = AngleSharp.Configuration.Default;
        IBrowsingContext context = BrowsingContext.New(config);

        string html = await GetIndexHtml();
        IDocument document = await context.OpenAsync(req => req.Content(html));

        CSharpCompilation compilation = GetCompilation();
        var typeScriptFolders = GetTypeScriptFolders(compilation);
        var outputDir = Path.Combine(_config.ProjectDir, "output");
        Log.Information("Writing to {OutputDir}", outputDir);

        document = WriteAllFilesIntoHtml(document, typeScriptFolders);
        var updatedDoc = document.DocumentElement.OuterHtml;
        _logger.Information("{Html}", updatedDoc);
        _document = document;
    }

    private async Task<string> GetIndexHtml()
    {
        var assembly = Assembly.GetExecutingAssembly();
        ArgumentNullException.ThrowIfNull(assembly);
        string resourceName = $"{assembly.GetName().Name}.Ui.index.html";
        await using Stream stream = assembly.GetManifestResourceStream(resourceName)!;
        using StreamReader reader = new StreamReader(stream);
        return await reader.ReadToEndAsync();
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

    private IDocument WriteAllFilesIntoHtml(
        IDocument document,
        IEnumerable<TypeScriptFolder> typeScriptFolders
    )
    {
        var types = typeScriptFolders.SelectMany(x => x.Files).SelectMany(x => x.Types);
        IElement pre = document.CreateElement("pre");
        IElement code = document.CreateElement("code");
        code.ClassName = "language-typescript";
        foreach (TypeScriptType typeScriptType in types)
        {
            code = WriteTsInCodeBlock(code, typeScriptType);
            if (typeScriptType != types.Last())
            {
                code.TextContent += Environment.NewLine;
            }
        }
        pre.AppendChild(code);
        document.Body!.AppendChild(pre);
        return document;
    }

    private IElement WriteAllFilesIntoHtml(
        IElement div,
        IEnumerable<TypeScriptFolder> typeScriptFolders
    )
    {
        var types = typeScriptFolders.SelectMany(x => x.Files).SelectMany(x => x.Types);
        StringBuilder sb = new StringBuilder();
        foreach (TypeScriptType typeScriptType in types)
        {
            WriteTsInHtml(sb, typeScriptType);
            if (typeScriptType != types.Last())
            {
                sb.AppendLine();
            }
        }

        div.TextContent = sb.ToString();
        return div;
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
        var syntaxTrees = _config.NameSpaces.GetSyntaxTrees();
        return syntaxTrees.CreateCompilation();
    }

    private IEnumerable<TypeScriptFolder> GetTypeScriptFolders(CSharpCompilation compilation)
    {
        return _config.NameSpaces
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

    private IElement WriteTsInCodeBlock(IElement element, TypeScriptType typeScriptType)
    {
        var exportModel = GetExportModelType(typeScriptType.Name);
        var sb = new StringBuilder();
        sb.AppendLine(exportModel);
        foreach (var typeProperty in typeScriptType.Properties)
        {
            var property = $"{typeProperty.Name}: {typeProperty.Type}";
            sb.AppendLineWithTab(property, _endLinesWithSemicolon);
        }

        sb.AppendLine("}");

        element.TextContent += sb.ToString();

        return element;
    }

    private IElement WriteTsInMdBlock(IElement element, TypeScriptType typeScriptType)
    {
        var exportModel = GetExportModelType(typeScriptType.Name);
        var sb = new StringBuilder();
        sb.AppendLine($"```typescript");
        sb.AppendLine(exportModel);
        foreach (var typeProperty in typeScriptType.Properties)
        {
            var property = $"{typeProperty.Name}: {typeProperty.Type}";
            sb.AppendLineWithTab(property, _endLinesWithSemicolon);
        }

        sb.AppendLine("}");
        sb.AppendLine("```");

        element.TextContent = sb.ToString();

        return element;
    }

    private StringBuilder WriteTsInHtml(StringBuilder sb, TypeScriptType typeScriptType)
    {
        var exportModel = GetExportModelType(typeScriptType.Name);

        sb.AppendLine($"<script type=\"text/typescript\">");
        sb.AppendLine(exportModel);
        // fs.WriteLine(exportModel, false);

        foreach (var typeProperty in typeScriptType.Properties)
        {
            var property = $"{typeProperty.Name}: {typeProperty.Type}";
            sb.AppendLine(property);
            // sb.AppendLineWithTab(property, _endLinesWithSemicolon);
            // fs.WriteLineWithTab(property, _endLinesWithSemicolon);
        }
        sb.AppendLine("}");
        sb.AppendLine("</script>");
        return sb;
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
