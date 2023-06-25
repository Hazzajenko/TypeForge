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
}
