using System.Text.Json;
using System.Text.RegularExpressions;
using CaseExtensions;
using Humanizer;
using TypeForge.Core.Configuration;

namespace TypeForge.Core.Extensions;

public static class StringExtensions
{
    public static string ToCamelCase(this string s)
    {
        return CaseExtensions.StringExtensions.ToCamelCase(s);
    }

    public static string ToKebabCase(this string str)
    {
        return CaseExtensions.StringExtensions.ToKebabCase(str);
    }

    public static string ToPascalCase(this string s)
    {
        return CaseExtensions.StringExtensions.ToPascalCase(s);
    }

    public static string ToPrettyJson<T>(this T obj)
    {
        return JsonSerializer.Serialize(
            obj,
            new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            }
        );
    }
}