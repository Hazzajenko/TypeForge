using TypeForge.Core.Configuration;
using TypeForge.Core.Configuration.TypeForgeConfig;
using TypeForge.Core.Services;

namespace TypeForge.AspNetCore.Services;

public class TypeForgeService : WriterService
{
    private readonly TypeForgeConfig _config;

    public TypeForgeService(TypeForgeConfig config)
        : base(config)
    {
        _config = config;
    }
}
