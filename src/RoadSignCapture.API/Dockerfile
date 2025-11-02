# Base image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 5040

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj files for restore
COPY ["src/RoadSignCapture.API/RoadSignCapture.API.csproj", "RoadSignCapture.API/"]
COPY ["src/RoadSignCapture.Core/RoadSignCapture.Core.csproj", "RoadSignCapture.Core/"]
COPY ["src/RoadSignCapture.Infrastructure/RoadSignCapture.Infrastructure.csproj", "RoadSignCapture.Infrastructure/"]

RUN dotnet restore "RoadSignCapture.API/RoadSignCapture.API.csproj"

# Copy all remaining source files
COPY src/ .

# Build
WORKDIR "/src/RoadSignCapture.API"
RUN dotnet build "RoadSignCapture.API.csproj" -c Release -o /app/build

# Publish
FROM build AS publish
RUN dotnet publish "RoadSignCapture.API.csproj" -c Release -o /app/publish

# Final image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
CMD ["bash", "-c", "until (echo PING | nc ${REDIS_HOST} ${REDIS_PORT} | grep PONG); do echo 'Waiting for Redis...'; sleep 2; done && dotnet RoadSignCapture.API.dll"]
