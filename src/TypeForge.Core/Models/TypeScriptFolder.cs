using Microsoft.CodeAnalysis.CSharp.Syntax;
using TypeForge.Core.Configuration;

namespace TypeForge.Core.Models;

public class TypeScriptFileConfig
{
    public string NameSpace { get; set; } = default!;
    public TypeScriptFile File { get; set; } = default!;
}

public class TypeScriptFolder
{
    public string FolderName { get; set; } = default!;
    public string OutputPath { get; set; } = default!;
    public IEnumerable<TypeScriptFile> Files { get; set; } = default!;

    public TypeScriptFolder(TypeScriptFolder typeScriptFolder)
    {
        FolderName = typeScriptFolder.FolderName;
        OutputPath = typeScriptFolder.OutputPath;
        Files = typeScriptFolder.Files;
    }

    public static TypeScriptFolder RemoveFolderName(TypeScriptFolder typeScriptFolder)
    {
        var childTypeScriptFolder = new TypeScriptFolder(typeScriptFolder);
        childTypeScriptFolder.FolderName = "";
        return childTypeScriptFolder;
    }

    public TypeScriptFolder() { }
}

public class TypeScriptFile
{
    public string FileName { get; set; } = default!;
    public string PathRelativeToRootOutput { get; set; } = default!;
    public IEnumerable<TypeScriptType> Types { get; set; } = default!;
}

public class TypeScriptType
{
    public string Name { get; set; } = default!;
    public IEnumerable<TypeProperty> Properties { get; set; } = default!;
}

public class TypeProperty
{
    public string Name { get; set; } = default!;
    public string Type { get; set; } = default!;
}

public class ChildTypeScriptFolder : TypeScriptFolder
{
    public IEnumerable<ChildTypeScriptFolder> ChildFolders { get; set; } = default!;

    public ChildTypeScriptFolder(ChildTypeScriptFolder childTypeScriptFolder)
        : base(childTypeScriptFolder)
    {
        ChildFolders = childTypeScriptFolder.ChildFolders;
    }

    public ChildTypeScriptFolder() { }
}
