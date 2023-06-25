using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace DotTsArchitect.Core.StartUp;

public static class StartUp
{
    public static CSharpCompilation CreateCompilation(this SyntaxTree[] syntaxTrees)
    {
        var references = AppDomain.CurrentDomain
            .GetAssemblies()
            .Where(a => !a.IsDynamic)
            .Select(a => MetadataReference.CreateFromFile(a.Location));
        // .AddReferences(MetadataReference.CreateFromFile(typeof(object).Assembly.Location))
        var compilation = CSharpCompilation
            .Create("MyCompilation")
            .AddReferences(references)
            .AddSyntaxTrees(syntaxTrees);

        return compilation;
    }

    public static CSharpCompilation CreateCompilation()
    {
        var references = AppDomain.CurrentDomain
            .GetAssemblies()
            .Where(a => !a.IsDynamic)
            .Select(a => MetadataReference.CreateFromFile(a.Location));

        var compilation = CSharpCompilation.Create("MyCompilation").AddReferences(references);

        return compilation;
    }
}
