# Variables
SOLUTION_NAME = InventoryService
API_PROJECT = InventoryService.API
DOMAIN_PROJECT = InventoryService.Domain
APPLICATION_PROJECT = InventoryService.Application
INFRASTRUCTURE_PROJECT = InventoryService.Infrastructure

# Directories
SRC_DIR = .
BUILD_DIR = bin
OBJ_DIR = obj

# Configuration
CONFIGURATION = Release
FRAMEWORK = net8.0

# Colors for output
GREEN = \033[0;32m
YELLOW = \033[1;33m
RED = \033[0;31m
NC = \033[0m # No Color

.PHONY: help clean restore build test run docker-build docker-run setup dev watch format coverage

# Default target
help:
	@echo "$(GREEN)🎯 Inventory Service Makefile$(NC)"
	@echo ""
	@echo "$(YELLOW)Available targets:$(NC)"
	@echo "  help          - Show this help message"
	@echo "  clean         - Clean build artifacts"
	@echo "  restore       - Restore NuGet packages"
	@echo "  build         - Build the solution"
	@echo "  test          - Run all tests"
	@echo "  run           - Run the API project"
	@echo "  docker-build  - Build Docker image"
	@echo "  docker-run    - Run Docker container"
	@echo "  setup         - Setup development environment"
	@echo "  dev           - Start development environment"
	@echo "  watch         - Run in watch mode"
	@echo "  format        - Format code"
	@echo "  coverage      - Generate test coverage"

# Clean build artifacts
clean:
	@echo "$(YELLOW)🧹 Cleaning build artifacts...$(NC)"
	dotnet clean $(SOLUTION_NAME).sln
	@echo "$(GREEN)✅ Clean completed$(NC)"

# Restore NuGet packages
restore:
	@echo "$(YELLOW)📦 Restoring NuGet packages...$(NC)"
	dotnet restore $(SOLUTION_NAME).sln
	@echo "$(GREEN)✅ Restore completed$(NC)"

# Build the solution
build: restore
	@echo "$(YELLOW)🏗️ Building solution...$(NC)"
	dotnet build $(SOLUTION_NAME).sln --configuration $(CONFIGURATION) --no-restore
	@echo "$(GREEN)✅ Build completed$(NC)"

# Run tests
test: build
	@echo "$(YELLOW)🧪 Running tests...$(NC)"
	dotnet test $(SOLUTION_NAME).sln --configuration $(CONFIGURATION) --no-build --verbosity normal
	@echo "$(GREEN)✅ Tests completed$(NC)"

# Run the API project
run: build
	@echo "$(YELLOW)🚀 Starting API...$(NC)"
	dotnet run --project $(API_PROJECT) --configuration $(CONFIGURATION)

# Build Docker image
docker-build:
	@echo "$(YELLOW)🐳 Building Docker image...$(NC)"
	docker build -t inventory-service .
	@echo "$(GREEN)✅ Docker image built$(NC)"

# Run Docker container
docker-run:
	@echo "$(YELLOW)🐳 Running Docker container...$(NC)"
	docker run -p 5000:8080 --name inventory-service inventory-service

# Setup development environment
setup: clean restore
	@echo "$(YELLOW)🛠️ Setting up development environment...$(NC)"
	@echo "$(GREEN)✅ Development environment ready$(NC)"

# Development mode
dev: setup
	@echo "$(YELLOW)👨‍💻 Starting development environment...$(NC)"
	dotnet watch --project $(API_PROJECT)

# Watch mode
watch:
	@echo "$(YELLOW)👁️ Starting watch mode...$(NC)"
	dotnet watch --project $(API_PROJECT)

# Format code
format:
	@echo "$(YELLOW)✨ Formatting code...$(NC)"
	dotnet format $(SOLUTION_NAME).sln
	@echo "$(GREEN)✅ Code formatted$(NC)"

# Generate test coverage
coverage: test
	@echo "$(YELLOW)📊 Generating test coverage...$(NC)"
	dotnet test $(SOLUTION_NAME).sln --collect:"XPlat Code Coverage"
	@echo "$(GREEN)✅ Coverage report generated$(NC)"

# Health check
health:
	@echo "$(YELLOW)🏥 Checking service health...$(NC)"
	@curl -s http://localhost:5000/health || echo "$(RED)❌ Service not running$(NC)"

# Package for deployment
package: build test
	@echo "$(YELLOW)📦 Packaging for deployment...$(NC)"
	dotnet publish $(API_PROJECT) --configuration $(CONFIGURATION) --output ./publish
	@echo "$(GREEN)✅ Package ready for deployment$(NC)"

# Deploy (placeholder)
deploy: package
	@echo "$(YELLOW)🚀 Deploying application...$(NC)"
	@echo "$(GREEN)✅ Deployment completed$(NC)"
