# Notification Microservice Makefile

# Variables
PROJECT_NAME = NotificationService
API_PROJECT = NotificationService.API
DOMAIN_PROJECT = NotificationService.Domain
APPLICATION_PROJECT = NotificationService.Application
INFRASTRUCTURE_PROJECT = NotificationService.Infrastructure

# Docker variables
DOCKER_IMAGE = notification-service
DOCKER_TAG = latest
DOCKER_REGISTRY = your-registry.com

# MongoDB and RabbitMQ for development
MONGODB_CONTAINER = notification-mongodb
RABBITMQ_CONTAINER = notification-rabbitmq

.PHONY: help
help: ## Show this help message
	@echo "Notification Microservice - Available commands:"
	@grep -E '^[a-zA-Z_-]+:.*?## .*$$' $(MAKEFILE_LIST) | sort | awk 'BEGIN {FS = ":.*?## "}; {printf "\033[36m%-20s\033[0m %s\n", $$1, $$2}'

.PHONY: install
install: ## Install .NET dependencies
	@echo "Installing dependencies..."
	dotnet restore
	@echo "Dependencies installed successfully!"

.PHONY: build
build: ## Build the solution
	@echo "Building solution..."
	dotnet build --configuration Release --no-restore
	@echo "Build completed successfully!"

.PHONY: clean
clean: ## Clean build artifacts
	@echo "Cleaning solution..."
	dotnet clean
	rm -rf */bin */obj
	@echo "Clean completed!"

.PHONY: test
test: ## Run all tests
	@echo "Running tests..."
	dotnet test --configuration Release --no-build --verbosity normal --collect:"XPlat Code Coverage"
	@echo "Tests completed!"

.PHONY: test-watch
test-watch: ## Run tests in watch mode
	@echo "Running tests in watch mode..."
	dotnet watch test --configuration Debug

.PHONY: run
run: ## Run the API locally
	@echo "Starting Notification API..."
	cd $(API_PROJECT) && dotnet run

.PHONY: watch
watch: ## Run the API in watch mode
	@echo "Starting Notification API in watch mode..."
	cd $(API_PROJECT) && dotnet watch run

.PHONY: format
format: ## Format code using dotnet format
	@echo "Formatting code..."
	dotnet format
	@echo "Code formatting completed!"

.PHONY: lint
lint: ## Run code analysis
	@echo "Running code analysis..."
	dotnet build --verbosity normal --configuration Release
	@echo "Code analysis completed!"

.PHONY: publish
publish: ## Publish the application
	@echo "Publishing application..."
	dotnet publish $(API_PROJECT) -c Release -o ./publish
	@echo "Application published to ./publish/"

.PHONY: docker-build
docker-build: ## Build Docker image
	@echo "Building Docker image..."
	docker build -t $(DOCKER_IMAGE):$(DOCKER_TAG) .
	@echo "Docker image built: $(DOCKER_IMAGE):$(DOCKER_TAG)"

.PHONY: docker-run
docker-run: ## Run Docker container
	@echo "Running Docker container..."
	docker run -d \
		--name $(PROJECT_NAME) \
		-p 8080:8080 \
		-e ASPNETCORE_ENVIRONMENT=Development \
		$(DOCKER_IMAGE):$(DOCKER_TAG)
	@echo "Container started on port 8080"

.PHONY: docker-stop
docker-stop: ## Stop Docker container
	@echo "Stopping Docker container..."
	docker stop $(PROJECT_NAME) || true
	docker rm $(PROJECT_NAME) || true
	@echo "Container stopped"

.PHONY: docker-push
docker-push: ## Push Docker image to registry
	@echo "Pushing Docker image to registry..."
	docker tag $(DOCKER_IMAGE):$(DOCKER_TAG) $(DOCKER_REGISTRY)/$(DOCKER_IMAGE):$(DOCKER_TAG)
	docker push $(DOCKER_REGISTRY)/$(DOCKER_IMAGE):$(DOCKER_TAG)
	@echo "Image pushed to registry"

.PHONY: dev-deps-start
dev-deps-start: ## Start development dependencies (MongoDB, RabbitMQ)
	@echo "Starting development dependencies..."
	docker run -d \
		--name $(MONGODB_CONTAINER) \
		-p 27017:27017 \
		-v mongodb_data:/data/db \
		mongo:7.0
	docker run -d \
		--name $(RABBITMQ_CONTAINER) \
		-p 5672:5672 \
		-p 15672:15672 \
		-e RABBITMQ_DEFAULT_USER=guest \
		-e RABBITMQ_DEFAULT_PASS=guest \
		rabbitmq:3-management
	@echo "Development dependencies started!"
	@echo "MongoDB: localhost:27017"
	@echo "RabbitMQ Management: http://localhost:15672 (guest/guest)"

