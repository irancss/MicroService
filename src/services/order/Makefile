# Order Microservice Makefile
# Clean Architecture with CQRS and Saga Pattern
# Technologies: .NET 8, MassTransit, RabbitMQ, PostgreSQL

# Variables
PROJECT_NAME := OrderService
SOLUTION_FILE := order.sln
API_PROJECT := OrderService.API/OrderService.API.csproj
INFRASTRUCTURE_PROJECT := OrderService.Infrastructure/OrderService.Infrastructure.csproj
APPLICATION_PROJECT := OrderService.Application/OrderService.Application.csproj
CORE_PROJECT := OrderService.Core/OrderService.Domain.csproj

# Docker Configuration
DOCKER_IMAGE_NAME := order-service
DOCKER_TAG := latest
DOCKER_REGISTRY := localhost:5000

# Database Configuration
DB_CONNECTION_STRING := "Host=localhost;Database=OrderServiceDB;Username=postgres;Password=postgres"
MIGRATION_NAME := InitialCreate

# RabbitMQ Configuration
RABBITMQ_HOST := localhost
RABBITMQ_PORT := 5672
RABBITMQ_USER := guest
RABBITMQ_PASS := guest

# Build Configuration
BUILD_CONFIG := Release
OUTPUT_DIR := ./publish

.PHONY: help clean restore build test run run-dev migrate docker-build docker-run docker-compose-up docker-compose-down setup-dev install-tools format lint security-scan

# Default target
help: ## Show this help message
	@echo "Order Microservice - Available Commands:"
	@echo "========================================="
	@grep -E '^[a-zA-Z_-]+:.*?## .*$$' $(MAKEFILE_LIST) | sort | awk 'BEGIN {FS = ":.*?## "}; {printf "\033[36m%-20s\033[0m %s\n", $$1, $$2}'

# Development Commands
setup-dev: ## Setup development environment
	@echo "Setting up development environment..."
	dotnet --version
	dotnet tool restore
	$(MAKE) restore
	$(MAKE) install-tools
	@echo "Development environment setup complete!"

install-tools: ## Install required development tools
	@echo "Installing development tools..."
	dotnet tool install --global dotnet-ef --version 8.0.0 || true
	dotnet tool install --global dotnet-format || true
	dotnet tool install --global security-scan || true
	@echo "Tools installation complete!"

# Build Commands
clean: ## Clean build artifacts
	@echo "Cleaning build artifacts..."
	dotnet clean $(SOLUTION_FILE)
	Remove-Item -Recurse -Force ./publish -ErrorAction SilentlyContinue || true
	Remove-Item -Recurse -Force ./**/bin -ErrorAction SilentlyContinue || true
	Remove-Item -Recurse -Force ./**/obj -ErrorAction SilentlyContinue || true
	@echo "Clean completed!"

restore: ## Restore NuGet packages
	@echo "Restoring packages..."
	dotnet restore $(SOLUTION_FILE)
	@echo "Package restore completed!"

build: clean restore ## Build the solution
	@echo "Building solution..."
	dotnet build $(SOLUTION_FILE) --configuration $(BUILD_CONFIG) --no-restore
	@echo "Build completed!"

build-release: clean restore ## Build for production release
	@echo "Building for production..."
	dotnet build $(SOLUTION_FILE) --configuration Release --no-restore
	@echo "Production build completed!"

publish: build-release ## Publish application
	@echo "Publishing application..."
	dotnet publish $(API_PROJECT) --configuration $(BUILD_CONFIG) --output $(OUTPUT_DIR) --no-build
	@echo "Publish completed! Output: $(OUTPUT_DIR)"

# Test Commands
test: ## Run all tests
	@echo "Running tests..."
	dotnet test $(SOLUTION_FILE) --configuration $(BUILD_CONFIG) --no-build --verbosity normal
	@echo "Tests completed!"

test-coverage: ## Run tests with coverage
	@echo "Running tests with coverage..."
	dotnet test $(SOLUTION_FILE) --collect:"XPlat Code Coverage" --results-directory ./TestResults
	@echo "Test coverage completed! Check ./TestResults"

