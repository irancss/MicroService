# Marketing Microservice Makefile
# .NET 8 Marketing Service with Clean Architecture & CQRS
# فایل Makefile برای میکروسرویس بازاریابی با معماری پاک و CQRS

# Variables / متغیرها
PROJECT_NAME = MarketingService
SOLUTION_FILE = Marketing.sln
API_PROJECT = MarketingService.API
DOMAIN_PROJECT = MarketingService.Domain
APPLICATION_PROJECT = MarketingService.Application
INFRASTRUCTURE_PROJECT = MarketingService.Infrastructure

# Database Configuration / پیکربندی دیتابیس
DB_CONNECTION_STRING = "Host=localhost;Database=MarketingDB;Username=postgres;Password=password"
MIGRATION_NAME = InitialCreate

# Docker Configuration / پیکربندی داکر
DOCKER_IMAGE_NAME = marketing-service
DOCKER_TAG = latest

# Colors for output / رنگ‌ها برای خروجی
RED = \033[0;31m
GREEN = \033[0;32m
YELLOW = \033[1;33m
BLUE = \033[0;34m
NC = \033[0m

# Database settings / تنظیمات دیتابیس
DB_CONNECTION = "Host=localhost;Database=MarketingDB;Username=postgres;Password=password"
MIGRATION_NAME = InitialCreate

# Docker settings / تنظیمات داکر
DOCKER_IMAGE = marketing-service
DOCKER_TAG = latest

# Default target / هدف پیش‌فرض
.DEFAULT_GOAL := help

# Colors for output / رنگ‌ها برای خروجی
BLUE = \033[0;34m
GREEN = \033[0;32m
YELLOW = \033[0;33m
RED = \033[0;31m
NC = \033[0m # No Color

##@ Development Commands / دستورات توسعه

.PHONY: help
help: ## Show this help message / نمایش این پیام راهنما
	@echo "$(BLUE)Marketing Microservice - Available Commands$(NC)"
	@echo "$(BLUE)میکروسرویس بازاریابی - دستورات موجود$(NC)"
	@echo ""
	@awk 'BEGIN {FS = ":.*##"; printf "\nUsage:\n  make \033[36m<target>\033[0m\n"} /^[a-zA-Z_0-9-]+:.*?##/ { printf "  \033[36m%-15s\033[0m %s\n", $$1, $$2 } /^##@/ { printf "\n\033[1m%s\033[0m\n", substr($$0, 5) } ' $(MAKEFILE_LIST)

.PHONY: setup
setup: ## Setup development environment / راه‌اندازی محیط توسعه
	@echo "$(YELLOW)Setting up development environment...$(NC)"
	@echo "$(YELLOW)راه‌اندازی محیط توسعه...$(NC)"
	dotnet --version
	dotnet restore $(SOLUTION_FILE)
	@echo "$(GREEN)Development environment setup complete!$(NC)"
	@echo "$(GREEN)راه‌اندازی محیط توسعه کامل شد!$(NC)"

.PHONY: clean
clean: ## Clean build artifacts / پاک‌سازی فایل‌های ساخت
	@echo "$(YELLOW)Cleaning build artifacts...$(NC)"
	@echo "$(YELLOW)پاک‌سازی فایل‌های ساخت...$(NC)"
	dotnet clean $(SOLUTION_FILE)
	@echo "$(GREEN)Clean complete!$(NC)"

.PHONY: restore
restore: ## Restore NuGet packages / بازیابی پکیج‌های NuGet
	@echo "$(YELLOW)Restoring NuGet packages...$(NC)"
	@echo "$(YELLOW)بازیابی پکیج‌های NuGet...$(NC)"
	dotnet restore $(SOLUTION_FILE)

.PHONY: build
build: restore ## Build the solution / ساخت راه‌حل
	@echo "$(YELLOW)Building solution...$(NC)"
	@echo "$(YELLOW)ساخت راه‌حل...$(NC)"
	dotnet build $(SOLUTION_FILE) --no-restore
	@echo "$(GREEN)Build complete!$(NC)"

.PHONY: test
test: build ## Run tests / اجرای تست‌ها
	@echo "$(YELLOW)Running tests...$(NC)"
	@echo "$(YELLOW)اجرای تست‌ها...$(NC)"
	dotnet test $(SOLUTION_FILE) --no-build --verbosity normal

.PHONY: run
run: build ## Run the API locally / اجرای API به صورت محلی
	@echo "$(YELLOW)Starting Marketing API...$(NC)"
	@echo "$(YELLOW)شروع API بازاریابی...$(NC)"
	cd MarketingService.API && dotnet run

.PHONY: watch
watch: ## Run with hot reload / اجرا با بارگذاری مجدد خودکار
	@echo "$(YELLOW)Starting with hot reload...$(NC)"
	@echo "$(YELLOW)شروع با بارگذاری مجدد خودکار...$(NC)"
	cd MarketingService.API && dotnet watch run

##@ Database Commands / دستورات دیتابیس

.PHONY: migration-add
migration-add: ## Add new migration (MIGRATION_NAME=name) / افزودن migration جدید
	@echo "$(YELLOW)Adding migration: $(MIGRATION_NAME)$(NC)"
	@echo "$(YELLOW)افزودن migration: $(MIGRATION_NAME)$(NC)"
	cd MarketingService.API && dotnet ef migrations add $(MIGRATION_NAME) --project ../$(INFRASTRUCTURE_PROJECT)

.PHONY: migration-remove
migration-remove: ## Remove last migration / حذف آخرین migration
	@echo "$(YELLOW)Removing last migration...$(NC)"
	@echo "$(YELLOW)حذف آخرین migration...$(NC)"
	cd MarketingService.API && dotnet ef migrations remove --project ../$(INFRASTRUCTURE_PROJECT)

.PHONY: db-update
db-update: ## Update database / بروزرسانی دیتابیس
	@echo "$(YELLOW)Updating database...$(NC)"
	@echo "$(YELLOW)بروزرسانی دیتابیس...$(NC)"
	cd MarketingService.API && dotnet ef database update --project ../$(INFRASTRUCTURE_PROJECT)

.PHONY: db-drop
db-drop: ## Drop database / حذف دیتابیس
	@echo "$(RED)Dropping database...$(NC)"
	@echo "$(RED)حذف دیتابیس...$(NC)"
	cd MarketingService.API && dotnet ef database drop --project ../$(INFRASTRUCTURE_PROJECT) --force

.PHONY: db-reset
db-reset: db-drop db-update ## Reset database / ریست دیتابیس
	@echo "$(GREEN)Database reset complete!$(NC)"
	@echo "$(GREEN)ریست دیتابیس کامل شد!$(NC)"

##@ Docker Commands / دستورات داکر

.PHONY: docker-build
docker-build: ## Build Docker image / ساخت image داکر
	@echo "$(YELLOW)Building Docker image...$(NC)"
	@echo "$(YELLOW)ساخت image داکر...$(NC)"
	docker build -t $(DOCKER_IMAGE):$(DOCKER_TAG) -f Dockerfile .

.PHONY: docker-run
docker-run: ## Run in Docker container / اجرا در کانتینر داکر
	@echo "$(YELLOW)Running Docker container...$(NC)"
	@echo "$(YELLOW)اجرای کانتینر داکر...$(NC)"
	docker run -p 8080:8080 -p 8081:8081 $(DOCKER_IMAGE):$(DOCKER_TAG)

.PHONY: docker-compose-up
docker-compose-up: ## Start with docker-compose / شروع با docker-compose
	@echo "$(YELLOW)Starting services with docker-compose...$(NC)"
	@echo "$(YELLOW)شروع سرویس‌ها با docker-compose...$(NC)"
	docker-compose up -d

.PHONY: docker-compose-down
docker-compose-down: ## Stop docker-compose services / توقف سرویس‌های docker-compose
	@echo "$(YELLOW)Stopping services...$(NC)"
	@echo "$(YELLOW)توقف سرویس‌ها...$(NC)"
	docker-compose down

##@ Code Quality / کیفیت کد

.PHONY: format
format: ## Format code / فرمت کردن کد
	@echo "$(YELLOW)Formatting code...$(NC)"
	@echo "$(YELLOW)فرمت کردن کد...$(NC)"
	dotnet format $(SOLUTION_FILE)

.PHONY: lint
lint: ## Run linting / اجرای بررسی کد
	@echo "$(YELLOW)Running code analysis...$(NC)"
	@echo "$(YELLOW)اجرای تحلیل کد...$(NC)"
	dotnet build $(SOLUTION_FILE) --verbosity normal

.PHONY: security-scan
security-scan: ## Run security scan / اجرای اسکن امنیتی
	@echo "$(YELLOW)Running security scan...$(NC)"
	@echo "$(YELLOW)اجرای اسکن امنیتی...$(NC)"
	dotnet list package --vulnerable --include-transitive

##@ Package Management / مدیریت پکیج‌ها

.PHONY: packages-update
packages-update: ## Update all packages / بروزرسانی همه پکیج‌ها
	@echo "$(YELLOW)Updating packages...$(NC)"
	@echo "$(YELLOW)بروزرسانی پکیج‌ها...$(NC)"
	dotnet list package --outdated
	@echo "Run 'dotnet add package <PackageName>' to update individual packages"

.PHONY: packages-list
packages-list: ## List all packages / لیست همه پکیج‌ها
	@echo "$(YELLOW)Listing packages...$(NC)"
	@echo "$(YELLOW)لیست پکیج‌ها...$(NC)"
	dotnet list package

##@ Production Commands / دستورات تولید

.PHONY: publish
publish: build ## Publish for production / انتشار برای تولید
	@echo "$(YELLOW)Publishing for production...$(NC)"
	@echo "$(YELLOW)انتشار برای تولید...$(NC)"
	dotnet publish $(API_PROJECT) -c Release -o ./publish

.PHONY: publish-docker
publish-docker: docker-build ## Build and tag for production / ساخت و برچسب‌گذاری برای تولید
	@echo "$(YELLOW)Tagging for production...$(NC)"
	@echo "$(YELLOW)برچسب‌گذاری برای تولید...$(NC)"
	docker tag $(DOCKER_IMAGE):$(DOCKER_TAG) $(DOCKER_IMAGE):production

##@ Monitoring & Health / نظارت و سلامت

.PHONY: health-check
health-check: ## Check service health / بررسی سلامت سرویس
	@echo "$(YELLOW)Checking service health...$(NC)"
	@echo "$(YELLOW)بررسی سلامت سرویس...$(NC)"
	curl -f http://localhost:5000/health || echo "Service not running"

.PHONY: logs
logs: ## View application logs / مشاهده لاگ‌های برنامه
	@echo "$(YELLOW)Viewing logs...$(NC)"
	@echo "$(YELLOW)مشاهده لاگ‌ها...$(NC)"
	docker-compose logs -f $(PROJECT_NAME) || echo "Use 'dotnet run' logs if not using Docker"

##@ Development Tools / ابزارهای توسعه

.PHONY: dev-tools
dev-tools: ## Install development tools / نصب ابزارهای توسعه
	@echo "$(YELLOW)Installing development tools...$(NC)"
	@echo "$(YELLOW)نصب ابزارهای توسعه...$(NC)"
	dotnet tool install --global dotnet-ef
	dotnet tool install --global dotnet-format
	dotnet tool install --global dotnet-outdated-tool

.PHONY: swagger
swagger: ## Open Swagger UI / باز کردن Swagger UI
	@echo "$(YELLOW)Opening Swagger UI...$(NC)"
	@echo "$(YELLOW)باز کردن Swagger UI...$(NC)"
	@echo "Navigate to: http://localhost:5000/swagger"
	@echo "آدرس: http://localhost:5000/swagger"

.PHONY: hangfire
hangfire: ## Open Hangfire Dashboard / باز کردن داشبورد Hangfire
	@echo "$(YELLOW)Opening Hangfire Dashboard...$(NC)"
	@echo "$(YELLOW)باز کردن داشبورد Hangfire...$(NC)"
	@echo "Navigate to: http://localhost:5000/hangfire"
	@echo "آدرس: http://localhost:5000/hangfire"

##@ Quick Start / شروع سریع

.PHONY: quick-start
quick-start: setup build db-update run ## Complete setup and run / راه‌اندازی کامل و اجرا
	@echo "$(GREEN)Marketing Service is now running!$(NC)"
	@echo "$(GREEN)سرویس بازاریابی در حال اجرا است!$(NC)"

##@ Cleanup / پاک‌سازی

.PHONY: clean-all
clean-all: clean ## Clean everything including Docker / پاک‌سازی همه چیز شامل داکر
	@echo "$(YELLOW)Cleaning all artifacts...$(NC)"
	@echo "$(YELLOW)پاک‌سازی همه آثار...$(NC)"
	docker system prune -f
	docker volume prune -f
