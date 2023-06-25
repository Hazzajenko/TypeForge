using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace DotTsArchitect.Core.Services;

public class CompilationService
{
    public CompilationService() { }

    public CSharpCompilation CreateCompilation()
    {
        var references = AppDomain.CurrentDomain
            .GetAssemblies()
            .Where(a => !a.IsDynamic)
            .Select(a => MetadataReference.CreateFromFile(a.Location));

        var compilation = CSharpCompilation.Create("MyCompilation").AddReferences(references);

        return compilation;
        // .AddSyntaxTrees(tree);
    }
}
