using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Datadog.Trace.Abstractions;
using Datadog.Trace.ExtensionMethods;
using Datadog.Trace.Logging;
using Datadog.Trace.Vendors.Serilog.Events;

namespace Datadog.Trace
{
    /// <summary>
    /// A Span represents a logical unit of work in the system. It may be
    /// related to other spans by parent/children relationships. The span
    /// tracks the duration of an operation as well as associated metadata in
    /// the form of a resource name, a service name, and user defined tags.
    /// </summary>
    public class Span : SpanBase
    {
        private static readonly Vendors.Serilog.ILogger Log = DatadogLogging.For<Span>();
        private static readonly bool IsLogLevelDebugEnabled = Log.IsEnabled(LogEventLevel.Debug);

        internal Span(SpanContext context, DateTimeOffset? start)
        {
            Log.Debug(
                "Span started: [s_id: {0}, p_id: {1}, t_id: {2}]",
                SpanId,
                Context.ParentId,
                TraceId);
        }
    }
}
