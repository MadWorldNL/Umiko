using System.Diagnostics;

namespace MadWorldNL.Umiko.Statistics;

public static class TracesOverview
{
    public static readonly ActivitySource ActivitySource = new(TelemetryConstants.Namespace);
}