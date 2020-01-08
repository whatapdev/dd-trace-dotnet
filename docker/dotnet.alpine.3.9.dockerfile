# Alpine 3.9
FROM mcr.microsoft.com/dotnet/core/runtime:2.1-alpine3.9
RUN apk add gcompat

# All Alpine 3.x packages
RUN apk add bash curl
RUN apk add libc6-compat