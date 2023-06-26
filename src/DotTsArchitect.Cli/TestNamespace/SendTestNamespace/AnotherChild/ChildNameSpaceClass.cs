namespace DotTsArchitect.Cli.TestNamespace.SendTestNamespace.AnotherChild;

public class ChildNameSpaceClass
{
    public string Name { get; set; }
    public string? NullableName { get; set; }
    public int Age { get; set; }
    public int? NullableAge { get; set; }
    public DateTime BirthDate { get; set; }
    public DateTime? NullableBirthDate { get; set; }
    public bool IsAlive { get; set; }
    public bool? NullableIsAlive { get; set; }
    public decimal Money { get; set; }
    public decimal? NullableMoney { get; set; }
    public double Height { get; set; }
    public double? NullableHeight { get; set; }
    public float Weight { get; set; }
    public float? NullableWeight { get; set; }
    public Guid Guid { get; set; }
    public Guid? NullableGuid { get; set; }

    public IEnumerable<string> Names { get; set; }
    public IEnumerable<string>? NullableNames { get; set; }
    public IEnumerable<int> Ages { get; set; }
    public IEnumerable<int>? NullableAges { get; set; }
    public IEnumerable<DateTime> BirthDates { get; set; }
    public IEnumerable<DateTime>? NullableBirthDates { get; set; }
    
    
}
