using System.Data.Common;
using DotTsArchitect.Core.TypeConverters;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DotTsArchitect.Core;

public abstract class TypeConverter
{
    public abstract string Convert(Type type);
}

public class CSharpTypeSwitch<T>
{
    private readonly Dictionary<Type, Func<T>> _matches = new();

    public CSharpTypeSwitch<T> Case<TCase>(Func<T> action)
        where TCase : T
    {
        _matches.Add(typeof(TCase), action);
        return this;
    }

    public T Switch(Type type)
    {
        if (_matches.TryGetValue(type, out var action))
        {
            return action();
        }

        throw new ArgumentException($"Type {type} is not supported");
    }
}

public class TypeConverterService { }

public static class Helpers
{
    /*public static string Convert(this Type type)
    {
        if (type == typeof(string))
        {
            return "string";
        }
        else if (type == typeof(int))
        {
            return "number";
        }
        else if (type == typeof(DateTime))
        {
            return "string";
        }
        else if (type == typeof(DateOnly))
        {
            return "string";
        }
        
        throw new ArgumentException($"Type {type} is not supported");
    }*/
}

public static class TypeConverter3
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
            { "Int32", "number" },
            { "DateTime", "string" },
            { "DateOnly", "string" },
            { "Boolean", "boolean" }
        };

    /*
    private static readonly Dictionary<SyntaxToken, string> TypeSyntaxMap = new ()
    {
        { , "string" },
        { typeof(int), "number" },
        { typeof(DateTime), "string" },
        { typeof(DateOnly), "string" }
    };
    */

    public static string Convert(this Type type)
    {
        if (TypeMap.TryGetValue(type, out var tsType))
        {
            return tsType;
        }

        throw new ArgumentException($"Type {type} is not supported");
    }

    public static string Convert(this string stringType)
    {
        if (StringTypeMap.TryGetValue(stringType, out var tsType))
        {
            return tsType;
        }

        throw new ArgumentException($"Type {stringType} is not supported");
    }
}




/*switch (type)
{
    case string:
        return "string";


}*/
/*return new CSharpTypeSwitch<Type>()
    .Case<NumberConverter>(() => new NumberConverter().Convert(type))
    .Case<DateTimeConverter>(() => new DateTimeConverter().Convert(type))
    .Case<StringConverter>(() => new StringConverter().Convert(type))
    .Switch(type);*/
