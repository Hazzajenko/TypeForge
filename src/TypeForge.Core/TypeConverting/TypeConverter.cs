using Microsoft.CodeAnalysis;
using TypeForge.Core.Models;
using TypeForge.Core.TypeConverting.Dictionaries;

namespace TypeForge.Core.TypeConverting;

public static class TypeConverter
{
    public static string Convert(this string stringType, NullableTypeOptions? nullOptions = null)
    {
        var typeScriptType = TypeMapDictionary.GetMappedType(stringType);
        if (nullOptions is not null && nullOptions.IsNullable)
        {
            return typeScriptType.AddNullableType(nullOptions.NullableType);
        }

        return typeScriptType;
    }

    public static string Convert(
        this ITypeSymbol typeSymbol,
        NullableTypeOptions? nullOptions = null
    ) => typeSymbol.Name.Convert(nullOptions);

    public static string ConvertArray(
        this string stringType,
        NullableTypeOptions? nullOptions = null
    )
    {
        var typeScriptType = TypeMapDictionary.GetMappedType(stringType);
        if (nullOptions is not null && nullOptions.IsNullable)
        {
            return $"{typeScriptType}[]".AddNullableType(nullOptions.NullableType);
        }

        return $"{typeScriptType}[]";
    }

    public static string ConvertArray(
        this ITypeSymbol typeSymbol,
        NullableTypeOptions? nullOptions = null
    ) => typeSymbol.Name.ConvertArray(nullOptions);

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
