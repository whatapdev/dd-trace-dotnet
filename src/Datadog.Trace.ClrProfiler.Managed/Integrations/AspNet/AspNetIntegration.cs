#if !NETSTANDARD2_0
using System;
using System.Linq;
using System.Reflection;
using System.Web;
using Datadog.Trace.AspNet;
using Datadog.Trace.ClrProfiler.Emit;
using Datadog.Trace.Logging;

namespace Datadog.Trace.ClrProfiler.Integrations.AspNet
{
    /// <summary>
    /// Instrumentation wrapper for AspNet
    /// </summary>
    public static class AspNetIntegration
    {
        private const string IntegrationName = "AspNet";
        private const string OperationName = "aspnet.request";
        private const string MinimumVersion = "1.0.0.0";
        private const string MaximumVersion = "1.0.0.0";

        private const string AssemblyName = "Microsoft.Web.Infrastructure";
        private const string BuildManagerTypeName = "Microsoft.Web.Infrastructure.DynamicModuleHelper.DynamicModuleUtility";

        private static readonly Vendors.Serilog.ILogger Log = DatadogLogging.GetLogger(typeof(AspNetIntegration));

        private static bool registeredHttpModule = false;

        /// <summary>
        /// Wrapper method used to instrument Microsoft.Web.Infrastructure.DynamicModuleHelper.DynamicModuleUtility.RegisterModule
        /// </summary>
        /// <param name="moduleType">The module type argument</param>
        /// <param name="opCode">The OpCode used in the original method call.</param>
        /// <param name="mdToken">The mdToken of the original method call.</param>
        /// <param name="moduleVersionPtr">A pointer to the module version GUID.</param>
        [InterceptMethod(
            CallerAssembly = "System.Web.Optimization",
            TargetAssembly = AssemblyName,
            TargetType = BuildManagerTypeName,
            TargetSignatureTypes = new[] { ClrNames.Void, "System.Type" },
            TargetMinimumVersion = MinimumVersion,
            TargetMaximumVersion = MaximumVersion)]
        public static void RegisterModule(
            object moduleType,
            int opCode,
            int mdToken,
            long moduleVersionPtr)
        {
            if (!registeredHttpModule)
            {
                // The whole point of instrumenting a method so early on in the application load process
                // is to register our HttpModule.
                HttpApplication.RegisterModule(typeof(TracingHttpModule));
                registeredHttpModule = true;
            }

            Action<object> instrumentedMethod;
            Type concreteType = null;

            try
            {
                var targetAssembly = Assembly.Load("Microsoft.Web.Infrastructure, Version=1.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35");
                concreteType = targetAssembly.GetType("Microsoft.Web.Infrastructure.DynamicModuleHelper.DynamicModuleUtility", throwOnError: true);
            }
            catch (Exception ex)
            {
                Log.ErrorRetrievingMethod(
                    exception: ex,
                    moduleVersionPointer: moduleVersionPtr,
                    mdToken: mdToken,
                    opCode: opCode,
                    instrumentedType: BuildManagerTypeName,
                    methodName: nameof(RegisterModule),
                    instanceType: null,
                    relevantArguments: new[] { concreteType?.AssemblyQualifiedName });
                throw;
            }

            try
            {
                instrumentedMethod =
                    MethodBuilder<Action<object>>
                       .Start(moduleVersionPtr, mdToken, opCode, nameof(RegisterModule))
                       .WithParameters(moduleType)
                       .WithConcreteType(concreteType)
                       .Build();
            }
            catch (Exception ex)
            {
                Log.ErrorRetrievingMethod(
                    exception: ex,
                    moduleVersionPointer: moduleVersionPtr,
                    mdToken: mdToken,
                    opCode: opCode,
                    null,
                    methodName: nameof(RegisterModule));
                throw;
            }

            instrumentedMethod(moduleType);
        }
    }
}
#endif
