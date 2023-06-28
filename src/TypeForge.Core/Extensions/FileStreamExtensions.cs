using System.Text;

namespace TypeForge.Core.Extensions;

public static class FileStreamExtensions
{
    public static void WriteLine(this FileStream fileStream, string content, bool endWithSemicolon)
    {
        Byte[] txt = new UTF8Encoding(true).GetBytes(
            content.EndWithSemicolon(endWithSemicolon) + Environment.NewLine
        );
        fileStream.Write(txt, 0, txt.Length);
    }

    public static void WriteEmptyLine(this FileStream fileStream)
    {
        Byte[] txt = new UTF8Encoding(true).GetBytes(Environment.NewLine);
        fileStream.Write(txt, 0, txt.Length);
    }

    public static string EndWithSemicolon(this string content, bool endWithSemicolon)
    {
        return content + (endWithSemicolon ? ";" : "");
    }

    public static void WriteLineWithTab(
        this FileStream fileStream,
        string content,
        bool endWithSemicolon
    )
    {
        Byte[] txt = new UTF8Encoding(true).GetBytes(
            "\t" + content.EndWithSemicolon(endWithSemicolon) + Environment.NewLine
        );
        fileStream.Write(txt, 0, txt.Length);
    }
}
