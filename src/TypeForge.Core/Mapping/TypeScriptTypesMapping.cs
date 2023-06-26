using System.Net.NetworkInformation;
using TypeForge.Core.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Serilog;
using TypeForge.Core.Configuration;
using TypeForge.Core.Models;

namespace TypeForge.Core.Mapping;

public static class TypeScriptTypesMapping
{
    public static TypeScriptType MapToTypeScriptType(
        this ClassDeclarationSyntax request,
        GlobalConfig config
    )
    {
        string typeName = request.Identifier.Text.GetTypeName(config);

        var properties = request
            .DescendantNodes()
            .OfType<PropertyDeclarationSyntax>()
            .Select(property =>
            {
                string propertyName = property.Identifier.ValueText.ToCaseOfOption(
                    config.PropertyNameCase
                );
                (string Value, bool Nullable) propertyResult = request.GetTypeFromCompilation(
                    property,
                    config.Compilation,
                    config.NullableType
                );

                if (propertyResult.Nullable)
                {
                    propertyName = $"{propertyName}?";
                }
                return new TypeProperty { Name = propertyName, Type = propertyResult.Value };
            });

        return new TypeScriptType { Name = typeName, Properties = properties };
    }

    public static TypeScriptType MapToTypeScriptType(
        this ClassDeclarationSyntax request,
        InputGlobalConfig config,
        CSharpCompilation compilation
    )
    {
        string typeName = request.Identifier.Text.GetTypeName(config);

        var properties = request
            .DescendantNodes()
            .OfType<PropertyDeclarationSyntax>()
            .Select(property =>
            {
                string propertyName = property.Identifier.ValueText.ToCaseOfOption(
                    config.PropertyNameCase
                );
                (string Value, bool Nullable) propertyResult = request.GetTypeFromCompilation(
                    property,
                    compilation,
                    config.NullableType
                );
                if (propertyResult.Nullable)
                {
                    Log.Information("Nullable name propertyResult");
                    propertyName = $"{propertyName}?";
                }
                return new TypeProperty { Name = propertyName, Type = propertyResult.Value };
            });

        return new TypeScriptType { Name = typeName, Properties = properties };
    }

    private static (string Value, bool Nullable) GetTypeFromCompilation(
        this ClassDeclarationSyntax typeSyntax,
        PropertyDeclarationSyntax property,
        Compilation compilation,
        NullableType nullableType
    )
    {
        Compilation comp = compilation.AddSyntaxTrees(typeSyntax.SyntaxTree);
        SemanticModel semanticModel = comp.GetSemanticModel(typeSyntax.SyntaxTree);
        var kindText = property.Type.Kind().ToString();
        ITypeSymbol? typeSymbol = ModelExtensions.GetTypeInfo(semanticModel, property.Type).Type;
        var typeSymbolName = typeSymbol!.Name;

        if (kindText == "NullableType")
        {
            return new() { Value = typeSymbol.HandleNullable(nullableType), Nullable = true, };
            // return typeSymbol.HandleNullable(nullableType);
        }

        if (typeSymbolName == "String")
        {
            if (kindText == "NullableType")
            {
                return new()
                {
                    Value = typeSymbolName.Convert(new NullableTypeOptions(nullableType)),
                    Nullable = true,
                };
                // return typeSymbolName.Convert(new NullableTypeOptions(nullableType));
            }
        }

        if (typeSymbolName == "IEnumerable")
        {
            return new()
            {
                Value = typeSymbol.HandleList(new NullableTypeOptions(nullableType)),
                Nullable = false,
            };
            // return typeSymbol.HandleList();
        }

        return new() { Value = typeSymbolName.Convert(), Nullable = false, };
        // return typeSymbolName.Convert();
    }

    private static string HandleNullable(this ITypeSymbol typeSymbol, NullableType nullableType)
    {
        if (typeSymbol.Name == "String")
        {
            return typeSymbol.Name.Convert(new NullableTypeOptions(nullableType));
        }
        if (typeSymbol is INamedTypeSymbol namedTypeSymbol)
        {
            if (namedTypeSymbol.IsGenericType && namedTypeSymbol.Name == "Nullable")
            {
                ITypeSymbol nullableUnderlyingType = namedTypeSymbol.TypeArguments[0];
                if (nullableUnderlyingType.Name == "IEnumerable")
                {
                    return nullableUnderlyingType.HandleList(new NullableTypeOptions(nullableType));
                }
                return nullableUnderlyingType.Name.Convert(new NullableTypeOptions(nullableType));
            }
        }
        throw new Exception("Type is not nullable");
    }

    private static string HandleList(
        this ITypeSymbol typeSymbol,
        NullableTypeOptions? nullOptions = null
    )
    {
        if (typeSymbol is INamedTypeSymbol namedTypeSymbol)
        {
            if (namedTypeSymbol.IsGenericType && namedTypeSymbol.Name == "IEnumerable")
            {
                ITypeSymbol nullableUnderlyingType = namedTypeSymbol.TypeArguments[0];
                return nullableUnderlyingType.Name.ConvertArray(nullOptions);
            }
        }
        throw new Exception("Type is not a list");
    }

    private static bool IsNullableType(
        this ITypeSymbol typeSymbol,
        out INamedTypeSymbol? namedTypeSymbol
    )
    {
        namedTypeSymbol = typeSymbol as INamedTypeSymbol;
        return namedTypeSymbol != null
            && namedTypeSymbol.IsGenericType
            && namedTypeSymbol.ConstructedFrom.Name == "Nullable";
    }

    private static bool IsListType(
        this string typeSymbolName,
        ITypeSymbol typeSymbol,
        out INamedTypeSymbol? namedTypeSymbol
    )
    {
        namedTypeSymbol = typeSymbol as INamedTypeSymbol;
        var isGenericType = namedTypeSymbol != null && namedTypeSymbol.IsGenericType;
        var ctorName = namedTypeSymbol?.ConstructedFrom.Name;
        if (typeSymbolName == "IEnumerable")
        {
            var isIEnumerable = ctorName is "IEnumerable";
        }
        return namedTypeSymbol is not null && typeSymbolName == "IEnumerable";
    }

    private static bool IsListType(
        this ITypeSymbol typeSymbol,
        out INamedTypeSymbol? namedTypeSymbol
    )
    {
        namedTypeSymbol = typeSymbol as INamedTypeSymbol;
        return namedTypeSymbol != null
            && namedTypeSymbol.IsGenericType
            && namedTypeSymbol.ConstructedFrom.Name
                is "List"
                    or "IList"
                    or "IEnumerable"
                    or "ICollection"
                    or "IReadOnlyList"
                    or "IReadOnlyCollection";
    }

    /*private static string ConvertArray(this string typeSymbolName)
    {
        return typeSymbolName switch
        {
            "Int32" => "number[]",
            "String" => "string[]",
            "Boolean" => "boolean[]",
            "DateTime" => "Date[]",
            _ => $"{typeSymbolName}[]"
        };
    }*/
}
