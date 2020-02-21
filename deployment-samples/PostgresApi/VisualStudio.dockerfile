#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:2.1-stretch-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Set up Datadog APM
# Perform setup in the "base" target so that the Visual Studio Docker command will include the dd-trace-dotnet setup
ARG TRACER_VERSION=1.13.0
RUN mkdir -p /var/log/datadog
RUN mkdir -p /opt/datadog
RUN curl -LO https://github.com/DataDog/dd-trace-dotnet/releases/download/v${TRACER_VERSION}/datadog-dotnet-apm_${TRACER_VERSION}_amd64.deb
RUN dpkg -i ./datadog-dotnet-apm_${TRACER_VERSION}_amd64.deb

ENV CORECLR_ENABLE_PROFILING=1
ENV CORECLR_PROFILER={846F5F1C-F9AE-4B07-969E-05C26BC060D8}
ENV CORECLR_PROFILER_PATH=/opt/datadog/Datadog.Trace.ClrProfiler.Native.so
ENV DD_INTEGRATIONS=/opt/datadog/integrations.json
ENV DD_DOTNET_TRACER_HOME=/opt/datadog
ENV DD_AGENT_HOST=host.docker.internal

FROM mcr.microsoft.com/dotnet/core/sdk:2.1-stretch AS build
WORKDIR /src
COPY ["PostgresApi/PostgresApi.csproj", "PostgresApi/"]
RUN dotnet restore "PostgresApi/PostgresApi.csproj"
COPY . .
WORKDIR "/src/PostgresApi"
RUN dotnet build "PostgresApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PostgresApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app

# Copy the application binaries and set the entrypoint
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PostgresApi.dll"]