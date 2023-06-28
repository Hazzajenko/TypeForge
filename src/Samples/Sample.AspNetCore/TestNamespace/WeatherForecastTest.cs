using TypeForge.Core.Attributes;

namespace TypeForge.Cli.TestNamespace;

[TypeForge("weatherForecast.ts")]
public class WeatherForecastTest
{
    public DateOnly TestDate { get; set; }

    public int TestTemperatureC { get; set; }

    public int TestTemperatureF => 32 + (int)(TestTemperatureC / 0.5556);

    public string? TestSummary { get; set; }
}
