using System.Net.NetworkInformation;
using TypeForge.Core.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Serilog;
using TypeForge.Core.Configuration;
using TypeForge.Core.Models;
using TypeForge.Core.Utils;

namespace TypeForge.Core.Mapping;

public static class TypeMapping
{
    public static TypeScriptType MapToTypeScriptType(
        this ClassDeclarationSyntax request,
        TypeForgeConfig config,
        CSharpCompilation compilation
    )
    {
        string typeName = request.Identifier.Text.GetTypeName(config);

        var properties = request
            .DescendantNodes()
            .OfType<PropertyDeclarationSyntax>()
            .Select(property => property.ToTypeProperty(request, config, compilation));

        return new TypeScriptType { Name = typeName, Properties = properties };
    }

    private static TypeProperty ToTypeProperty(
        this PropertyDeclarationSyntax property,
        ClassDeclarationSyntax request,
        TypeForgeConfig config,
        Compilation compilation
    )
    {
        string propertyName = property.Identifier.ValueText.ToCaseOfOption(config.PropertyNameCase);
        TypeFromCompilationResult propertyResult = request.GetTypeFromCompilation(
            property,
            compilation,
            config.NullableType
        );
        if (propertyResult.Nullable && config.NullableType == NullableType.QuestionMark)
        {
            propertyName = $"{propertyName}?";
        }
        return new TypeProperty { Name = propertyName, Type = propertyResult.Value };
    }

    private record TypeFromCompilationResult(string Value, bool Nullable);

    private static TypeFromCompilationResult GetTypeFromCompilation(
        this ClassDeclarationSyntax typeSyntax,
        PropertyDeclarationSyntax property,
        Compilation compilation,
        NullableType nullableType
    )
    {
        SemanticModel semanticModel = compilation
            .AddSyntaxTrees(typeSyntax.SyntaxTree)
            .GetSemanticModel(typeSyntax.SyntaxTree);
        var kindText = property.Type.Kind().ToString();
        ITypeSymbol? typeSymbol = ModelExtensions.GetTypeInfo(semanticModel, property.Type).Type;
        var typeSymbolName = typeSymbol!.Name;

        if (kindText == "NullableType")
        {
            return new TypeFromCompilationResult(typeSymbol.HandleNullable(nullableType), true);
        }

        if (typeSymbolName == "IEnumerable")
        {
            return new TypeFromCompilationResult(
                typeSymbol.HandleList(new NullableTypeOptions(nullableType)),
                false
            );
        }

        return new TypeFromCompilationResult(typeSymbolName.Convert(), false);
    }

    private static string HandleNullable(this ITypeSymbol typeSymbol, NullableType nullableType)
    {
        if (typeSymbol.Name == "String")
        {
            return typeSymbol.Name.Convert(new NullableTypeOptions(nullableType));
        }

        if (typeSymbol is not INamedTypeSymbol namedTypeSymbol)
            throw new Exception("Type is not nullable");
        if (!namedTypeSymbol.IsGenericType || namedTypeSymbol.Name != "Nullable")
            throw new Exception("Type is not nullable");
        ITypeSymbol nullableUnderlyingType = namedTypeSymbol.TypeArguments[0];
        return nullableUnderlyingType.Name == "IEnumerable"
            ? HandleList(nullableUnderlyingType, new NullableTypeOptions(nullableType))
            : nullableUnderlyingType.Name.Convert(new NullableTypeOptions(nullableType));
    }

    private static string HandleList(
        this ITypeSymbol typeSymbol,
        NullableTypeOptions? nullOptions = null
    )
    {
        if (typeSymbol is not INamedTypeSymbol namedTypeSymbol)
            throw new Exception("Type is not a list");
        if (!namedTypeSymbol.IsGenericType || namedTypeSymbol.Name != "IEnumerable")
            throw new Exception("Type is not a list");
        ITypeSymbol nullableUnderlyingType = namedTypeSymbol.TypeArguments[0];
        return nullableUnderlyingType.Name.ConvertArray(nullOptions);
    }
}
