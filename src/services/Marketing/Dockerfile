# Marketing Service Dockerfile
# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj files for better Docker layer caching
COPY ["MarketingService.API/MarketingService.API.csproj", "MarketingService.API/"]
COPY ["MarketingService.Application/MarketingService.Application.csproj", "MarketingService.Application/"]
COPY ["MarketingService.Domain/MarketingService.Domain.csproj", "MarketingService.Domain/"]
COPY ["MarketingService.Infrastructure/MarketingService.Infrastructure.csproj", "MarketingService.Infrastructure/"]

# Restore packages
RUN dotnet restore "MarketingService.API/MarketingService.API.csproj"

# Copy source code
COPY . .

# Build and publish
WORKDIR "/src/MarketingService.API"
RUN dotnet build "MarketingService.API.csproj" -c Release -o /app/build
RUN dotnet publish "MarketingService.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Install necessary packages for PostgreSQL
RUN apt-get update && apt-get install -y \
    curl \
    && rm -rf /var/lib/apt/lists/*

# Copy published app
COPY --from=build /app/publish .

# Create a non-root user
RUN adduser --disabled-password --gecos '' appuser && chown -R appuser /app
USER appuser

# Expose ports
EXPOSE 8080
EXPOSE 8081

# Set environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# Health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=5s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "MarketingService.API.dll"]
