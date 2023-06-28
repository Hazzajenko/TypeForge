using McMaster.Extensions.CommandLineUtils;
using Serilog;
using TypeForge.Cli.Mapping;
using TypeForge.Core.Configuration;
using TypeForge.Core.Extensions;
using TypeForge.Core.Mapping;
using TypeForge.Core.Services;

namespace TypeForge.Cli;

[Command(Name = "forge", Description = "Generate TypeScript types from C# types")]
[HelpOption("-?|-h|--help")]
public class TypeForgeOptions
{
    public void OnExecute()
    {
        if (Directory is null || Output is null)
        {
            Log.Logger.Information("Using config");
            HandleUseConfig();
            return;
        }
        var globalConfig = this.ToTypeForgeConfig();
        this.DumpObjectJson();
        var writerService = new WriterService(globalConfig);
        writerService.WriteFromConfig();
    }

    private static void HandleUseConfig()
    {
        string projectDir = GetProjectDirectory();
        ConfigFileType configFileType = projectDir.GetConfigFileTypeIfExist();
        Log.Logger.Information("Config file type: {ConfigFileType}", configFileType);
        if (configFileType is ConfigFileType.None)
        {
            throw new Exception("Config file not found");
        }

        ConfigFile config = projectDir.GetConfigFile(configFileType);

        if (config.NameSpaces is null)
        {
            throw new Exception("Config file namespaces is null");
        }
        var globalConfig = config.ToTypeForgeConfig(projectDir);
        var writer = new WriterService(globalConfig);
        writer.WriteFromConfig();
    }

    [Argument(
        0,
        Name = "directory",
        Description = "Directory of files to convert. Leave empty to use config."
    )]
    public string? Directory { get; set; } = default!;

    [Argument(
        1,
        Name = "output",
        Description = "Directory to output files to. Leave empty to use config."
    )]
    public string? Output { get; set; } = default!;

    // * File
    [Option(
        ShortName = "fc",
        LongName = "fileNameCase",
        Description = "File name case. Default = KebabCase. Example = kebab-case"
    )]
    public string FileNameCase { get; set; } = "KebabCase";

    [Option(
        ShortName = "fp",
        LongName = "fileNamePrefix",
        Description = "File name prefix, Default = null. Example = I"
    )]
    public string? FileNamePrefix { get; set; }

    [Option(
        ShortName = "fs",
        LongName = "fileNameSuffix",
        Description = "File name suffix, Default = null. Example = Model"
    )]
    public string? FileNameSuffix { get; set; }

    // * Folder
    [Option(
        ShortName = "fd",
        LongName = "folderNameCase",
        Description = "Folder name case, Default = KebabCase"
    )]
    public string FolderNameCase { get; set; } = "KebabCase";

    // * Type
    [Option(
        ShortName = "tm",
        LongName = "typeModel",
        Description = "Type or Interface. Default = Type"
    )]
    public string TypeModel { get; set; } = "Type";

    [Option(
        ShortName = "tc",
        LongName = "typeNameCase",
        Description = "Type name case. Default = PascalCase"
    )]
    public string TypeNameCase { get; set; } = "PascalCase";

    [Option(
        ShortName = "tp",
        LongName = "typeNamePrefix",
        Description = "Type name prefix. Default = null. Example = I"
    )]
    public string? TypeNamePrefix { get; set; }

    [Option(
        ShortName = "ts",
        LongName = "typeNameSuffix",
        Description = "Type name suffix. Default = null. Example = Model"
    )]
    public string? TypeNameSuffix { get; set; }

    // * Property
    [Option(
        ShortName = "pc",
        LongName = "propertyNameCase",
        Description = "Property name case. Default = CamelCase"
    )]
    public string PropertyNameCase { get; set; } = "CamelCase";

    // * Other Options
    [Option(
        ShortName = "nt",
        LongName = "nullableType",
        Description = "Nullable type. Default = QuestionMark. QuestionMark or Null Or Undefined"
    )]
    public string NullableType { get; set; } = "QuestionMark";

    [Option(
        ShortName = "gi",
        LongName = "generateIndexFile",
        Description = "Generate a index file to export all generated files. Default = true"
    )]
    public bool GenerateIndexFile { get; set; } = true;

    [Option(ShortName = "gn", LongName = "groupByNamespace", Description = "Group by namespace.")]
    public bool GroupByNameSpace { get; set; } = true;

    [Option(
        ShortName = "nf",
        LongName = "nameSpaceInOneFile",
        Description = "Namespace in one file. Default = false"
    )]
    public bool NameSpaceInOneFile { get; set; }

    [Option(
        ShortName = "es",
        LongName = "endLinesWithSemicolon",
        Description = "End lines with semicolon. Default = false"
    )]
    public bool EndLinesWithSemicolon { get; set; }
}
