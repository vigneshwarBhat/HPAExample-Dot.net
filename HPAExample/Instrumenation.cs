namespace HPAExample;

using System.Diagnostics.Metrics;


public class Instrumentation : IDisposable
{
    internal const string ActivitySourceName = "HPAExample.AspNetCore";
    internal const string MeterName = "HPAExample.AspNetCore";
    private readonly Meter meter;

    public Instrumentation()
    {
        string? version = typeof(Instrumentation).Assembly.GetName().Version?.ToString();
        meter = new Meter(MeterName, version);
        FreezingDaysCounter = meter.CreateCounter<long>("weather.days.freezing", "The number of days where the temperature is below freezing");
        CartLinesCounter = meter.CreateCounter<long>("hpaexample.Largecart","Number", "Number of lines of cart");
    }


    public Counter<long> FreezingDaysCounter { get; }
    public Counter<long> CartLinesCounter { get; }

    public void Dispose()
    {
       meter.Dispose();
    }
}