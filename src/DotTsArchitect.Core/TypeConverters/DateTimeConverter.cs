namespace DotTsArchitect.Core.TypeConverters;

public class DateTimeConverter : TypeConverter
{
    public override string Convert(Type type)
    {
        return "string";
    }
}