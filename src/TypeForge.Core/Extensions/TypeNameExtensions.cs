using System.Text.Json;
using System.Text.RegularExpressions;
using TypeForge.Core.Configuration;
using TypeForge.Core.Configuration.TypeForgeConfig;
using TypeForge.Core.Models;

namespace TypeForge.Core.Extensions;

public static class TypeNameExtensions
{
    public static string GetTypeName(this string typeName, TypeForgeConfig config)
    {
        var typeNamePrefix = config.TypeNamePrefix;
        var typeNameSuffix = config.TypeNameSuffix;
        TypeNameCase typeNameCase = config.TypeNameCase;
        return typeName
            .AddPrefixAndSuffix(typeNamePrefix, typeNameSuffix)
            .ToCaseOfOption(typeNameCase);
    }

    public static string AddPrefixAndSuffix(
        this string typeName,
        string? prefix = null,
        string? suffix = null,
        PropertyNameCase option = PropertyNameCase.CamelCase
    )
    {
        if (prefix is not null)
        {
            typeName = $"{prefix}{typeName}".ToCaseOfOption(option);
        }

        if (suffix is not null)
        {
            typeName = $"{typeName}{suffix}".ToCaseOfOption(option);
        }

        return typeName;
    }

    public static string ToCaseOfOption(this string s, TypeNameCase option)
    {
        return option switch
        {
            TypeNameCase.CamelCase => s.ToCamelCase(),
            TypeNameCase.PascalCase => s.ToPascalCase(),
            _ => throw new ArgumentOutOfRangeException(nameof(option), option, null)
        };
    }

    public static string ToCaseOfOption(this string s, PropertyNameCase option)
    {
        return option switch
        {
            PropertyNameCase.CamelCase => s.ToCamelCase(),
            PropertyNameCase.PascalCase => s.ToPascalCase(),
            _ => throw new ArgumentOutOfRangeException(nameof(option), option, null)
        };
    }

    public static string GetExportStatement(this TypeScriptFile file)
    {
        string fileName = file.FileName.TakeOutTsExtension();
        return $"export * from './{fileName}'";
    }

    public static string GetExportModelType(
        this TypeScriptType typeScriptType,
        TypeForgeConfig config
    )
    {
        TypeModel typeModel = config.TypeModel;
        var typeName = typeScriptType.Name.GetTypeName(config);
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
