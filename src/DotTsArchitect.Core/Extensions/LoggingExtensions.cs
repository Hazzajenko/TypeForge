using System.Text.Json;

namespace DotTsArchitect.Core.Extensions;

public static class LoggingExtensions
{
    public static void Log(this string message)
    {
        Console.WriteLine(message);
    }

    public static void Log(this string message, params object[] args)
    {
        Console.WriteLine(message, args);
    }

    public static void Log(this object @object)
    {
        var objectType = @object.GetType().Name;
        Console.WriteLine(objectType);
        var objectType2 = nameof(@object);
        Console.WriteLine(objectType2);
    }

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
