using System.Text;

namespace DotTsArchitect.Core.Utils;

public static class WriterUtils
{
    public static void WriteLine(this FileStream fileStream, string content)
    {
        Byte[] txt = new UTF8Encoding(true).GetBytes(content + Environment.NewLine);
        fileStream.Write(txt, 0, txt.Length);
    }
}
