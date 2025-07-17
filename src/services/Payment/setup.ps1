#!/usr/bin/env pwsh

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "   Payment Microservice Setup Script" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Check if .NET 8 is installed
Write-Host "Checking .NET 8 installation..." -ForegroundColor Yellow
try {
    $dotnetVersion = dotnet --version
    if ($dotnetVersion -notmatch "^8\.") {
        throw "Wrong version"
    }
    Write-Host "✓ .NET 8.0 SDK found ($dotnetVersion)" -ForegroundColor Green
}
catch {
    Write-Host "ERROR: .NET 8.0 SDK is required" -ForegroundColor Red
    Write-Host "Please install from: https://dotnet.microsoft.com/download/dotnet/8.0" -ForegroundColor Yellow
    exit 1
}

# Check if Docker is running
Write-Host "Checking Docker..." -ForegroundColor Yellow
try {
    docker version | Out-Null
    Write-Host "✓ Docker is running" -ForegroundColor Green
}
catch {
    Write-Host "ERROR: Docker is not running" -ForegroundColor Red
    Write-Host "Please start Docker Desktop" -ForegroundColor Yellow
    exit 1
}

# Check if EF Core tools are installed
Write-Host "Checking Entity Framework tools..." -ForegroundColor Yellow
$efInstalled = dotnet tool list -g | Select-String "dotnet-ef"
if (-not $efInstalled) {
    Write-Host "Installing Entity Framework tools..." -ForegroundColor Yellow
    dotnet tool install --global dotnet-ef
    if ($LASTEXITCODE -ne 0) {
        Write-Host "ERROR: Failed to install EF Core tools" -ForegroundColor Red
        exit 1
    }
    Write-Host "✓ Entity Framework tools installed" -ForegroundColor Green
} else {
    Write-Host "✓ Entity Framework tools found" -ForegroundColor Green
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "   Starting Infrastructure Services" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Start dependencies with Docker Compose
Write-Host "Starting PostgreSQL, MongoDB, Redis, RabbitMQ..." -ForegroundColor Yellow
docker-compose -f docker-compose.dev.yml up -d
if ($LASTEXITCODE -ne 0) {
    Write-Host "ERROR: Failed to start dependencies" -ForegroundColor Red
    exit 1
}

Write-Host "Waiting for services to be ready..." -ForegroundColor Yellow
Start-Sleep -Seconds 30

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "   Building and Preparing Application" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Restore packages
Write-Host "Restoring NuGet packages..." -ForegroundColor Yellow
dotnet restore
if ($LASTEXITCODE -ne 0) {
    Write-Host "ERROR: Failed to restore packages" -ForegroundColor Red
    exit 1
}

# Build solution
Write-Host "Building solution..." -ForegroundColor Yellow
dotnet build --configuration Release --no-restore
if ($LASTEXITCODE -ne 0) {
    Write-Host "ERROR: Build failed" -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "   Database Setup" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Apply migrations
Write-Host "Applying database migrations..." -ForegroundColor Yellow
Set-Location Payment.Infrastructure
dotnet ef database update --startup-project ../Payment.API
if ($LASTEXITCODE -ne 0) {
    Write-Host "ERROR: Migration failed" -ForegroundColor Red
    Set-Location ..
    exit 1
}
Set-Location ..

Write-Host ""
Write-Host "========================================" -ForegroundColor Green
Write-Host "   Setup Complete!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host ""
Write-Host "The Payment Microservice has been set up successfully." -ForegroundColor Green
Write-Host ""
Write-Host "Services running:" -ForegroundColor Cyan
Write-Host "  - PostgreSQL: localhost:5432 (password: 123)" -ForegroundColor White
Write-Host "  - MongoDB: localhost:27017 (root:123)" -ForegroundColor White
Write-Host "  - Redis: localhost:6379 (password: 123)" -ForegroundColor White
Write-Host "  - RabbitMQ: localhost:5672 (guest:guest)" -ForegroundColor White
Write-Host "  - RabbitMQ Management: http://localhost:15672" -ForegroundColor White
Write-Host ""
Write-Host "To start the application:" -ForegroundColor Cyan
Write-Host "  dotnet run --project Payment.API" -ForegroundColor Yellow
Write-Host ""
Write-Host "API will be available at:" -ForegroundColor Cyan
Write-Host "  - HTTP: http://localhost:5000" -ForegroundColor White
Write-Host "  - Swagger: http://localhost:5000/swagger" -ForegroundColor White
Write-Host "  - Health: http://localhost:5000/health" -ForegroundColor White
Write-Host ""
Write-Host "To stop infrastructure services:" -ForegroundColor Cyan
Write-Host "  docker-compose -f docker-compose.dev.yml down" -ForegroundColor Yellow
Write-Host ""

# Optionally start the application
$startApp = Read-Host "Start the application now? (y/N)"
if ($startApp -eq "y" -or $startApp -eq "Y") {
    Write-Host ""
    Write-Host "Starting Payment API..." -ForegroundColor Green
    dotnet run --project Payment.API
}
