# Payment Microservice Makefile
.PHONY: build clean test run migrate docker-build docker-run dev prod help

# Variables
PROJECT_NAME = Payment
API_PROJECT = Payment.API
INFRA_PROJECT = Payment.Infrastructure
DOCKER_IMAGE = payment-service
DOCKER_TAG = latest
DB_PASSWORD = 123

# Default target
help:
	@echo "Available commands:"
	@echo "  build        - Build the solution"
	@echo "  clean        - Clean the solution"
	@echo "  test         - Run all tests"
	@echo "  run          - Run the application in development mode"
	@echo "  migrate      - Apply database migrations"
	@echo "  dev          - Run development environment with dependencies"
	@echo "  prod         - Run production environment"
	@echo "  docker-build - Build Docker image"
	@echo "  docker-run   - Run Docker container"
	@echo "  setup        - Initial setup (build + migrate)"
	@echo "  clean-all    - Clean everything including Docker images"

# Build the solution
build:
	@echo "Building $(PROJECT_NAME) solution..."
	dotnet restore
	dotnet build --configuration Release --no-restore

# Clean build artifacts
clean:
	@echo "Cleaning $(PROJECT_NAME) solution..."
	dotnet clean
	rm -rf */bin */obj

# Run tests
test:
	@echo "Running tests for $(PROJECT_NAME)..."
	dotnet test --configuration Release --no-build --verbosity normal

# Run the application
run:
	@echo "Running $(PROJECT_NAME) application..."
	dotnet run --project $(API_PROJECT) --configuration Development

# Apply database migrations
migrate:
	@echo "Applying database migrations..."
	cd $(INFRA_PROJECT) && dotnet ef database update --startup-project ../$(API_PROJECT)

# Create new migration
migration:
	@echo "Creating new migration: $(name)"
	cd $(INFRA_PROJECT) && dotnet ef migrations add $(name) --startup-project ../$(API_PROJECT)

# Remove last migration
migration-remove:
	@echo "Removing last migration..."
	cd $(INFRA_PROJECT) && dotnet ef migrations remove --startup-project ../$(API_PROJECT)

# Development environment setup
dev: build
	@echo "Starting development environment..."
	@echo "Starting dependencies with Docker Compose..."
	docker-compose -f docker-compose.dev.yml up -d
	@echo "Waiting for services to be ready..."
	timeout 30
	$(MAKE) migrate
	$(MAKE) run

# Production environment
prod: build migrate
	@echo "Starting production environment..."
	dotnet run --project $(API_PROJECT) --configuration Release

# Docker build
docker-build:
	@echo "Building Docker image..."
	docker build -t $(DOCKER_IMAGE):$(DOCKER_TAG) .

# Docker run
docker-run:
	@echo "Running Docker container..."
	docker run -p 5000:8080 -e ASPNETCORE_ENVIRONMENT=Production $(DOCKER_IMAGE):$(DOCKER_TAG)

# Initial setup
setup: build migrate
	@echo "Initial setup completed!"

# Clean everything including Docker
clean-all: clean
	@echo "Cleaning Docker images..."
	docker rmi $(DOCKER_IMAGE):$(DOCKER_TAG) 2>/dev/null || true
	docker system prune -f

# Install EF Core tools if not present
install-ef:
	@echo "Installing Entity Framework Core tools..."
	dotnet tool install --global dotnet-ef

# Package the application
package: build test
	@echo "Packaging application..."
	dotnet publish $(API_PROJECT) -c Release -o ./publish

# Watch for file changes and auto-rebuild
watch:
	@echo "Watching for changes..."
	dotnet watch --project $(API_PROJECT) run

# Database seed (if you have seed data)
seed:
	@echo "Seeding database..."
	dotnet run --project $(API_PROJECT) -- --seed

# Health check
health:
	@echo "Checking application health..."
	curl -f http://localhost:5000/health || echo "Application is not running"

# Generate API documentation
docs:
	@echo "Generating API documentation..."
	dotnet run --project $(API_PROJECT) --urls=http://localhost:5000 &
	timeout 10
	curl http://localhost:5000/swagger/v1/swagger.json > api-docs.json
	pkill -f "dotnet.*$(API_PROJECT)"

# Performance test
perf:
	@echo "Running performance tests..."
	# Add your performance testing tools here
	# Example: artillery, k6, etc.
