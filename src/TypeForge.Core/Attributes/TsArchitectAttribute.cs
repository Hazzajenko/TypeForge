using System.Diagnostics;

namespace TypeForge.Core.Attributes;

[AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public sealed class TsArchitectAttribute: Attribute
{
    public TsArchitectAttribute(
        string path 
        )
    {
        Path = path;
    }
    
    public string Path { get; set;  }
    
    public string GetCurrentLocation()
    {
        /*var currentLocation = Path;
        if (string.IsNullOrWhiteSpace(currentLocation))
        {
            currentLocation = Environment.CurrentDirectory;
        }
        return currentLocation;*/
        var stackTrace = new StackTrace(true);
        var frame = stackTrace.GetFrame(0);
        var filePath = frame.GetFileName();
        var lineNumber = frame.GetFileLineNumber();
        var columnNumber = frame.GetFileColumnNumber();
        return $"{filePath}({lineNumber},{columnNumber})";
    }
}