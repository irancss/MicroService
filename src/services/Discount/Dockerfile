# Use the official .NET 8 SDK image for building
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj files and restore dependencies
COPY ["DiscountService.API/DiscountService.API.csproj", "DiscountService.API/"]
COPY ["DiscountService.Application/DiscountService.Application.csproj", "DiscountService.Application/"]
COPY ["DiscountService.Domain/DiscountService.Domain.csproj", "DiscountService.Domain/"]
COPY ["DiscountService.Infrastructure/DiscountService.Infrastructure.csproj", "DiscountService.Infrastructure/"]

RUN dotnet restore "DiscountService.API/DiscountService.API.csproj"

# Copy the rest of the source code
COPY . .

# Build the application
WORKDIR "/src/DiscountService.API"
RUN dotnet build "DiscountService.API.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "DiscountService.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Create the runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Create a non-root user for security
RUN adduser --disabled-password --gecos '' appuser

# Create logs directory and set permissions
RUN mkdir -p /app/logs && chown appuser:appuser /app/logs

# Copy the published application
COPY --from=publish /app/publish .

# Change ownership of the app directory
RUN chown -R appuser:appuser /app

# Switch to the non-root user
USER appuser

# Expose the port
EXPOSE 8080

# Set environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

# Start the application
ENTRYPOINT ["dotnet", "DiscountService.API.dll"]
