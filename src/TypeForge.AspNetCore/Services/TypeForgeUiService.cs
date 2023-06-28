using System.Reflection;
using System.Text;
using AngleSharp;
using AngleSharp.Dom;
using Microsoft.CodeAnalysis.CSharp;
using Serilog;
using TypeForge.AspNetCore.Extensions;
using TypeForge.Core.Configuration.TypeForgeConfig;
using TypeForge.Core.Extensions;
using TypeForge.Core.Mapping;
using TypeForge.Core.Models;

namespace TypeForge.AspNetCore.Services;

public class TypeForgeUiService
{
    private readonly TypeForgeConfig _config;

    private readonly ILogger _logger;
    private readonly bool _endLinesWithSemicolon;
    private readonly bool _generateIndexFile;
    private IDocument? _document;

    public TypeForgeUiService(TypeForgeConfig config)
    {
        _config = config;
        _endLinesWithSemicolon = _config.EndLinesWithSemicolon;
        _generateIndexFile = _config.GenerateIndexFile;
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

        string html = await GetInitialIndexHtml();
        IDocument document = await context.OpenAsync(req => req.Content(html));

        CSharpCompilation compilation = _config.ConfigDirectories.GetCSharpCompilation();
        var typeScriptFolders = _config.ConfigDirectories.GetTypeScriptFolders(
            compilation,
            _config
        );
        var outputDir = Path.Combine(_config.ProjectDir, "output");
        Log.Information("Writing to {OutputDir}", outputDir);

        document = WriteAllFilesIntoHtmlV2(document, typeScriptFolders);

        // document = _groupByNamespace
        //     ? WriteTypeScriptFilesInGroups(document, typeScriptFolders)
        //     : WriteAllFilesIntoHtmlV2(document, typeScriptFolders);
        var updatedDoc = document.DocumentElement.OuterHtml;
        _logger.Information("{Html}", updatedDoc);
        _document = document;
    }

    private async Task<string> GetInitialIndexHtml()
    {
        var assembly = Assembly.GetExecutingAssembly();
        ArgumentNullException.ThrowIfNull(assembly);
        string resourceName = $"{assembly.GetName().Name}.Ui.index.html";
        await using Stream stream = assembly.GetManifestResourceStream(resourceName)!;
        using StreamReader reader = new StreamReader(stream);
        return await reader.ReadToEndAsync();
    }

    public IDocument WriteTypeScriptFilesInGroups(
        IDocument document,
        IEnumerable<TypeScriptFolder> typeScriptFolders
    )
    {
        return typeScriptFolders.Aggregate(document, WriteTypeScriptFolder);
    }

    private IDocument WriteTypeScriptFolder(IDocument document, TypeScriptFolder typeScriptFolder)
    {
        return typeScriptFolder.Files.Aggregate(document, WriteTypeScriptFile);
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

    private IDocument WriteAllFilesIntoHtmlV2(
        IDocument document,
        IEnumerable<TypeScriptFolder> typeScriptFolders
    )
    {
        for (int folderIndex = 0; folderIndex < typeScriptFolders.Count(); folderIndex++)
        {
            TypeScriptFolder folder = typeScriptFolders.ElementAt(folderIndex);
            IElement div = document.CreateElement("div");
            div.TextContent = folder.FolderName;
            IElement pre = document.CreateElement("pre");
            IElement code = document.CreateElement("code");
            for (int fileIndex = 0; fileIndex < folder.Files.Count(); fileIndex++)
            {
                TypeScriptFile file = folder.Files.ElementAt(fileIndex);
                for (int typeIndex = 0; typeIndex < file.Types.Count(); typeIndex++)
                {
                    TypeScriptType type = file.Types.ElementAt(typeIndex);
                    code.ClassName = "language-typescript";
                    code = WriteTsInCodeBlock(code, type);
                    pre.AppendChild(code);
                }
            }
            pre.AppendChild(div);
            pre.AppendChild(code);
            document.Body!.AppendChild(pre);
        }
        return document;
    }

    private IDocument WriteTypeScriptFile(IDocument document, TypeScriptFile typeScriptFile)
    {
        IElement div = document.CreateElement("div");
        div.TextContent = typeScriptFile.FileName;
        div.ClassName = "file-name";
        div.Id = typeScriptFile.FileName;
        IElement pre = document.CreateElement("pre");
        IElement code = document.CreateElement("code");
        code.ClassName = "language-typescript";
        for (int typeIndex = 0; typeIndex < typeScriptFile.Types.Count(); typeIndex++)
        {
            var typeScriptType = typeScriptFile.Types.ElementAt(typeIndex);
            code = WriteTsInCodeBlock(code, typeScriptType);
            if (typeIndex != typeScriptFile.Types.Count() - 1)
            {
                code.TextContent += Environment.NewLine;
            }
        }
        pre.AppendChild(div);
        pre.AppendChild(code);
        document.Body!.AppendChild(pre);
        return document;
    }

    private IElement WriteTsInCodeBlock(IElement element, TypeScriptType typeScriptType)
    {
        var exportModel = typeScriptType.GetExportModelType(_config);
        // var exportModel = GetExportModelType(typeScriptType.Name);
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
        var exportModel = typeScriptType.GetExportModelType(_config);
        // var exportModel = GetExportModelType(typeScriptType.Name);
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
        var exportModel = typeScriptType.GetExportModelType(_config);
        // var exportModel = GetExportModelType(typeScriptType.Name);

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
        var exportModel = typeScriptType.GetExportModelType(_config);
        // var exportModel = GetExportModelType(typeScriptType.Name);
        fs.WriteLine(exportModel, false);

        foreach (var typeProperty in typeScriptType.Properties)
        {
            var property = $"{typeProperty.Name}: {typeProperty.Type}";
            fs.WriteLineWithTab(property, _endLinesWithSemicolon);
        }

        fs.WriteLine("}", false);
    }
    //
    // private string GetExportModelType(string typeName)
    // {
    //     TypeModel typeModel = _config.TypeModel;
    //     typeName = typeName.GetTypeName(_config);
    //     string exportTypeString = $"export type {typeName} = {{";
    //     string exportInterfaceString = $"export interface {typeName} {{";
    //
    //     return typeModel switch
    //     {
    //         TypeModel.Type => exportTypeString,
    //         TypeModel.Interface => exportInterfaceString,
    //         _ => throw new ArgumentOutOfRangeException(nameof(typeModel), typeModel, null)
    //     };
    // }
}
