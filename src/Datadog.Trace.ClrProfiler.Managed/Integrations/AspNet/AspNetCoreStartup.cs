using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datadog.Trace.ClrProfiler.Integrations.AspNet
{
    /// <summary>
    /// Tracing integration for the Startup of AspNetCore applications
    /// </summary>
    public class AspNetCoreStartup
    {
        /// <summary>
        /// Method to run after the original target method.
        /// </summary>
        [InterceptMethod(
            TargetAssembly = "Microsoft.AspNetCore.Hosting",
            TargetType = "Microsoft.AspNetCore.Hosting.WebHostBuilderExtensions",
            TargetMethod = "UseStartup",
            TargetSignatureTypes = new[] { "Microsoft.AspNetCore.Hosting.IWebHostBuilder", "Microsoft.AspNetCore.Hosting.IWebHostBuilder", "System.Type" },
            TargetMinimumVersion = "2",
            TargetMaximumVersion = "3",
            EnforceSignatureCheck = false)]
        public static object AddDatadogTracing(object hostBuilder)
        {
            try
            {
                // TODO: Implement something sane here. Hard dependencies may be a no-no so possibly use reflection here.
                // return Datadog.Trace.Diagnostics.Configuration.WebHostBuilderHelpers.AddDatadogTracing(hostBuilder);
                return hostBuilder;
            }
            catch (Exception)
            {
                return hostBuilder;
            }
        }
    }
}
