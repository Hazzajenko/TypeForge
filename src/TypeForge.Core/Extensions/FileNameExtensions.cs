using System.Text.Json;
using System.Text.RegularExpressions;
using TypeForge.Core.Configuration;
using TypeForge.Core.Models;

namespace TypeForge.Core.Extensions;

public static class FileNameExtensions
{
    public static string GetFileName(this string fileName, TypeForgeConfig config)
    {
        var fileNameCase = config.FileNameCase;
        var fileNamePrefix = config.TypeNamePrefix;
        var fileNameSuffix = config.TypeNameSuffix;
        return fileName
            .RemoveCsExtension()
            .ToCaseOfOption(fileNameCase)
            .AddPrefixAndSuffix(fileNameCase, fileNamePrefix, fileNameSuffix)
            .AddTsExtension();
    }

    public static string GetFileName(this string fileName, InputGlobalConfig config)
    {
        var fileNameCase = config.FileNameCase;
        var fileNamePrefix = config.TypeNamePrefix;
        var fileNameSuffix = config.TypeNameSuffix;
        return fileName
            .RemoveCsExtension()
            .ToCaseOfOption(fileNameCase)
            .AddPrefixAndSuffix(fileNameCase, fileNamePrefix, fileNameSuffix)
            .AddTsExtension();
    }

    public static string GetFileName(
        this string fileName,
        FileNameCase option,
        string? fileNamePrefix = null,
        string? fileNameSuffix = null
    )
    {
        return fileName
            .RemoveCsExtension()
            .ToCaseOfOption(option)
            .AddPrefixAndSuffix(option, fileNamePrefix, fileNameSuffix)
            .AddTsExtension();
    }

    private static string RemoveCsExtension(this string fileName)
    {
        return fileName.Replace(".cs", "");
    }

    private static string AddPrefixAndSuffix(
        this string fileName,
        FileNameCase fileNameCase,
        string? prefix = null,
        string? suffix = null
    )
    {
        if (prefix is not null)
        {
            fileName = $"{prefix}{fileName}".ToCaseOfOption(fileNameCase);
        }

        if (suffix is not null)
        {
            fileName = $"{fileName}.{suffix.ToCaseOfOption(fileNameCase)}";
        }

        return fileName;
    }

    public static string ToCaseOfOption(this string s, FileNameCase option)
    {
        return option switch
        {
            FileNameCase.CamelCase => s.ToCamelCase(),
            FileNameCase.PascalCase => s.ToPascalCase(),
            FileNameCase.KebabCase => s.ToKebabCase(),
            _ => throw new ArgumentOutOfRangeException(nameof(option), option, null)
        };
    }

    public static string ToCaseOfOption(this string s, FolderNameCase option)
    {
        return option switch
        {
            FolderNameCase.CamelCase => s.ToCamelCase(),
            FolderNameCase.PascalCase => s.ToPascalCase(),
            FolderNameCase.KebabCase => s.ToKebabCase(),
            _ => throw new ArgumentOutOfRangeException(nameof(option), option, null)
        };
    }

    private static string AddTsExtension(this string s)
    {
        return $"{s}.ts";
    }

    public static string GetFileNameFromPath(this string path)
    {
        var split = path.Split("\\");
        var fileName = split[^1];
        return fileName;
    }
}
