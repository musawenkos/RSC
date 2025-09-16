# Base image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 5050

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj files first (for restore)
COPY ["src/RoadSignCapture.Web/RoadSignCapture.Web.csproj", "RoadSignCapture.Web/"]
COPY ["src/RoadSignCapture.Core/RoadSignCapture.Core.csproj", "RoadSignCapture.Core/"]
COPY ["src/RoadSignCapture.Infrastructure/RoadSignCapture.Infrastructure.csproj", "RoadSignCapture.Infrastructure/"]

RUN dotnet restore "RoadSignCapture.Web/RoadSignCapture.Web.csproj"

# Copy the rest of the source
COPY src/ .

WORKDIR "/src/RoadSignCapture.Web"
RUN dotnet build "RoadSignCapture.Web.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "RoadSignCapture.Web.csproj" -c Release -o /app/publish

# Final image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RoadSignCapture.Web.dll"]
