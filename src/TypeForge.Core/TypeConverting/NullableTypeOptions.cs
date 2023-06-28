using TypeForge.Core.Models;

namespace TypeForge.Core.TypeConverting;

public record NullableTypeOptions(NullableType NullableType)
{
    public bool IsNullable => NullableType != NullableType.None;
};
