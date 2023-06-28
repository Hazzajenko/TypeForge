using Serilog;
using TypeForge.Core.Configuration;
using TypeForge.Core.Models;

namespace TypeForge.Core.Extensions;

public static class DirectoryExtensions
{
    public static string GetProjectDirectory()
    {
        string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        return baseDirectory.Substring(0, baseDirectory.IndexOf("bin", StringComparison.Ordinal));
    }

    public static string GetRelativePath(string fromPath, string toPath)
    {
        if (string.IsNullOrEmpty(fromPath))
            throw new ArgumentNullException(nameof(fromPath));
        if (string.IsNullOrEmpty(toPath))
            throw new ArgumentNullException(nameof(toPath));

        var fromUri = new Uri(fromPath);
        var toUri = new Uri(toPath);

        if (fromUri.Scheme != toUri.Scheme)
            return toPath;

        var relativeUri = fromUri.MakeRelativeUri(toUri);
        var relativePath = Uri.UnescapeDataString(relativeUri.ToString());

        return relativePath.Replace('/', Path.DirectorySeparatorChar);
    }

    public static string GetPathFromParentNamespace(
        this string fileLocation,
        string parentNamespace,
        string fileName,
        FolderNameCase folderNameCase
    )
    {
        var splitFromNamespace = fileLocation.Split(parentNamespace)[1];
        var path = splitFromNamespace.Split(fileName)[0];
        path = path.RemoveStartAndEndSlash();
        return path.ToCaseOfOption(folderNameCase);
    }

    public static string GetPathFromParentNamespaceKeepRootFolder(
        this string fileLocation,
        string parentNamespace,
        string fileName,
        FolderNameCase folderNameCase
    )
    {
        var adjustedFileLocation = fileLocation.Replace("/", "\\");
        var parentKeepRootFolder = GetParentDirectorySafe(parentNamespace);
        var splitFromNamespace = adjustedFileLocation.Split(parentKeepRootFolder)[1];
        var path = splitFromNamespace.Split(fileName)[0];
        path = path.RemoveStartAndEndSlash();
        return path.ToCaseOfOption(folderNameCase);
    }

    public static string GetLastDirectoryInPath(this string path) =>
        path.Split(Path.DirectorySeparatorChar)[path.Split(Path.DirectorySeparatorChar).Length - 1];

    public static string GetParentDirectoryIfContains(this string path, string contains)
    {
        if (path.Contains(contains))
            return path.GetParentDirectorySafe();
        return path;
    }

    // private static string GetParentDirectory(this string path) =>
    //     Directory.GetParent(path).FullName;

    public static string GetParentDirectorySafe(this string path)
    {
        var directoryInfo = new DirectoryInfo(path);
        return directoryInfo.Parent is not null ? directoryInfo.Parent.FullName : path;
    }

    public static string GetStartOfPath(this string path) =>
        path.Split(Path.DirectorySeparatorChar)[0];

    public static string ConvertSlashesToBackSlashes(this string path) =>
        path.Replace("/", Path.DirectorySeparatorChar.ToString());

    private static string RemoveStartAndEndSlash(this string path) =>
        path.RemoveStartSlash().RemoveEndSlash();

    private static string RemoveStartSlash(this string path) =>
        path.StartsWith("\\") ? path[1..] : path;

    private static string RemoveEndSlash(this string path) =>
        path.EndsWith("\\") ? path[..^1] : path;

    public static IEnumerable<string> GetFilesInAllDirectories(
        this ConfigDirectoryWithPath configDirectory
    ) => Directory.GetFiles(configDirectory.Path, "*.cs", SearchOption.AllDirectories);

    public static IEnumerable<string> GetFilesInOnlyTopDirectory(
        this ConfigDirectoryWithPath configDirectory
    ) => Directory.GetFiles(configDirectory.Path, "*.cs", SearchOption.TopDirectoryOnly);

    public static IEnumerable<string> GetFilesInDirectoryAndSubDirectoriesByDepth(
        this ConfigDirectoryWithPath configDirectory
    )
    {
        var path = configDirectory.Path;
        var depth = configDirectory.Depth;
        if (depth == -1)
        {
            return configDirectory.GetFilesInAllDirectories().ToList();
        }
        var files = new List<string>();
        files.AddRange(Directory.GetFiles(path, "*.cs"));
        if (depth <= 1)
            return files;
        foreach (var directory in Directory.GetDirectories(path))
        {
            files.AddRange(GetFilesInDirectory(directory, depth - 1));
        }

        return files;
    }

    public static IEnumerable<string> GetFilesInDirectory(this string path, int depth = -1)
    {
        var files = new List<string>();
        files.AddRange(Directory.GetFiles(path, "*.cs"));
        if (depth <= 1)
            return files;
        foreach (var directory in Directory.GetDirectories(path))
        {
            files.AddRange(GetFilesInDirectory(directory, depth - 1));
        }

        return files;
    }

    public static string GetFolderName(this string path, FolderNameCase folderNameCase) =>
        path.Split("\\")[path.Split("\\").Length - 1].ToCaseOfOption(folderNameCase);
}
