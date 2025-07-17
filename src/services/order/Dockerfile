# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
WORKDIR /src

# Copy csproj files and restore dependencies
COPY ["OrderService.API/OrderService.API.csproj", "OrderService.API/"]
COPY ["OrderService.Application/OrderService.Application.csproj", "OrderService.Application/"]
COPY ["OrderService.Core/OrderService.Domain.csproj", "OrderService.Core/"]
COPY ["OrderService.Infrastructure/OrderService.Infrastructure.csproj", "OrderService.Infrastructure/"]

# Restore packages
RUN dotnet restore "OrderService.API/OrderService.API.csproj"

# Copy all source code
COPY . .

# Build application
WORKDIR "/src/OrderService.API"
RUN dotnet build "OrderService.API.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "OrderService.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS final
WORKDIR /app

# Install curl for health checks
RUN apk --no-cache add curl

# Create non-root user
RUN addgroup -g 1000 appgroup && adduser -u 1000 -G appgroup -s /bin/sh -D appuser
USER appuser

# Copy published app
COPY --from=publish /app/publish .

# Create logs directory
USER root
RUN mkdir -p /app/logs && chown -R appuser:appgroup /app/logs
USER appuser

# Expose ports
EXPOSE 8080 8081

# Environment variables
ENV ASPNETCORE_URLS=http://+:8080;https://+:8081
ENV ASPNETCORE_ENVIRONMENT=Production
ENV DOTNET_RUNNING_IN_CONTAINER=true
ENV DOTNET_USE_POLLING_FILE_WATCHER=true

# Health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=60s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

# Entry point
ENTRYPOINT ["dotnet", "OrderService.API.dll"]
