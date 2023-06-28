using Serilog;
using TypeForge.Core.Configuration;

namespace TypeForge.Core.Extensions;

public static class FileExtensions
{
    public static FileStream CreateFileSafe(this FileInfo file)
    {
        if (file.Directory!.Exists)
            return file.Create();
        file.Directory.Create();

        return file.Create();
    }

    public static string TakeOutTsExtension(this string fileName)
    {
        return fileName.Replace(".ts", "");
    }
}
