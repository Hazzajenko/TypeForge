namespace DotTsArchitect.Core.TypeConverters;

public class StringConverter : TypeConverter
{
    public override string Convert(Type type)
    {
        return "string";
    }
}