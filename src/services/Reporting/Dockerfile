# Dockerfile برای سرویس گزارش‌گیری
# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution file
COPY Reporting.sln ./

# Copy project files
COPY ReportingService.API/ReportingService.API.csproj ReportingService.API/
COPY ReportingService.Application/ReportingService.Application.csproj ReportingService.Application/
COPY ReportingService.Domain/ReportingService.Domain.csproj ReportingService.Domain/
COPY ReportingService.Infrastructure/ReportingService.Infrastructure.csproj ReportingService.Infrastructure/

# Restore dependencies
RUN dotnet restore

# Copy source code
COPY . .

# Build the application
RUN dotnet build "ReportingService.API/ReportingService.API.csproj" -c Release -o /app/build

# Publish the application
RUN dotnet publish "ReportingService.API/ReportingService.API.csproj" -c Release -o /app/publish

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Install curl for health checks
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

# Copy published app
COPY --from=build /app/publish .

# Create non-root user for security
RUN addgroup --system --gid 1001 dotnet \
    && adduser --system --uid 1001 --ingroup dotnet --shell /bin/false dotnet \
    && chown -R dotnet:dotnet /app

USER dotnet

# Environment variables
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:80

# Health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=5s --retries=3 \
    CMD curl -f http://localhost:80/health || exit 1

# Expose port
EXPOSE 80

# Entry point
ENTRYPOINT ["dotnet", "ReportingService.API.dll"]
