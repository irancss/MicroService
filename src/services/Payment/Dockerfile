# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj files and restore dependencies
COPY Payment.sln ./
COPY Payment.API/Payment.API.csproj Payment.API/
COPY Payment.Application/Payment.Application.csproj Payment.Application/
COPY Payment.Domain/Payment.Domain.csproj Payment.Domain/
COPY Payment.Infrastructure/Payment.Infrastructure.csproj Payment.Infrastructure/

RUN dotnet restore

# Copy source code and build
COPY . .
WORKDIR /src/Payment.API
RUN dotnet build -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Install curl for health checks
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

# Create non-root user
RUN adduser --disabled-password --gecos '' appuser && chown -R appuser /app
USER appuser

# Copy published app
COPY --from=publish /app/publish .

# Expose port
EXPOSE 8080

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
  CMD curl -f http://localhost:8080/health || exit 1

# Entry point
ENTRYPOINT ["dotnet", "Payment.API.dll"]
