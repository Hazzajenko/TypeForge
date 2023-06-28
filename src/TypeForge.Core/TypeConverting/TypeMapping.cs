using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using TypeForge.Core.Configuration;
using TypeForge.Core.Configuration.TypeForgeConfig;
using TypeForge.Core.Extensions;
using TypeForge.Core.Models;
using TypeForge.Core.TypeConverting.Dictionaries;

namespace TypeForge.Core.TypeConverting;

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
        ITypeSymbol typeSymbol = semanticModel.GetTypeInfo(property.Type).Type!;

        if (property.IsNullableType())
        {
            return new TypeFromCompilationResult(typeSymbol.HandleNullable(nullableType), true);
        }

        if (typeSymbol.IsArrayType())
        {
            return new TypeFromCompilationResult(
                typeSymbol.HandleArrayType(new NullableTypeOptions(nullableType)),
                false
            );
        }

        return new TypeFromCompilationResult(typeSymbol.Convert(), false);
    }

    private static bool IsNullableType(this PropertyDeclarationSyntax property) =>
        property.Type.Kind() == SyntaxKind.NullableType;

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
            ? HandleArrayType(nullableUnderlyingType, new NullableTypeOptions(nullableType))
            : nullableUnderlyingType.Name.Convert(new NullableTypeOptions(nullableType));
    }

    private static string HandleArrayType(
        this ITypeSymbol typeSymbol,
        NullableTypeOptions? nullOptions = null
    )
    {
        if (typeSymbol is not INamedTypeSymbol namedTypeSymbol)
            throw new Exception("Type is not a list");
        if (!namedTypeSymbol.IsGenericType || namedTypeSymbol.Name != "IEnumerable")
            throw new Exception("Type is not a list");
        ITypeSymbol nullableUnderlyingType = namedTypeSymbol.TypeArguments[0];
        return nullableUnderlyingType.ConvertArray(nullOptions);
    }
}
