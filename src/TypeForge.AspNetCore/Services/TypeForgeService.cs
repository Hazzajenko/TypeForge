using TypeForge.Core.Configuration;

namespace TypeForge.AspNetCore.Services;

public class TypeForgeService
{
    public TypeForgeService(DotTsArchitectConfig config)
    {
        Path = config.Path;
        TypeNamePrefix = config.TypeNamePrefix;
        TypeNameSuffix = config.TypeNameSuffix;
        TypeModel = config.TypeModel;
        PropertyNameCase = config.PropertyNameCase;
        FileNameCase = config.FileNameCase;
    }

    public string Path { get; set; }
    public string? TypeNamePrefix { get; set; }
    public string? TypeNameSuffix { get; set; }
    public TypeModel TypeModel { get; set; }
    public PropertyNameCase PropertyNameCase { get; set; }
    public FileNameCase FileNameCase { get; set; }
}
