using System.Diagnostics;

namespace TypeForge.Core.Attributes;

[AttributeUsage(
    AttributeTargets.Interface | AttributeTargets.Class | AttributeTargets.Struct,
    AllowMultiple = false,
    Inherited = false
)]
public sealed class TypeForgeAttribute : Attribute
{
    public TypeForgeAttribute(string path)
    {
        Path = path;
    }

    public string Path { get; set; }

    public string GetCurrentLocation()
    {
        var stackTrace = new StackTrace(true);
        StackFrame? frame = stackTrace.GetFrame(0);
        if (frame is null)
        {
            return string.Empty;
        }
        var filePath = frame.GetFileName();
        var lineNumber = frame.GetFileLineNumber();
        var columnNumber = frame.GetFileColumnNumber();
        return $"{filePath}({lineNumber},{columnNumber})";
    }
}
