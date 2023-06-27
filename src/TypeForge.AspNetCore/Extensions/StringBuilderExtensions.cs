using System.Text;
using TypeForge.Core.Utils;

namespace TypeForge.AspNetCore.Extensions;

public static class StringBuilderExtensions
{
    public static StringBuilder AppendLine(
        this StringBuilder stringBuilder,
        string content,
        bool endWithSemicolon
    )
    {
        return stringBuilder.AppendLine(content.EndWithSemicolon(endWithSemicolon));
    }

    public static StringBuilder AppendLineWithTab(
        this StringBuilder stringBuilder,
        string content,
        bool endWithSemicolon
    )
    {
        return stringBuilder.AppendLine("\t" + content.EndWithSemicolon(endWithSemicolon));
    }
}
