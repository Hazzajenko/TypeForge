using System.Collections.ObjectModel;
using Microsoft.CodeAnalysis;

namespace TypeForge.Core.TypeConverting.Dictionaries;

public static class ArrayMapDictionary
{
    private static string[] TypeScriptArrayTypes() =>
        new[]
        {
            nameof(List<object>),
            nameof(IList<object>),
            nameof(ICollection<object>),
            nameof(Collection<object>),
            nameof(IEnumerable<object>),
            nameof(ReadOnlyCollection<object>),
            nameof(IReadOnlyCollection<object>),
            nameof(IReadOnlyList<object>)
        };

    public static bool IsArrayType(string type) => TypeScriptArrayTypes().Contains(type);

    public static bool IsArrayType(this ITypeSymbol typeSymbol) =>
        TypeScriptArrayTypes().Contains(typeSymbol.Name);
}
