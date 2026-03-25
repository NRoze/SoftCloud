# ===========================
# Stage 1: Build
# ===========================
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy global MSBuild props/packages first for caching
COPY Directory.Build.props ./
COPY Directory.Packages.props ./

# Copy project file(s) only to restore
COPY SoftCloud/SoftCloud.csproj SoftCloud/

# Restore dependencies
RUN dotnet restore SoftCloud/SoftCloud.csproj

# Copy all source files
COPY . .

# Build and publish
RUN dotnet build SoftCloud/SoftCloud.csproj -c Release -o /app/build
RUN dotnet publish SoftCloud/SoftCloud.csproj -c Release -o /app/publish /p:UseAppHost=false

# ===========================
# Stage 2: Runtime image
# ===========================
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

# Create a non-root user for security
ARG APP_UID=1000
RUN useradd -u $APP_UID -m appuser
USER appuser

# Copy published app from build stage
COPY --from=build /app/publish .

# Expose default ASP.NET Core port
EXPOSE 8080

# Healthcheck endpoint
HEALTHCHECK --interval=30s --timeout=5s --retries=3 \
  CMD curl -f http://localhost:8080/health || exit 1

# Entry point
ENTRYPOINT ["dotnet", "SoftCloud.dll"]