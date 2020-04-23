using System;

namespace Datadog.Trace
{
    internal interface ITraceContext
    {
        DateTimeOffset UtcNow { get; }

        SamplingPriority? SamplingPriority { get; set; }

        AbstractSpan RootSpan { get; }

        void AddSpan(AbstractSpan span);

        void CloseSpan(AbstractSpan span);

        void LockSamplingPriority();
    }
}
