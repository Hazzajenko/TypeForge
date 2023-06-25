namespace DotTsArchitect.Core.TypeConverters;

public class NumberConverter : TypeConverter
{
    public override string Convert(Type type)
    {
        return "number";
    }
}