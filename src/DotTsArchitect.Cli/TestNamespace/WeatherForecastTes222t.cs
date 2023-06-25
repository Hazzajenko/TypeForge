using DotTsArchitect.Core.Attributes;

namespace DotTsArchitect.Cli.TestNamespace;

[TsArchitect("weatherForecast.ts")]
public class WeatherForecas222tTest
{
    public DateOnly TestDate { get; set; }

    public int TestTemperatureC { get; set; }

    public int TestTemperatureF => 32 + (int)(TestTemperatureC / 0.5556);

    public string? TestSummary { get; set; }
}
