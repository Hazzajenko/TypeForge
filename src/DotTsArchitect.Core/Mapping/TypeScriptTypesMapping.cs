using DotTsArchitect.Core.Configuration;
using DotTsArchitect.Core.Extensions;
using DotTsArchitect.Core.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DotTsArchitect.Core.Mapping;

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
                string propertyType = request.GetTypeFromCompilation(property, config.Compilation);
                return new TypeProperty { Name = propertyName, Type = propertyType };
            });

        return new TypeScriptType { Name = typeName, Properties = properties };
    }

    private static string GetTypeFromCompilation(
        this ClassDeclarationSyntax typeSyntax,
        PropertyDeclarationSyntax property,
        Compilation compilation
    )
    {
        var comp = compilation.AddSyntaxTrees(typeSyntax.SyntaxTree);
        var semanticModel = comp.GetSemanticModel(typeSyntax.SyntaxTree);
        var typeSymbol = semanticModel.GetTypeInfo(property.Type).Type;
        var typeSymbolName = typeSymbol!.Name;
        return typeSymbolName.Convert();
    }
}