# Run Commands
run: build ## Run the application
	@echo "Starting Order Service..."
	@echo "API will be available at: https://localhost:7001"
	@echo "Swagger UI: https://localhost:7001/swagger"
	cd OrderService.API && dotnet run --configuration $(BUILD_CONFIG)

run-dev: ## Run in development mode with hot reload
	@echo "Starting Order Service in development mode..."
	@echo "Hot reload enabled - API: https://localhost:7001"
	cd OrderService.API && dotnet watch run

run-prod: publish ## Run published application
	@echo "Starting published application..."
	cd $(OUTPUT_DIR) && dotnet OrderService.API.dll

# Database Commands
migrate: ## Run database migrations
	@echo "Running database migrations..."
	cd OrderService.Infrastructure && dotnet ef database update --connection $(DB_CONNECTION_STRING)
	@echo "Migrations completed!"

migration-add: ## Add new migration (usage: make migration-add MIGRATION_NAME=MigrationName)
	@echo "Adding migration: $(MIGRATION_NAME)"
	cd OrderService.Infrastructure && dotnet ef migrations add $(MIGRATION_NAME)
	@echo "Migration $(MIGRATION_NAME) added!"

migration-script: ## Generate SQL script for migrations
	@echo "Generating migration script..."
	cd OrderService.Infrastructure && dotnet ef migrations script --output ../migration-script.sql
	@echo "Script generated: migration-script.sql"

db-drop: ## Drop database (DANGEROUS)
	@echo "WARNING: This will drop the database!"
	@echo "Press Ctrl+C to cancel or Enter to continue..."
	@read
	cd OrderService.Infrastructure && dotnet ef database drop --force --connection $(DB_CONNECTION_STRING)

# Docker Commands
docker-build: ## Build Docker image
	@echo "Building Docker image..."
	docker build -t $(DOCKER_IMAGE_NAME):$(DOCKER_TAG) .
	@echo "Docker image built: $(DOCKER_IMAGE_NAME):$(DOCKER_TAG)"

docker-run: ## Run Docker container
	@echo "Running Docker container..."
	docker run -d --name order-service-container \
		-p 8080:8080 \
		-e ASPNETCORE_ENVIRONMENT=Production \
		-e ConnectionStrings__DefaultConnection=$(DB_CONNECTION_STRING) \
		-e RabbitMQ__Host=$(RABBITMQ_HOST) \
		$(DOCKER_IMAGE_NAME):$(DOCKER_TAG)
	@echo "Container started! API available at: http://localhost:8080"

docker-stop: ## Stop Docker container
	@echo "Stopping Docker container..."
	docker stop order-service-container || true
	docker rm order-service-container || true
	@echo "Container stopped!"

docker-push: docker-build ## Push Docker image to registry
	@echo "Pushing to registry..."
	docker tag $(DOCKER_IMAGE_NAME):$(DOCKER_TAG) $(DOCKER_REGISTRY)/$(DOCKER_IMAGE_NAME):$(DOCKER_TAG)
	docker push $(DOCKER_REGISTRY)/$(DOCKER_IMAGE_NAME):$(DOCKER_TAG)
	@echo "Image pushed to registry!"

# Docker Compose Commands
docker-compose-up: ## Start services with Docker Compose
	@echo "Starting services with Docker Compose..."
	docker-compose up -d
	@echo "Services started! Check docker-compose ps"

docker-compose-down: ## Stop Docker Compose services
	@echo "Stopping Docker Compose services..."
	docker-compose down
	@echo "Services stopped!"

docker-compose-logs: ## View Docker Compose logs
	docker-compose logs -f

# Code Quality Commands
format: ## Format code
	@echo "Formatting code..."
	dotnet format $(SOLUTION_FILE)
	@echo "Code formatting completed!"

lint: ## Run linter
	@echo "Running linter..."
	dotnet format $(SOLUTION_FILE) --verify-no-changes --verbosity diagnostic
	@echo "Linting completed!"

security-scan: ## Run security scan
	@echo "Running security scan..."
	dotnet list package --vulnerable --include-transitive
	@echo "Security scan completed!"

