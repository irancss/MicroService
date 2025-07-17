# IdentityServer8 Microservice Makefile

# Variables
PROJECT_NAME = identityserver8-microservice
DOCKER_COMPOSE = docker-compose
DOTNET = dotnet

# Default target
.DEFAULT_GOAL := help

# Help target
help: ## Show this help message
	@echo "Available commands:"
	@grep -E '^[a-zA-Z_-]+:.*?## .*$$' $(MAKEFILE_LIST) | sort | awk 'BEGIN {FS = ":.*?## "}; {printf "\033[36m%-20s\033[0m %s\n", $$1, $$2}'

# Development commands
restore: ## Restore NuGet packages
	$(DOTNET) restore

build: ## Build the project
	$(DOTNET) build

run: ## Run the project locally
	$(DOTNET) run

watch: ## Run the project with hot reload
	$(DOTNET) watch run

clean: ## Clean build artifacts
	$(DOTNET) clean
	rm -rf bin obj

# Database commands
migration: ## Create a new migration
	$(DOTNET) ef migrations add $(name) --project .

migrate: ## Apply migrations to database
	$(DOTNET) ef database update

db-drop: ## Drop the database
	$(DOTNET) ef database drop --force

db-reset: db-drop migrate ## Reset database (drop and recreate)

# Docker commands
docker-build: ## Build Docker image
	docker build -t $(PROJECT_NAME) .

docker-up: ## Start services with Docker Compose
	$(DOCKER_COMPOSE) up -d

docker-down: ## Stop services
	$(DOCKER_COMPOSE) down

docker-logs: ## Show Docker logs
	$(DOCKER_COMPOSE) logs -f

docker-restart: docker-down docker-up ## Restart Docker services

docker-clean: ## Clean Docker containers and images
	$(DOCKER_COMPOSE) down -v --rmi all

# Development helpers
format: ## Format code
	$(DOTNET) format

test: ## Run tests
	$(DOTNET) test

publish: ## Publish for production
	$(DOTNET) publish -c Release -o ./publish

# Quick start
dev: restore build migrate run ## Quick development setup

# Production
prod: clean restore build publish ## Production build
