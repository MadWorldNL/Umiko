using System.Diagnostics.Metrics;

namespace MadWorldNL.Umiko.Statistics;

public class MetricsOverview
{
    private static Meter _meter = new Meter("MadWorldNL.Umiko", "1.0.0");
    public static Counter<long> CommandCounter = _meter.CreateCounter<long>("command-counter", "Number of commands executed");
    public static Counter<long> QueryCounter = _meter.CreateCounter<long>("query-counter", "Number of queries executed"); 
}