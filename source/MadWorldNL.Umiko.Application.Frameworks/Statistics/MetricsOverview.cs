using System.Diagnostics.Metrics;

namespace MadWorldNL.Umiko.Statistics;

public static class MetricsOverview
{
    private static readonly Meter Meter = new(TelemetryConstants.Namespace, TelemetryConstants.Version);
    public static readonly Counter<long> CommandCounter = Meter.CreateCounter<long>("command-counter", "Number of commands executed");
    public static readonly Counter<long> QueryCounter = Meter.CreateCounter<long>("query-counter", "Number of queries executed");
    public static readonly Counter<long> EventCounter = Meter.CreateCounter<long>("event-counter", "Number of events executed");
}