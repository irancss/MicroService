@echo off
echo ========================================
echo   Payment Microservice Setup Script
echo ========================================
echo.

REM Check if .NET 8 is installed
echo Checking .NET 8 installation...
dotnet --version | findstr "8." >nul
if %errorlevel% neq 0 (
    echo ERROR: .NET 8.0 SDK is required
    echo Please install from: https://dotnet.microsoft.com/download/dotnet/8.0
    pause
    exit /b 1
)
echo ✓ .NET 8.0 SDK found

REM Check if Docker is running
echo Checking Docker...
docker version >nul 2>&1
if %errorlevel% neq 0 (
    echo ERROR: Docker is not running
    echo Please start Docker Desktop
    pause
    exit /b 1
)
echo ✓ Docker is running

REM Check if EF Core tools are installed
echo Checking Entity Framework tools...
dotnet tool list -g | findstr "dotnet-ef" >nul
if %errorlevel% neq 0 (
    echo Installing Entity Framework tools...
    dotnet tool install --global dotnet-ef
    if %errorlevel% neq 0 (
        echo ERROR: Failed to install EF Core tools
        pause
        exit /b 1
    )
)
echo ✓ Entity Framework tools found

echo.
echo ========================================
echo   Starting Infrastructure Services
echo ========================================
echo.

REM Start dependencies with Docker Compose
echo Starting PostgreSQL, MongoDB, Redis, RabbitMQ...
docker-compose -f docker-compose.dev.yml up -d
if %errorlevel% neq 0 (
    echo ERROR: Failed to start dependencies
    pause
    exit /b 1
)

echo Waiting for services to be ready...
timeout /t 30 /nobreak >nul

echo.
echo ========================================
echo   Building and Preparing Application
echo ========================================
echo.

REM Restore packages
echo Restoring NuGet packages...
dotnet restore
if %errorlevel% neq 0 (
    echo ERROR: Failed to restore packages
    pause
    exit /b 1
)

REM Build solution
echo Building solution...
dotnet build --configuration Release --no-restore
if %errorlevel% neq 0 (
    echo ERROR: Build failed
    pause
    exit /b 1
)

echo.
echo ========================================
echo   Database Setup
echo ========================================
echo.

REM Apply migrations
echo Applying database migrations...
cd Payment.Infrastructure
dotnet ef database update --startup-project ../Payment.API
if %errorlevel% neq 0 (
    echo ERROR: Migration failed
    cd ..
    pause
    exit /b 1
)
cd ..

echo.
echo ========================================
echo   Setup Complete!
echo ========================================
echo.
echo The Payment Microservice has been set up successfully.
echo.
echo Services running:
echo   - PostgreSQL: localhost:5432 (password: 123)
echo   - MongoDB: localhost:27017 (root:123)
echo   - Redis: localhost:6379 (password: 123)
echo   - RabbitMQ: localhost:5672 (guest:guest)
echo   - RabbitMQ Management: http://localhost:15672
echo.
echo To start the application:
echo   dotnet run --project Payment.API
echo.
echo API will be available at:
echo   - HTTP: http://localhost:5000
echo   - Swagger: http://localhost:5000/swagger
echo   - Health: http://localhost:5000/health
echo.
echo To stop infrastructure services:
echo   docker-compose -f docker-compose.dev.yml down
echo.
pause
