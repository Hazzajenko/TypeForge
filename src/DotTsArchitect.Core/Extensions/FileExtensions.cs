namespace DotTsArchitect.Core.Extensions;

public static class FileExtensions
{
    public static FileStream CreateFileSafe(this FileInfo file)
    {
        // check if folder exists
        if (!file.Directory!.Exists)
        {
            file.Directory.Create();
        }

        return file.Create();

        /*if (!file.Exists)
        {
            file.Create().Close();
        }

        return file.OpenWrite();*/
    }
}
