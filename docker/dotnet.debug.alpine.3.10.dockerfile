# Alpine 3.10
FROM mcr.microsoft.com/dotnet/core/runtime:2.1-alpine3.10

#ENV CORECLR_ENABLE_PROFILING=1
#ENV CORECLR_PROFILER={846F5F1C-F9AE-4B07-969E-05C26BC060D8}
#ENV CORECLR_PROFILER_PATH=/src/Datadog.Trace.ClrProfiler.Native/obj/Debug/x64/Datadog.Trace.ClrProfiler.Native.so
#ENV DD_DOTNET_TRACER_HOME=/
#ENV DD_INTEGRATIONS=integrations.json

# Add requirements for running Datadog CLR Profiler
RUN apk add gcompat
RUN apk add bash curl
RUN apk add libc6-compat

# Install the packages required to get the vs debugger server running
RUN apk --no-cache add curl procps
RUN curl -sSL https://aka.ms/getvsdbgsh | bash /dev/stdin -v latest -l /publish/vsdbg;

# This assumes everything is built already
COPY samples/Samples.HttpMessageHandler/bin/Debug/netcoreapp2.1/publish/ ./project/samples/Samples.HttpMessageHandler/bin/Debug/netcoreapp2.1/publish/
COPY src/Datadog.Trace.ClrProfiler.Native/obj/Debug/x64/ ./project/src/Datadog.Trace.ClrProfiler.Native/obj/Debug/x64/
COPY docker/ ./project/docker
COPY integrations.json ./project/integrations.json

#ENTRYPOINT ["ls", "/samples/Samples.HttpMessageHandler/bin/Debug/netcoreapp2.1/publish/profiler-lib"]
ENTRYPOINT ["bash", "-c", "/project/docker/with-profiler-logs.bash /project/docker/with-profiler.bash dotnet /project/samples/Samples.HttpMessageHandler/bin/Debug/netcoreapp2.1/publish/Samples.HttpMessageHandler.dll"]