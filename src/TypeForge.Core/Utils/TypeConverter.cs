using TypeForge.Core.Configuration;
using TypeForge.Core.Models;

namespace TypeForge.Core.Utils;

public static class TypeConverter
{
    private static readonly Dictionary<Type, string> TypeMap =
        new()
        {
            { typeof(string), "string" },
            { typeof(int), "number" },
            { typeof(DateTime), "string" },
            { typeof(DateOnly), "string" },
            { typeof(Boolean), "boolean" }
        };

    private static readonly Dictionary<string, string> StringTypeMap =
        new()
        {
            { "String", "string" },
            { "Guid", "string" },
            { "Int32", "number" },
            { "Int", "number" },
            { "Decimal", "number" },
            { "Double", "number" },
            { "Single", "number" },
            { "DateTime", "string" },
            { "DateOnly", "string" },
            { "Boolean", "boolean" }
        };

    public static string Convert(this Type type)
    {
        if (TypeMap.TryGetValue(type, out var tsType))
        {
            return tsType;
        }

        throw new ArgumentException($"Type {type} is not supported");
    }

    public static string Convert(this string stringType, NullableTypeOptions? nullOptions = null)
    {
        if (StringTypeMap.TryGetValue(stringType, out var tsType))
        {
            if (nullOptions is not null && nullOptions.IsNullable)
            {
                return tsType.AddNullableType(nullOptions.NullableType);
            }

            return tsType;
        }

        throw new ArgumentException($"Type {stringType} is not supported");
    }

    public static string ConvertArray(
        this string stringType,
        NullableTypeOptions? nullOptions = null
    )
    {
        if (StringTypeMap.TryGetValue(stringType, out var tsType))
        {
            if (nullOptions is not null && nullOptions.IsNullable)
            {
                return $"{tsType}[]".AddNullableType(nullOptions.NullableType);
            }

            return $"{tsType}[]";
        }

        throw new ArgumentException($"Type {stringType} is not supported");
    }

    private static string AddNullableType(this string type, NullableType nullableType)
    {
        return nullableType switch
        {
            NullableType.None => type,
            NullableType.QuestionMark => type,
            NullableType.Undefined => $"{type} | undefined",
            NullableType.Null => $"{type} | null",
            _ => throw new ArgumentOutOfRangeException(nameof(nullableType), nullableType, null)
        };
    }
}
