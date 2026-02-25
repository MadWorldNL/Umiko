using System.Diagnostics;
using System.Text;

namespace MadWorldNL.Umiko.Consumers;

internal static class ActivityContextExtensions
{
    internal static ActivityContext ToActivityContext(this IDictionary<string, object?>? headers)
    {
        if (headers is null) return default;

        if (!headers.TryGetValue("traceparent", out var traceparentObj)) return default;

        var traceparent = traceparentObj is byte[] bytes
            ? Encoding.UTF8.GetString(bytes)
            : traceparentObj?.ToString();

        string? tracestate = null;
        if (headers.TryGetValue("tracestate", out var tracestateObj))
            tracestate = tracestateObj is byte[] tsBytes
                ? Encoding.UTF8.GetString(tsBytes)
                : tracestateObj?.ToString();

        return ActivityContext.TryParse(traceparent, tracestate, isRemote: true, out var context)
            ? context
            : default;
    }
}