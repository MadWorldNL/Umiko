namespace MadWorldNL.Umiko.Statistics;

public static class TelemetryConstants
{
    private const string ComponentName = "MadWorldNL";
    private const string ProductName = "Umiko";
    public const string Version = "1.0.0";
    
    public static string Namespace => $"{ComponentName}.{ProductName}";
}