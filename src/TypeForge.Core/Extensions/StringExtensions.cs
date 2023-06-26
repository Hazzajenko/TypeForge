using System.Text.Json;
using System.Text.RegularExpressions;
using TypeForge.Core.Configuration;

namespace TypeForge.Core.Extensions;

public static class StringExtensions
{
    public static string ToCamelCase(this string s)
    {
        var x = s.Replace("_", "");
        if (x.Length == 0)
            return "null";
        x = Regex.Replace(
            x,
            "([A-Z])([A-Z]+)($|[A-Z])",
            m => m.Groups[1].Value + m.Groups[2].Value.ToLower() + m.Groups[3].Value
        );
        return char.ToLower(x[0]) + x.Substring(1);
    }

    public static string ToKebabCase(this string str)
    {
        if (string.IsNullOrWhiteSpace(str))
            return str;

        return Regex.Replace(str, @"([a-z0-9])([A-Z])", "$1-$2", RegexOptions.Compiled).ToLower();
    }

    public static string ToPascalCase(this string s)
    {
        var x = s.Replace("_", "");
        if (x.Length == 0)
            return "null";
        x = Regex.Replace(
            x,
            "([A-Z])([A-Z]+)($|[A-Z])",
            m => m.Groups[1].Value + m.Groups[2].Value.ToLower() + m.Groups[3].Value
        );
        return char.ToUpper(x[0]) + x.Substring(1);
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
