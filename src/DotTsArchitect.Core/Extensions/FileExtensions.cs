using Serilog;

namespace DotTsArchitect.Core.Extensions;

public static class FileExtensions
{
    public static FileStream CreateFileSafe(this FileInfo file)
    {
        if (!file.Directory!.Exists)
        {
            Log.Logger.Information("Creating directory {Directory}", file.Directory.FullName);
            file.Directory.Create();
        }

        return file.Create();
    }

    public static string TakeOutTsExtension(this string fileName)
    {
        return fileName.Replace(".ts", "");
    }
}
