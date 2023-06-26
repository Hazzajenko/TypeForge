using TypeForge.Core.Configuration;

namespace TypeForge.Core.Models;

// public class TypeModels
// {
//
// }

public record NullableTypeOptions(NullableType NullableType)
{
    public bool IsNullable => NullableType != NullableType.None;
};
