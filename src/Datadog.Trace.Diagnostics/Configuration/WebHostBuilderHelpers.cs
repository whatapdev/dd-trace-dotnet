using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Datadog.Trace.Diagnostics.Configuration
{
    public static class WebHostBuilderHelpers
    {
        public static IWebHostBuilder AddDatadogTracing(IWebHostBuilder arg)
        {
            return arg.ConfigureServices((services) => services.AddDatadogTracing());
        }
    }
}
