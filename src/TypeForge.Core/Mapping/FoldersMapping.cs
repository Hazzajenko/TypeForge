using TypeForge.Core.Models;

namespace TypeForge.Core.Mapping;

public static class FoldersMapping
{
    public static IEnumerable<TypeScriptType> GetTypesFromFolders(
        this IEnumerable<TypeScriptFolder> typeScriptFolders
    ) => typeScriptFolders.SelectMany(x => x.Files).SelectMany(x => x.Types);
}