# Health Check Commands
health-check: ## Check service health
	@echo "Checking service health..."
	curl -f http://localhost:8080/health || echo "Service is not responding"

# Monitoring Commands
logs: ## Show application logs
	@echo "Showing recent logs..."
	Get-Content -Path "./OrderService.API/logs/logs-*" -Tail 50 -Wait

logs-errors: ## Show error logs only
	@echo "Showing error logs..."
	Select-String -Path "./OrderService.API/logs/logs-*" -Pattern "ERROR|FATAL" | Select-Object -Last 20

# Utility Commands
version: ## Show version information
	@echo "Order Service Version Information:"
	@echo "=================================="
	@echo ".NET Version: $(shell dotnet --version)"
	@echo "Solution: $(SOLUTION_FILE)"
	@echo "Build Config: $(BUILD_CONFIG)"
	@echo "Docker Image: $(DOCKER_IMAGE_NAME):$(DOCKER_TAG)"

env-check: ## Check environment requirements
	@echo "Checking environment..."
	@echo "✓ .NET SDK: $(shell dotnet --version)"
	@echo "✓ Docker: $(shell docker --version 2>/dev/null || echo 'Not installed')"
	@echo "✓ PostgreSQL: $(shell pg_config --version 2>/dev/null || echo 'Not found in PATH')"
	@echo "Environment check completed!"

# Package Management
package-update: ## Update all NuGet packages
	@echo "Updating packages..."
	dotnet list package --outdated
	@echo "Review outdated packages above, then run specific updates manually"

package-restore-force: ## Force restore packages
	@echo "Force restoring packages..."
	dotnet nuget locals all --clear
	$(MAKE) restore
	@echo "Force restore completed!"

# Performance Commands
benchmark: ## Run performance benchmarks
	@echo "Running benchmarks..."
	cd OrderService.API && dotnet run --configuration Release -- --benchmark
	@echo "Benchmarks completed!"

# Production Commands
deploy-staging: build-release ## Deploy to staging environment
	@echo "Deploying to staging..."
	$(MAKE) docker-build
	@echo "Staging deployment completed!"

deploy-prod: ## Deploy to production (requires confirmation)
	@echo "WARNING: Deploying to PRODUCTION!"
	@echo "Press Ctrl+C to cancel or Enter to continue..."
	@read
	$(MAKE) build-release
	$(MAKE) docker-build
	$(MAKE) docker-push
	@echo "Production deployment completed!"

# Maintenance Commands
backup-db: ## Backup database
	@echo "Creating database backup..."
	pg_dump $(DB_CONNECTION_STRING) > backup_$(shell Get-Date -Format "yyyyMMdd_HHmmss").sql
	@echo "Database backup completed!"

cleanup: ## Cleanup old files and containers
	@echo "Cleaning up..."
	docker system prune -f
	Remove-Item -Recurse -Force ./TestResults -ErrorAction SilentlyContinue || true
	@echo "Cleanup completed!"

# Quick Commands
quick-start: restore build run ## Quick start for development
all: clean restore build test ## Build and test everything

# Saga Testing Commands
test-saga: ## Test Saga workflow
	@echo "Testing Order Saga workflow..."
	cd OrderService.API && dotnet run --configuration Debug -- --test-saga
	@echo "Saga testing completed!"

# Message Queue Commands
rabbitmq-status: ## Check RabbitMQ status
	@echo "Checking RabbitMQ status..."
	curl -f http://$(RABBITMQ_HOST):15672/api/overview || echo "RabbitMQ Management not accessible"

# Configuration Commands
config-dev: ## Show development configuration
	@echo "Development Configuration:"
	@echo "========================="
	@echo "API URL: https://localhost:7001"
	@echo "Database: $(DB_CONNECTION_STRING)"
	@echo "RabbitMQ: $(RABBITMQ_HOST):$(RABBITMQ_PORT)"
	@echo "Environment: Development"

config-prod: ## Show production configuration
	@echo "Production Configuration:"
	@echo "========================"
	@echo "Docker Image: $(DOCKER_IMAGE_NAME):$(DOCKER_TAG)"
	@echo "Registry: $(DOCKER_REGISTRY)"
	@echo "Environment: Production"
