# Discount Service Makefile
# Commands for building, testing, and deploying the Discount microservice

# Variables
PROJECT_NAME = discount-service
API_PROJECT = DiscountService.API
INFRA_PROJECT = DiscountService.Infrastructure
DOCKER_IMAGE = $(PROJECT_NAME):latest
DOCKER_COMPOSE_FILE = docker-compose.yml
MIGRATION_NAME = InitialCreate

# Colors for output
GREEN = \033[0;32m
YELLOW = \033[1;33m
RED = \033[0;31m
NC = \033[0m # No Color

.PHONY: help build clean restore test run migrate docker-build docker-run docker-stop docker-clean setup dev prod

# Default target
help: ## Show this help message
	@echo "$(GREEN)Discount Service - Available Commands:$(NC)"
	@echo ""
	@grep -E '^[a-zA-Z_-]+:.*?## .*$$' $(MAKEFILE_LIST) | sort | awk 'BEGIN {FS = ":.*?## "}; {printf "$(YELLOW)%-20s$(NC) %s\n", $$1, $$2}'

# Development Commands
setup: ## Initial setup - restore packages and build
	@echo "$(GREEN)Setting up Discount Service...$(NC)"
	@$(MAKE) restore
	@$(MAKE) build
	@echo "$(GREEN)Setup completed successfully!$(NC)"

restore: ## Restore NuGet packages
	@echo "$(YELLOW)Restoring NuGet packages...$(NC)"
	dotnet restore

build: ## Build the solution
	@echo "$(YELLOW)Building solution...$(NC)"
	dotnet build --no-restore

clean: ## Clean build artifacts
	@echo "$(YELLOW)Cleaning build artifacts...$(NC)"
	dotnet clean
	@find . -name "bin" -type d -exec rm -rf {} + 2>/dev/null || true
	@find . -name "obj" -type d -exec rm -rf {} + 2>/dev/null || true

rebuild: clean restore build ## Clean, restore, and build

# Testing Commands
test: ## Run all tests
	@echo "$(YELLOW)Running tests...$(NC)"
	dotnet test --no-build --verbosity normal

test-coverage: ## Run tests with coverage
	@echo "$(YELLOW)Running tests with coverage...$(NC)"
	dotnet test --collect:"XPlat Code Coverage" --results-directory:"TestResults"

# Running Commands
run: ## Run the API locally
	@echo "$(YELLOW)Starting Discount Service API...$(NC)"
	cd $(API_PROJECT) && dotnet run

run-watch: ## Run the API with hot reload
	@echo "$(YELLOW)Starting Discount Service API with hot reload...$(NC)"
	cd $(API_PROJECT) && dotnet watch run

dev: docker-dev ## Start development environment with Docker

# Database Commands
migrate: ## Apply database migrations
	@echo "$(YELLOW)Applying database migrations...$(NC)"
	cd $(INFRA_PROJECT) && dotnet ef database update --startup-project ../$(API_PROJECT)

migrate-add: ## Add a new migration (usage: make migrate-add NAME=MigrationName)
	@if [ -z "$(NAME)" ]; then \
		echo "$(RED)Error: Migration name required. Usage: make migrate-add NAME=MigrationName$(NC)"; \
		exit 1; \
	fi
	@echo "$(YELLOW)Creating migration: $(NAME)...$(NC)"
	cd $(INFRA_PROJECT) && dotnet ef migrations add $(NAME) --startup-project ../$(API_PROJECT) --output-dir Data/Migrations

migrate-remove: ## Remove the last migration
	@echo "$(YELLOW)Removing last migration...$(NC)"
	cd $(INFRA_PROJECT) && dotnet ef migrations remove --startup-project ../$(API_PROJECT)

migrate-script: ## Generate SQL migration script
	@echo "$(YELLOW)Generating migration script...$(NC)"
	cd $(INFRA_PROJECT) && dotnet ef migrations script --startup-project ../$(API_PROJECT) --output migrations.sql

db-drop: ## Drop the database
	@echo "$(RED)Dropping database...$(NC)"
	cd $(INFRA_PROJECT) && dotnet ef database drop --startup-project ../$(API_PROJECT) --force

db-reset: db-drop migrate ## Drop and recreate database

# Docker Commands
docker-build: ## Build Docker image
	@echo "$(YELLOW)Building Docker image...$(NC)"
	docker build -t $(DOCKER_IMAGE) .

docker-run: ## Run application with Docker Compose
	@echo "$(YELLOW)Starting services with Docker Compose...$(NC)"
	docker-compose up -d

docker-dev: ## Start development environment
	@echo "$(YELLOW)Starting development environment...$(NC)"
	docker-compose up -d postgres redis rabbitmq
	@echo "$(GREEN)Development services started. Run 'make run' to start the API locally.$(NC)"

docker-logs: ## Show Docker logs
	docker-compose logs -f

docker-stop: ## Stop Docker containers
	@echo "$(YELLOW)Stopping Docker containers...$(NC)"
	docker-compose down

docker-clean: ## Clean Docker resources
	@echo "$(YELLOW)Cleaning Docker resources...$(NC)"
	docker-compose down -v --remove-orphans
	docker image prune -f
	docker volume prune -f

docker-restart: docker-stop docker-run ## Restart Docker containers

# Production Commands
prod-build: ## Build for production
	@echo "$(YELLOW)Building for production...$(NC)"
	dotnet publish $(API_PROJECT) -c Release -o publish

prod-docker: ## Build production Docker image
	@echo "$(YELLOW)Building production Docker image...$(NC)"
	docker build -t $(PROJECT_NAME):prod -f Dockerfile.prod . || docker build -t $(PROJECT_NAME):prod .

# Monitoring and Health
health: ## Check service health
	@echo "$(YELLOW)Checking service health...$(NC)"
	@curl -f http://localhost:8080/health || echo "$(RED)Service is not running$(NC)"

swagger: ## Open Swagger documentation
	@echo "$(YELLOW)Opening Swagger documentation...$(NC)"
	@which open >/dev/null 2>&1 && open http://localhost:8080/swagger || \
	 which xdg-open >/dev/null 2>&1 && xdg-open http://localhost:8080/swagger || \
	 echo "Please open http://localhost:8080/swagger in your browser"

# Utility Commands
format: ## Format code
	@echo "$(YELLOW)Formatting code...$(NC)"
	dotnet format

lint: ## Run code analysis
	@echo "$(YELLOW)Running code analysis...$(NC)"
	dotnet format --verify-no-changes --verbosity diagnostic

security-scan: ## Run security scan
	@echo "$(YELLOW)Running security scan...$(NC)"
	dotnet list package --vulnerable --include-transitive

update-packages: ## Update NuGet packages
	@echo "$(YELLOW)Updating NuGet packages...$(NC)"
	dotnet list package --outdated

# Environment Commands
env-copy: ## Copy environment template
	@if [ ! -f .env ]; then \
		cp .env.template .env; \
		echo "$(GREEN).env file created from template$(NC)"; \
	else \
		echo "$(YELLOW).env file already exists$(NC)"; \
	fi

# Deployment Commands
deploy-staging: ## Deploy to staging environment
	@echo "$(YELLOW)Deploying to staging...$(NC)"
	# Add your staging deployment commands here

deploy-prod: ## Deploy to production environment
	@echo "$(YELLOW)Deploying to production...$(NC)"
	# Add your production deployment commands here

# Backup Commands
backup-db: ## Backup database
	@echo "$(YELLOW)Creating database backup...$(NC)"
	# Add database backup commands here

# All-in-one Commands
full-setup: clean setup docker-dev migrate ## Complete setup from scratch
	@echo "$(GREEN)Full setup completed! Service is ready to use.$(NC)"

quick-start: docker-dev ## Quick start for development
	@echo "$(GREEN)Development environment started!$(NC)"
	@echo "$(YELLOW)Next steps:$(NC)"
	@echo "  1. Run 'make migrate' to set up the database"
	@echo "  2. Run 'make run' to start the API"
	@echo "  3. Visit http://localhost:8080/swagger for API documentation"

# Status and Information
status: ## Show service status
	@echo "$(GREEN)=== Discount Service Status ===$(NC)"
	@echo "$(YELLOW)Docker Containers:$(NC)"
	@docker-compose ps || echo "Docker Compose not running"
	@echo "\n$(YELLOW)API Health:$(NC)"
	@curl -s http://localhost:8080/health | head -1 || echo "API not responding"
	@echo "\n$(YELLOW)Database Connection:$(NC)"
	@docker-compose exec postgres pg_isready -U postgres 2>/dev/null || echo "Database not accessible"

info: ## Show project information
	@echo "$(GREEN)=== Discount Service Information ===$(NC)"
	@echo "$(YELLOW)Project:$(NC) $(PROJECT_NAME)"
	@echo "$(YELLOW)API Project:$(NC) $(API_PROJECT)"
	@echo "$(YELLOW)Infrastructure Project:$(NC) $(INFRA_PROJECT)"
	@echo "$(YELLOW)Docker Image:$(NC) $(DOCKER_IMAGE)"
	@echo "$(YELLOW)API URL:$(NC) http://localhost:8080"
	@echo "$(YELLOW)Swagger URL:$(NC) http://localhost:8080/swagger"
	@echo "$(YELLOW)Health Check:$(NC) http://localhost:8080/health"

# Troubleshooting
troubleshoot: ## Common troubleshooting steps
	@echo "$(GREEN)=== Troubleshooting Guide ===$(NC)"
	@echo "$(YELLOW)1. Check Docker services:$(NC)"
	@docker-compose ps
	@echo "\n$(YELLOW)2. Check application logs:$(NC)"
	@echo "   make docker-logs"
	@echo "\n$(YELLOW)3. Check health endpoint:$(NC)"
	@echo "   make health"
	@echo "\n$(YELLOW)4. Reset everything:$(NC)"
	@echo "   make docker-clean && make full-setup"

# Development workflow
workflow: ## Show development workflow
	@echo "$(GREEN)=== Development Workflow ===$(NC)"
	@echo "$(YELLOW)1. Initial Setup:$(NC)"
	@echo "   make setup"
	@echo "   make docker-dev"
	@echo "   make migrate"
	@echo ""
	@echo "$(YELLOW)2. Daily Development:$(NC)"
	@echo "   make run-watch    # Start API with hot reload"
	@echo "   make test         # Run tests"
	@echo "   make format       # Format code"
	@echo ""
	@echo "$(YELLOW)3. Database Changes:$(NC)"
	@echo "   make migrate-add NAME=YourMigrationName"
	@echo "   make migrate"
	@echo ""
	@echo "$(YELLOW)4. Docker Development:$(NC)"
	@echo "   make docker-run   # Full environment"
	@echo "   make docker-logs  # View logs"
	@echo "   make docker-stop  # Stop services"
