#if !NET452

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Datadog.Trace.TestHelpers;
using Xunit;
using Xunit.Abstractions;

namespace Datadog.Trace.ClrProfiler.IntegrationTests
{
    public class GraphQLClientTests : TestHelper
    {
        private readonly GraphQLTests _graphQLServer;

        public GraphQLClientTests(ITestOutputHelper output)
            : base("GraphQL.Client", output)
        {
            _graphQLServer = new GraphQLTests(output);
        }

        [Fact]
        [Trait("Category", "EndToEnd")]
        public void SubmitsTraces()
        {
            int agentPort = TcpPortProvider.GetOpenPort();
            int aspNetCorePort = TcpPortProvider.GetOpenPort();

            using (var agent = new MockTracerAgent(agentPort))
            using (Process serverProcess = _graphQLServer.StartSample(agent.Port, arguments: null, packageVersion: string.Empty, aspNetCorePort: aspNetCorePort))
            {
                // Start GraphQL server
                var wh = new EventWaitHandle(false, EventResetMode.AutoReset);

                serverProcess.OutputDataReceived += (sender, args) =>
                {
                    if (args.Data != null)
                    {
                        if (args.Data.Contains("Now listening on:") || args.Data.Contains("Unable to start Kestrel"))
                        {
                            wh.Set();
                        }

                        Output.WriteLine($"[webserver][stdout] {args.Data}");
                    }
                };
                serverProcess.BeginOutputReadLine();

                serverProcess.ErrorDataReceived += (sender, args) =>
                {
                    if (args.Data != null)
                    {
                        Output.WriteLine($"[webserver][stderr] {args.Data}");
                    }
                };
                serverProcess.BeginErrorReadLine();

                // wait for server to start
                wh.WaitOne(5000);

                // Start GraphQL client
                using (ProcessResult processResult = RunSampleAndWaitForExit(agent.Port, packageVersion: string.Empty))
                {
                    Assert.True(processResult.ExitCode >= 0, $"Process exited with code {processResult.ExitCode}");
                    /*
                    var graphQLValidateSpans = agent.WaitForSpans(_expectedGraphQLValidateSpanCount, operationName: _graphQLValidateOperationName, returnAllOperations: false)
                                     .GroupBy(s => s.SpanId)
                                     .Select(grp => grp.First())
                                     .OrderBy(s => s.Start);
                    var graphQLExecuteSpans = agent.WaitForSpans(_expectedGraphQLExecuteSpanCount, operationName: _graphQLExecuteOperationName, returnAllOperations: false)
                                     .GroupBy(s => s.SpanId)
                                     .Select(grp => grp.First())
                                     .OrderBy(s => s.Start);
                                     */
                }

                if (!serverProcess.HasExited)
                {
                    serverProcess.Kill();
                }

                // var spans = graphQLValidateSpans.Concat(graphQLExecuteSpans).ToList();
                // WebServerTestHelpers.AssertExpectationsMet(_expectations, spans);
            }
        }
    }
}

#endif
