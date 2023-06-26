using Serilog;
using TypeForge.Core.Configuration;

namespace TypeForge.Core.Extensions;

public static class FileExtensions
{
    public static FileStream CreateFileSafe(
        this FileInfo file,
        FolderNameCase folderNameCase = FolderNameCase.KebabCase
    )
    {
        /*string newDirectoryName = file.Directory!.Name.ToCaseOfOption(folderNameCase);
        DirectoryInfo parentDirectory = file.Directory.Parent!;

        string newDirectoryFullPath = Path.Combine(parentDirectory.FullName, newDirectoryName);

        if (!Directory.Exists(newDirectoryFullPath))
        {
            Log.Logger.Information("Creating directory {Directory}", newDirectoryFullPath);
            Directory.CreateDirectory(newDirectoryFullPath);
        }

        string correctedPath = Path.Combine(newDirectoryFullPath, file.Name);
        FileInfo newFile = new FileInfo(correctedPath);*/

        if (!file.Directory!.Exists)
        {
            Log.Logger.Information("Creating directory {Directory}", file.Directory.FullName);
            file.Directory.Create();
        }

        return file.Create();
        // return newFile.Create();
    }

    public static string TakeOutTsExtension(this string fileName)
    {
        return fileName.Replace(".ts", "");
    }
}
