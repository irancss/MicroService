# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files
COPY SearchService.API/SearchService.API.csproj SearchService.API/
COPY SearchService.Application/SearchService.Application.csproj SearchService.Application/
COPY SearchService.Domain/SearchService.Domain.csproj SearchService.Domain/
COPY SearchService.Infrastructure/SearchService.Infrastructure.csproj SearchService.Infrastructure/

# Restore dependencies
RUN dotnet restore SearchService.API/SearchService.API.csproj

# Copy source code
COPY . .

# Build the application
WORKDIR /src/SearchService.API
RUN dotnet build SearchService.API.csproj -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish SearchService.API.csproj -c Release -o /app/publish --no-restore

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Install curl for health checks
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

# Create non-root user
RUN addgroup --system --gid 1001 searchapi && \
    adduser --system --uid 1001 --gid 1001 --shell /bin/false searchapi

# Copy published application
COPY --from=publish /app/publish .

# Set ownership
RUN chown -R searchapi:searchapi /app

# Switch to non-root user
USER searchapi

# Expose port
EXPOSE 8080

# Health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=30s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

# Set entry point
ENTRYPOINT ["dotnet", "SearchService.API.dll"]
