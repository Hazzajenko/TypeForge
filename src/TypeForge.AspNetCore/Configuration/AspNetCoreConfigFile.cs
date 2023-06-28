namespace TypeForge.AspNetCore.Configuration;

public class AspNetCoreConfigFile
{
    public string FolderNameCase { get; set; } = "KebabCase";
    public string? TypeNamePrefix { get; set; }
    public string? TypeNameSuffix { get; set; }
    public string? FileNamePrefix { get; set; }
    public string? FileNameSuffix { get; set; }
    public string FileNameCase { get; set; } = "KebabCase";
    public string TypeModel { get; set; } = "Type";
    public string TypeNameCase { get; set; } = "PascalCase";
    public string PropertyNameCase { get; set; } = "CamelCase";
    public string NullableType { get; set; } = "QuestionMark";
    public bool GenerateIndexFile { get; set; }
    public bool GroupByNameSpace { get; set; }
    public bool NameSpaceInOneFile { get; set; }
    public bool EndLinesWithSemicolon { get; set; }
    public AspNetCoreNameSpace[] NameSpaces { get; set; } = default!;
}

public class AspNetCoreNameSpace
{
    public string Name { get; set; } = default!;
    public bool IncludeChildren { get; set; } = true;
    public string Output { get; set; } = default!;
}

public class ConfigNameSpaceWithPath : AspNetCoreNameSpace
{
    public string Path { get; set; } = default!;
}
