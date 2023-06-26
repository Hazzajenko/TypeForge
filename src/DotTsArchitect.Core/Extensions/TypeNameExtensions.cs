using System.Text.Json;
using System.Text.RegularExpressions;
using DotTsArchitect.Core.Configuration;

namespace DotTsArchitect.Core.Extensions;

public static class TypeNameExtensions
{
    public static string GetTypeName(this string typeName, GlobalConfig config)
    {
        var typeNamePrefix = config.TypeNamePrefix;
        var typeNameSuffix = config.TypeNameSuffix;
        TypeNameCase typeNameCase = config.TypeNameCase;
        return typeName
            .AddPrefixAndSuffix(typeNamePrefix, typeNameSuffix)
            .ToCaseOfOption(typeNameCase);
    }

    // public static string GetTypeName(
    //     this string typeName,
    //     string? typeNamePrefix = null,
    //     string? typeNameSuffix = null
    // )
    // {
    //     return typeName.AddPrefixAndSuffix(typeNamePrefix, typeNameSuffix);
    // }

    private static string AddPrefixAndSuffix(
        this string typeName,
        string? prefix = null,
        string? suffix = null
    )
    {
        if (prefix is not null)
        {
            typeName = $"{prefix}{typeName}".ToCaseOfOption(PropertyNameCase.CamelCase);
        }

        if (suffix is not null)
        {
            typeName = $"{typeName}{suffix}".ToCaseOfOption(PropertyNameCase.CamelCase);
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
}
