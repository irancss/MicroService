# Dockerfile for Shipping Service
# .NET 8 Microservice with CQRS and Premium Subscription Features
# Build: docker build -t shipping-service:latest .
# Run: docker run -p 8080:80 shipping-service:latest

# ====================
# Build Stage
# ====================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Set work directory
WORKDIR /src

# Copy solution file and project files for dependency resolution
COPY ["Shipping.sln", "./"]
COPY ["ShippingService.API/ShippingService.API.csproj", "ShippingService.API/"]
COPY ["ShippingService.Application/ShippingService.Application.csproj", "ShippingService.Application/"]
COPY ["ShippingService.Domain/ShippingService.Domain.csproj", "ShippingService.Domain/"]
COPY ["ShippingService.Infrastructure/ShippingService.Infrastructure.csproj", "ShippingService.Infrastructure/"]

# Restore NuGet packages
RUN dotnet restore "Shipping.sln"

# Copy the entire source code
COPY . .

# Build the solution
RUN dotnet build "Shipping.sln" --configuration Release --no-restore

# Publish the API project
WORKDIR "/src/ShippingService.API"
RUN dotnet publish "ShippingService.API.csproj" \
    --configuration Release \
    --no-build \
    --output /app/publish \
    --runtime linux-x64 \
    --self-contained false

# ====================
# Runtime Stage
# ====================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

# Install curl for health checks
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

# Set work directory
WORKDIR /app

# Create a non-root user for security
RUN groupadd -r shipping && useradd -r -g shipping shipping

# Copy published files from build stage
COPY --from=build /app/publish .

# Change ownership to non-root user
RUN chown -R shipping:shipping /app

# Switch to non-root user
USER shipping

# Expose ports
EXPOSE 80
EXPOSE 443

# Set environment variables
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:80

# Health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=5s --retries=3 \
    CMD curl -f http://localhost:80/health || exit 1

# Entry point
ENTRYPOINT ["dotnet", "ShippingService.API.dll"]

# Labels for container metadata
LABEL maintainer="Shipping Service Team"
LABEL description="Shipping Service - .NET 8 Microservice with CQRS"
LABEL version="1.0.0"
LABEL org.opencontainers.image.title="Shipping Service"
LABEL org.opencontainers.image.description="Microservice for shipping management with premium subscription features"
LABEL org.opencontainers.image.vendor="Your Company"
LABEL org.opencontainers.image.version="1.0.0"
