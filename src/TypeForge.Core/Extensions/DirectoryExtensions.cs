﻿namespace TypeForge.Core.Extensions;

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
        string fileName
    )
    {
        var splitFromNamespace = fileLocation.Split(parentNamespace)[1];
        var path = fileLocation.Replace(splitFromNamespace, "");
        var takeOutFileName = path.Replace(fileName, "");
        var split = takeOutFileName.Split("\\")[takeOutFileName.Split("\\").Length - 1];
        return split;
    }

    public static string[] GetFilesInNamespace(this string path) =>
        Directory.GetFiles(path, "*.cs", SearchOption.AllDirectories);
}
