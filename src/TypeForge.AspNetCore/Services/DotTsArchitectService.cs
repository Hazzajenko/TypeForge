using TypeForge.Core.Configuration;

namespace DotTsArchitect.AspNetCore.Services;

public class DotTsArchitectService
{
    public DotTsArchitectService(DotTsArchitectConfig config)
    {
        Path = config.Path;
        TypeNamePrefix = config.TypeNamePrefix;
        TypeNameSuffix = config.TypeNameSuffix;
        ExportModelType = config.ExportModelType;
        PropertyNameCase = config.PropertyNameCase;
        FileNameCase = config.FileNameCase;
    }

    public string Path { get; set; }
    public string? TypeNamePrefix { get; set; }
    public string? TypeNameSuffix { get; set; }
    public ExportModelType ExportModelType { get; set; }
    public PropertyNameCase PropertyNameCase { get; set; }
    public FileNameCase FileNameCase { get; set; }
}
