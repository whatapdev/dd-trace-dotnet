# Alpine 3.10
FROM mcr.microsoft.com/dotnet/core/runtime:2.1-alpine3.10
RUN apk add gcompat

# All Alpine 3.x packages
RUN apk add bash curl
RUN apk add libc6-compat
