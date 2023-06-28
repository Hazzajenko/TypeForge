namespace TypeForge.Core.TypeConverting.Dictionaries;

public static class TypeMapDictionary
{
    private const string TypeScriptString = "string";
    private const string TypeScriptNumber = "number";
    private const string TypeScriptBoolean = "boolean";

    private static readonly Dictionary<string, string> StringTypeMap = GetMapping();

    private static string[] TypeScriptStringTypes() =>
        new[] { nameof(String), nameof(Guid), nameof(DateTime), nameof(DateOnly) };

    private static string[] TypeScriptNumberTypes() =>
        new[]
        {
            nameof(Int16),
            nameof(Int32),
            nameof(Int64),
            nameof(Decimal),
            nameof(Double),
            nameof(Single)
        };

    private static Dictionary<string, string> GetMapping()
    {
        var result = new Dictionary<string, string>();

        foreach (var item in TypeScriptStringTypes())
        {
            result[item] = TypeScriptString;
        }

        foreach (var item in TypeScriptNumberTypes())
        {
            result[item] = TypeScriptNumber;
        }

        result[nameof(Boolean)] = TypeScriptBoolean;

        return result;
    }

    public static string GetMappedType(string type)
    {
        return StringTypeMap.TryGetValue(type, out var result)
            ? result
            : throw new ArgumentException($"Type {type} is not supported");
    }
}
