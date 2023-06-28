using TypeForge.Core.Models;

namespace TypeForge.Core.Mapping;

public static class TypeScriptFolderMapping
{
    public static TypeScriptFolder ToRemoveFolderName(this TypeScriptFolder typeScriptFolder) =>
        new(typeScriptFolder)
        {
            FolderName = typeScriptFolder.FolderName.Replace(
                $"{typeScriptFolder.FolderName}{Path.DirectorySeparatorChar}",
                ""
            )
        };

    public static ChildTypeScriptFolder ToRemoveFolderName(
        this ChildTypeScriptFolder typeScriptFolder
    ) =>
        new(typeScriptFolder)
        {
            FolderName = typeScriptFolder.FolderName.Replace(
                $"{typeScriptFolder.FolderName}{Path.DirectorySeparatorChar}",
                ""
            )
        };

    public static TypeScriptFolder ToIndividualFolderName(this TypeScriptFolder typeScriptFolder)
    {
        if (typeScriptFolder.FolderName.Contains(Path.DirectorySeparatorChar))
        {
            return new TypeScriptFolder
            {
                FolderName = typeScriptFolder.FolderName.Split(Path.DirectorySeparatorChar).Last(),
                Files = typeScriptFolder.Files
            };
        }

        return typeScriptFolder;
    }
}
