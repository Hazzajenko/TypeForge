using DotTsArchitect.Core.Configuration;

namespace DotTsArchitect.Core.Models;

// public class TypeModels
// {
//
// }

public record NullableTypeOptions(NullableType NullableType)
{
    public bool IsNullable => NullableType != NullableType.None;
};
