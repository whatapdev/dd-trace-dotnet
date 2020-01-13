using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Datadog.Trace.Diagnostics.Configuration
{
    public static class WebHostBuilderHelpers
    {
        internal static object AddDatadogTracing(object hostBuilder)
        {
            var webHostBuilder = hostBuilder as IWebHostBuilder;
            if (webHostBuilder == null)
            {
                throw new ArgumentException($"{nameof(hostBuilder)} must be implement the IWebHostBuilder interface");
            }

            return webHostBuilder.ConfigureServices((services) => services.AddDatadogTracing());
        }
    }
}
