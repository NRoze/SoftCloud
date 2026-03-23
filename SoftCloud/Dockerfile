FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy csproj and restore as distinct layers
COPY ["SoftCloud.csproj", "./"]
RUN dotnet restore "SoftCloud.csproj"

# Copy everything else and build
COPY . .
RUN dotnet build "SoftCloud.csproj" -c Release -o /app/build
RUN dotnet publish "SoftCloud.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

# Best Practice: Run as non-root user for security (available in .NET 8+)
USER $APP_UID

# Install curl for the health check (if using Alpine, use: apk add --no-cache curl)
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

COPY --from=build /app/publish .

# Expose the default ASP.NET Core port (8080 for non-root)
EXPOSE 8080

HEALTHCHECK --interval=30s --timeout=5s --retries=3 \
  CMD curl -f http://localhost:5114/health || exit 1

ENTRYPOINT ["dotnet", "SoftCloud.dll"]