.PHONY: dev-deps-stop
dev-deps-stop: ## Stop development dependencies
	@echo "Stopping development dependencies..."
	docker stop $(MONGODB_CONTAINER) $(RABBITMQ_CONTAINER) || true
	docker rm $(MONGODB_CONTAINER) $(RABBITMQ_CONTAINER) || true
	@echo "Development dependencies stopped"

.PHONY: dev-deps-clean
dev-deps-clean: dev-deps-stop ## Clean development dependencies and volumes
	@echo "Cleaning development dependencies..."
	docker volume rm mongodb_data || true
	@echo "Development dependencies cleaned"

.PHONY: seed-templates
seed-templates: ## Seed notification templates
	@echo "Seeding notification templates..."
	curl -X POST http://localhost:5000/api/seed/templates || \
	curl -X POST https://localhost:7000/api/seed/templates
	@echo "Templates seeded successfully!"

.PHONY: dev-setup
dev-setup: dev-deps-start install build ## Complete development setup
	@echo "Development environment setup completed!"
	@echo "Run 'make run' to start the API"

.PHONY: dev-start
dev-start: dev-deps-start run ## Start complete development environment

.PHONY: dev-stop
dev-stop: dev-deps-stop ## Stop complete development environment

.PHONY: logs
logs: ## Show application logs (if running in Docker)
	docker logs -f $(PROJECT_NAME)

.PHONY: logs-deps
logs-deps: ## Show dependency logs
	@echo "MongoDB logs:"
	docker logs $(MONGODB_CONTAINER)
	@echo "\nRabbitMQ logs:"
	docker logs $(RABBITMQ_CONTAINER)

.PHONY: health-check
health-check: ## Check health of services
	@echo "Checking service health..."
	curl -f http://localhost:5000/health || curl -f https://localhost:7000/health || echo "API health check failed"
	@echo "\nChecking MongoDB..."
	docker exec $(MONGODB_CONTAINER) mongosh --eval "db.runCommand('ping')" || echo "MongoDB health check failed"
	@echo "\nChecking RabbitMQ..."
	curl -f http://localhost:15672/api/overview || echo "RabbitMQ health check failed"

.PHONY: migrations
migrations: ## Run database migrations (if any)
	@echo "Running database migrations..."
	# Add your migration commands here
	@echo "Migrations completed!"

.PHONY: backup-db
backup-db: ## Backup MongoDB database
	@echo "Creating database backup..."
	docker exec $(MONGODB_CONTAINER) mongodump --db NotificationService --out /tmp/backup
	docker cp $(MONGODB_CONTAINER):/tmp/backup ./backup-$(shell date +%Y%m%d_%H%M%S)
	@echo "Database backup created"

.PHONY: restore-db
restore-db: ## Restore MongoDB database (specify BACKUP_DIR)
	@echo "Restoring database from $(BACKUP_DIR)..."
	docker cp $(BACKUP_DIR) $(MONGODB_CONTAINER):/tmp/restore
	docker exec $(MONGODB_CONTAINER) mongorestore --db NotificationService /tmp/restore/NotificationService
	@echo "Database restored"

.PHONY: package
package: clean build publish docker-build ## Complete package build

.PHONY: ci-build
ci-build: install build test ## CI build pipeline

.PHONY: cd-deploy
cd-deploy: ci-build docker-build docker-push ## CD deployment pipeline

.PHONY: local-integration-test
local-integration-test: dev-deps-start ## Run integration tests locally
	@echo "Running integration tests..."
	sleep 10  # Wait for dependencies to be ready
	dotnet test --filter Category=Integration
	@echo "Integration tests completed!"

.PHONY: performance-test
performance-test: ## Run performance tests
	@echo "Running performance tests..."
	# Add your performance testing commands here (e.g., k6, NBomber)
	@echo "Performance tests completed!"

.PHONY: security-scan
security-scan: ## Run security scans
	@echo "Running security scans..."
	# Add security scanning tools here
	dotnet list package --vulnerable
	@echo "Security scan completed!"

.PHONY: docs
docs: ## Generate documentation
	@echo "Generating documentation..."
	# Add documentation generation commands here
	@echo "Documentation generated!"

.PHONY: all
all: clean install build test package ## Build everything

# Development workflows
.PHONY: quick-start
quick-start: ## Quick start for development
	@echo "🚀 Quick starting Notification Service..."
	make dev-setup
	make seed-templates
	@echo "✅ Ready for development!"
	@echo "📍 API: https://localhost:7000/swagger"
	@echo "📍 MongoDB: localhost:27017"
	@echo "📍 RabbitMQ: http://localhost:15672"

.PHONY: full-reset
full-reset: dev-deps-clean clean ## Complete reset of development environment
	@echo "♻️  Full reset completed!"

# Production deployment helpers
.PHONY: prod-deploy
prod-deploy: ## Deploy to production (customize as needed)
	@echo "🚀 Deploying to production..."
	# Add your production deployment commands here
	# kubectl apply -f k8s/
	# helm upgrade --install notification-service ./charts/notification-service
	@echo "✅ Production deployment completed!"
