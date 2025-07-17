# Multi-stage Dockerfile for Cart Microservice
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files and restore dependencies
COPY ["CartService.API/CartService.API.csproj", "CartService.API/"]
COPY ["CartService.Application/CartService.Application.csproj", "CartService.Application/"]
COPY ["CartService.Infrastructure/CartService.Infrastructure.csproj", "CartService.Infrastructure/"]
COPY ["Cart.Domain/CartService.Domain.csproj", "Cart.Domain/"]

RUN dotnet restore "CartService.API/CartService.API.csproj"

# Copy source code
COPY . .

# Build application
WORKDIR "/src/CartService.API"
RUN dotnet build "CartService.API.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "CartService.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final stage
FROM base AS final
WORKDIR /app

# Create non-root user for security
RUN adduser --disabled-password --gecos '' appuser && \
    chown -R appuser /app
USER appuser

COPY --from=publish /app/publish .

# Create logs directory
RUN mkdir -p /app/logs

ENTRYPOINT ["dotnet", "CartService.API.dll"]
