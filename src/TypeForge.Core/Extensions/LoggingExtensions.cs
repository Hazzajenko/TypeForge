using System.Text.Json;

namespace TypeForge.Core.Extensions;

public static class LoggingExtensions
{
    public static void DumpObjectJson(this object obj)
    {
        var type = obj.GetType();
        var typeName = type.Name;
        var json = JsonSerializer.Serialize(
            obj,
            new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            }
        );
        Serilog.Log.Logger.Information("{Type} json: {Json}", typeName, json);
    }
}
