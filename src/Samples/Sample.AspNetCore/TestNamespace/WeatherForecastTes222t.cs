using System.Collections;
using TypeForge.Core.Attributes;

namespace TypeForge.Cli.TestNamespace;

[TypeForge("weatherForecast.ts")]
public class WeatherForecas222tTest
{
    public DateOnly TestDate { get; set; }

    public int TestTemperatureC { get; set; }
    public int? TestTemperatureCNull { get; set; }
    public int? TestTemperatureCNull2 { get; set; }

    private int? _testTemperatureCNull3 = null;

    public int TestTemperatureF => 32 + (int)(TestTemperatureC / 0.5556);

    public string? TestSummary { get; set; }
    //
    // public ICollection<string> TestCollection { get; set; } = new List<string>();
    //
    // public IEnumerable<string> TestIEnumerable { get; set; } = new List<string>();
    //
    // public IList<string> TestIList { get; set; } = new List<string>();
    //
    // public List<string> TestList { get; set; } = new List<string>();
    //
    // public IReadOnlyCollection<string> TestIReadOnlyCollection { get; set; } = new List<string>();
    //
    // public IReadOnlyList<string> TestIReadOnlyList { get; set; } = new List<string>();
    //
    // public ICollection TestICollection { get; set; } = new List<string>();
    //
    // public IList TestIList2 { get; set; } = new List<string>();
}
