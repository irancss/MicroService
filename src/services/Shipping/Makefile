# Shipping Service Makefile - Complete Build and Deployment Automation
# ===================================================================
# .NET 8 Microservice with CQRS, Premium Subscriptions, and Free Shipping Rules
# مجموعه دستورات کامل برای مدیریت پروژه شیپینگ با قابلیت‌های پیشرفته

# Variables - متغیرهای پیکربندی
PROJECT_NAME = ShippingService
SOLUTION_FILE = Shipping.sln
API_PROJECT = ShippingService.API
INFRASTRUCTURE_PROJECT = ShippingService.Infrastructure
DOMAIN_PROJECT = ShippingService.Domain
APPLICATION_PROJECT = ShippingService.Application
TEST_PROJECT = ShippingService.Tests

# Database Configuration - پیکربندی دیتابیس
DB_CONNECTION = "Host=localhost;Database=ShippingServiceDb;Username=postgres;Password=your_password"
MIGRATION_NAME = AddPremiumSubscriptionAndFreeShippingRules

# Docker Configuration - پیکربندی داکر
DOCKER_IMAGE = shipping-service
DOCKER_TAG = latest
DOCKER_REGISTRY = your-registry.com

# Code Quality - کیفیت کد
COVERAGE_THRESHOLD = 80

.PHONY: help clean restore build test package deploy migrate docker-build docker-run docker-push lint format check-format security-scan docs serve-docs
.DEFAULT_GOAL := help

## Help Commands - دستورات راهنما
help: ## Show this help message - نمایش راهنمای دستورات
	@echo "📦 Shipping Service - Complete Build Automation"
	@echo "🚀 سیستم خودکارسازی کامل ساخت و استقرار میکروسرویس شیپینگ"
	@echo ""
	@echo "🚀 Main Commands - دستورات اصلی:"
	@echo "  make clean          - Clean all build artifacts - پاک کردن فایل‌های ساخت"
	@echo "  make restore        - Restore NuGet packages - بازیابی پکیج‌ها"
	@echo "  make build          - Build the solution - ساخت پروژه"
	@echo "  make test           - Run all tests with coverage - اجرای تست‌ها با پوشش"
	@echo "  make package        - Create deployment package - ایجاد پکیج استقرار"
	@echo "  make deploy         - Deploy to target environment - استقرار"
	@echo ""
	@echo "🗄️ Database Commands - دستورات دیتابیس:"
	@echo "  make migrate        - Run database migrations - اجرای migration"
	@echo "  make migration      - Create new migration - ایجاد migration جدید"
	@echo "  make db-update      - Update database to latest migration - به‌روزرسانی دیتابیس"
	@echo "  make db-seed        - Seed database with test data - اجرای seed data"
	@echo "  make premium-migrate - Apply premium features migration - اعمال migration ویژگی‌های ویژه"
	@echo ""
	@echo "🐳 Docker Commands - دستورات داکر:"
	@echo "  make docker-build   - Build Docker image - ساخت تصویر داکر"
	@echo "  make docker-run     - Run Docker container locally - اجرای کانتینر محلی"
	@echo "  make docker-push    - Push Docker image to registry - ارسال به رجیستری"
	@echo "  make docker-compose - Run with docker-compose - اجرای با docker-compose"
	@echo ""
	@echo "🔍 Code Quality Commands - دستورات کیفیت کد:"
	@echo "  make lint           - Run static code analysis - تحلیل استاتیک کد"
	@echo "  make format         - Format code with dotnet format - فرمت کردن کد"
	@echo "  make check-format   - Check code formatting - بررسی فرمت کد"
	@echo "  make security-scan  - Run security vulnerability scan - بررسی امنیتی"
	@echo "  make coverage       - Generate test coverage report - گزارش پوشش تست"
	@echo ""
	@echo "📚 Documentation Commands - دستورات مستندسازی:"
	@echo "  make docs           - Generate API documentation - تولید مستندات API"
	@echo "  make serve-docs     - Serve documentation locally - سرو مستندات محلی"
	@echo ""
	@echo "🔧 Development Commands - دستورات توسعه:"
	@echo "  make dev-setup      - Setup development environment - راه‌اندازی محیط توسعه"
	@echo "  make dev-run        - Run development server with hot reload - اجرای سرور توسعه"
	@echo "  make dev-watch      - Watch for changes and auto-rebuild - نظارت و بازسازی خودکار"
	@echo ""
	@echo "💎 Premium Features - ویژگی‌های ویژه:"
## Build Commands - دستورات ساخت
clean: ## Clean all build artifacts and temporary files - پاک کردن فایل‌های ساخت
	@echo "🧹 Cleaning build artifacts... - پاک کردن فایل‌های ساخت..."
	dotnet clean $(SOLUTION_FILE)
	Remove-Item -Recurse -Force -ErrorAction SilentlyContinue bin, obj, TestResults, coverage
	@echo "✅ Clean completed - پاک‌سازی کامل شد"

restore: ## Restore NuGet packages for all projects - بازیابی پکیج‌ها
	@echo "📦 Restoring NuGet packages... - بازیابی پکیج‌ها..."
	dotnet restore $(SOLUTION_FILE)
	@echo "✅ Package restore completed - بازیابی پکیج‌ها کامل شد"

build: restore ## Build the entire solution - ساخت کامل پروژه
	@echo "🔨 Building solution... - ساخت پروژه..."
	dotnet build $(SOLUTION_FILE) --configuration Release --no-restore
	@echo "✅ Build completed successfully - ساخت با موفقیت کامل شد"

build-debug: restore ## Build solution in debug mode - ساخت در حالت debug
	@echo "🔨 Building solution (Debug)... - ساخت در حالت دیباگ..."
	dotnet build $(SOLUTION_FILE) --configuration Debug --no-restore
	@echo "✅ Debug build completed - ساخت دیباگ کامل شد"

## Test Commands - دستورات تست
test: ## Run all tests with coverage reporting - اجرای تست‌ها با گزارش پوشش
	@echo "🧪 Running tests with coverage... - اجرای تست‌ها با پوشش..."
	dotnet test $(SOLUTION_FILE) \
		--configuration Release \
		--no-build \
		--verbosity normal \
		--collect:"XPlat Code Coverage" \
		--results-directory:./TestResults \
		--logger:"trx;LogFileName=test-results.trx" \
		--logger:"html;LogFileName=test-results.html"
	@echo "✅ Tests completed - تست‌ها کامل شد"

test-watch: ## Run tests in watch mode for development - تست در حالت watch
	@echo "👀 Running tests in watch mode... - اجرای تست در حالت watch..."
	dotnet watch test $(TEST_PROJECT) --configuration Debug

coverage: test ## Generate detailed test coverage report - تولید گزارش پوشش تست
	@echo "📊 Generating coverage report... - تولید گزارش پوشش..."
	dotnet tool install -g dotnet-reportgenerator-globaltool 2>$$null || echo "ReportGenerator already installed"
	reportgenerator \
		-reports:"TestResults/**/coverage.cobertura.xml" \
		-targetdir:"TestResults/CoverageReport" \
		-reporttypes:"Html;Badges;TextSummary"
	@echo "✅ Coverage report generated at TestResults/CoverageReport/ - گزارش پوشش در TestResults/CoverageReport/ تولید شد"

## Package and Deployment Commands - دستورات بسته‌بندی و استقرار
package: build ## Create deployment package - ایجاد پکیج استقرار
	@echo "📦 Creating deployment package... - ایجاد پکیج استقرار..."
	dotnet publish $(API_PROJECT) \
		--configuration Release \
		--no-build \
		--output ./publish \
		--runtime win-x64 \
		--self-contained false
	@echo "✅ Package created at ./publish/ - پکیج در ./publish/ ایجاد شد"

deploy: package ## Deploy to target environment - استقرار در محیط هدف
	@echo "🚀 Deploying application... - استقرار اپلیکیشن..."
	# Add your deployment commands here (Azure, AWS, etc.)
	@echo "✅ Deployment completed - استقرار کامل شد"

## Database Commands - دستورات دیتابیس
migrate: ## Run all pending database migrations - اجرای migration های معلق
	@echo "🗄️ Running database migrations... - اجرای migration های دیتابیس..."
	dotnet ef database update \
		--project $(INFRASTRUCTURE_PROJECT) \
		--startup-project $(API_PROJECT) \
		--connection $(DB_CONNECTION)
	@echo "✅ Migrations completed - migration ها کامل شد"

migration: ## Create a new database migration (requires MIGRATION_NAME) - ایجاد migration جدید
	@echo "📝 Creating new migration: $(MIGRATION_NAME)... - ایجاد migration جدید..."
	dotnet ef migrations add $(MIGRATION_NAME) \
		--project $(INFRASTRUCTURE_PROJECT) \
		--startup-project $(API_PROJECT)
	@echo "✅ Migration $(MIGRATION_NAME) created - migration $(MIGRATION_NAME) ایجاد شد"

db-update: ## Update database to latest migration - به‌روزرسانی دیتابیس
	@echo "⬆️ Updating database... - به‌روزرسانی دیتابیس..."
	dotnet ef database update \
		--project $(INFRASTRUCTURE_PROJECT) \
		--startup-project $(API_PROJECT)
	@echo "✅ Database updated - دیتابیس به‌روزرسانی شد"

db-seed: ## Seed database with test data - اجرای seed data
	@echo "🌱 Seeding database... - اجرای seed data..."
	dotnet run --project $(API_PROJECT) -- --seed-data
	@echo "✅ Database seeded - seed data اجرا شد"

db-reset: ## Reset database (drops and recreates) - بازنشانی دیتابیس
	@echo "🔄 Resetting database... - بازنشانی دیتابیس..."
	dotnet ef database drop --force \
		--project $(INFRASTRUCTURE_PROJECT) \
		--startup-project $(API_PROJECT)
	$(MAKE) migrate
	$(MAKE) db-seed
	@echo "✅ Database reset completed - بازنشانی دیتابیس کامل شد"

## Premium Features Commands - دستورات ویژگی‌های ویژه
premium-migrate: ## Apply premium features migration - اعمال migration ویژگی‌های ویژه
	@echo "💎 Applying premium features migration... - اعمال migration ویژگی‌های ویژه..."
	# Apply the SQL migration file we created
	psql $(DB_CONNECTION) -f MIGRATION_PREMIUM_FEATURES.sql
	@echo "✅ Premium features migration applied - migration ویژگی‌های ویژه اعمال شد"

premium-test: ## Test premium subscription features - تست ویژگی‌های اشتراک ویژه
	@echo "💎 Testing premium subscription features... - تست ویژگی‌های اشتراک ویژه..."
	dotnet test $(TEST_PROJECT) --filter "Category=Premium" --verbosity normal
	@echo "✅ Premium features tests completed - تست ویژگی‌های ویژه کامل شد"

## Docker Commands - دستورات داکر
docker-build: ## Build Docker image for the application - ساخت تصویر داکر
	@echo "🐳 Building Docker image... - ساخت تصویر داکر..."
	docker build -t $(DOCKER_IMAGE):$(DOCKER_TAG) -f Dockerfile .
	@echo "✅ Docker image built: $(DOCKER_IMAGE):$(DOCKER_TAG) - تصویر داکر ساخته شد"

docker-run: ## Run Docker container locally - اجرای کانتینر محلی
	@echo "🐳 Running Docker container... - اجرای کانتینر داکر..."
	docker run -d \
		--name $(PROJECT_NAME)-container \
		-p 8080:80 \
		-e ASPNETCORE_ENVIRONMENT=Development \
		$(DOCKER_IMAGE):$(DOCKER_TAG)
	@echo "✅ Container running at http://localhost:8080 - کانتینر در http://localhost:8080 اجرا شد"

docker-stop: ## Stop running Docker container - توقف کانتینر
	@echo "🛑 Stopping Docker container... - توقف کانتینر داکر..."
	docker stop $(PROJECT_NAME)-container || true
	docker rm $(PROJECT_NAME)-container || true
	@echo "✅ Container stopped - کانتینر متوقف شد"

docker-push: docker-build ## Push Docker image to registry - ارسال تصویر به رجیستری
	@echo "📤 Pushing Docker image to registry... - ارسال تصویر به رجیستری..."
	docker tag $(DOCKER_IMAGE):$(DOCKER_TAG) $(DOCKER_REGISTRY)/$(DOCKER_IMAGE):$(DOCKER_TAG)
	docker push $(DOCKER_REGISTRY)/$(DOCKER_IMAGE):$(DOCKER_TAG)
	@echo "✅ Image pushed to registry - تصویر به رجیستری ارسال شد"

docker-compose: ## Run application with docker-compose - اجرای با docker-compose
	@echo "🐳 Starting application with docker-compose... - شروع اپلیکیشن با docker-compose..."
	docker-compose up -d --build
	@echo "✅ Application started with docker-compose - اپلیکیشن با docker-compose شروع شد"

docker-compose-down: ## Stop docker-compose services - توقف سرویس‌های docker-compose
	@echo "🛑 Stopping docker-compose services... - توقف سرویس‌های docker-compose..."
	docker-compose down
	@echo "✅ Services stopped - سرویس‌ها متوقف شد"

## Code Quality Commands - دستورات کیفیت کد
lint: ## Run static code analysis - تحلیل استاتیک کد
	@echo "🔍 Running static code analysis... - تحلیل استاتیک کد..."
	dotnet tool install -g Microsoft.CodeAnalysis.NetAnalyzers 2>$$null || echo "Analyzers already installed"
	dotnet build $(SOLUTION_FILE) --verbosity normal
	@echo "✅ Static analysis completed - تحلیل استاتیک کامل شد"

format: ## Format code according to style guidelines - فرمت کردن کد
	@echo "💅 Formatting code... - فرمت کردن کد..."
	dotnet format $(SOLUTION_FILE) --fix-whitespace --fix-style --fix-analyzers
	@echo "✅ Code formatting completed - فرمت کردن کد کامل شد"

check-format: ## Check if code is properly formatted - بررسی فرمت کد
	@echo "🔍 Checking code format... - بررسی فرمت کد..."
	dotnet format $(SOLUTION_FILE) --verify-no-changes --verbosity diagnostic
	@echo "✅ Format check completed - بررسی فرمت کامل شد"

security-scan: ## Run security vulnerability scanning - بررسی آسیب‌پذیری امنیتی
	@echo "🔒 Running security scan... - بررسی امنیتی..."
	dotnet list package --vulnerable --include-transitive
	dotnet tool install -g Microsoft.CST.DevSkim.CLI 2>$$null || echo "DevSkim already installed"
	devskim analyze .
	@echo "✅ Security scan completed - بررسی امنیتی کامل شد"

## Documentation Commands - دستورات مستندسازی
docs: ## Generate API documentation - تولید مستندات API
	@echo "📚 Generating API documentation... - تولید مستندات API..."
	dotnet tool install -g Swashbuckle.AspNetCore.Cli 2>$$null || echo "Swagger CLI already installed"
	dotnet build $(API_PROJECT)
	swagger tofile --output ./docs/api.json $(API_PROJECT)/bin/Release/net8.0/$(API_PROJECT).dll v1
	@echo "✅ API documentation generated at ./docs/ - مستندات API در ./docs/ تولید شد"

serve-docs: docs ## Serve documentation locally - سرو مستندات محلی
	@echo "🌐 Serving documentation... - سرو مستندات..."
	# Requires Python for simple HTTP server
	cd docs && python -m http.server 8000
	@echo "📖 Documentation available at http://localhost:8000 - مستندات در http://localhost:8000 موجود است"

## Development Commands - دستورات توسعه
dev-setup: ## Setup development environment - راه‌اندازی محیط توسعه
	@echo "🛠️ Setting up development environment... - راه‌اندازی محیط توسعه..."
	dotnet tool install -g dotnet-ef
	dotnet tool install -g dotnet-watch
	dotnet tool install -g dotnet-format
	dotnet tool install -g Microsoft.dotnet-httprepl
	$(MAKE) restore
	$(MAKE) migrate
	@echo "✅ Development environment ready - محیط توسعه آماده است"

dev-run: ## Run development server with hot reload - اجرای سرور توسعه
	@echo "🔥 Starting development server... - شروع سرور توسعه..."
	dotnet watch run --project $(API_PROJECT) --environment Development

dev-watch: ## Watch for changes and auto-rebuild - نظارت و بازسازی خودکار
	@echo "👀 Watching for changes... - نظارت برای تغییرات..."
	dotnet watch build $(SOLUTION_FILE)

## Performance and Monitoring Commands - دستورات عملکرد و نظارت
benchmark: ## Run performance benchmarks - اجرای benchmark عملکرد
	@echo "⚡ Running performance benchmarks... - اجرای benchmark عملکرد..."
	dotnet run --project $(TEST_PROJECT) --configuration Release -- --benchmark
	@echo "✅ Benchmarks completed - benchmark ها کامل شد"

health-check: ## Check application health endpoints - بررسی سلامت اپلیکیشن
	@echo "🏥 Checking application health... - بررسی سلامت اپلیکیشن..."
	curl -f http://localhost:5000/health || echo "❌ Health check failed"
	curl -f http://localhost:5000/health/ready || echo "❌ Readiness check failed"
	@echo "✅ Health checks completed - بررسی سلامت کامل شد"

## Utility Commands - دستورات کاربردی
version: ## Display version information - نمایش اطلاعات نسخه
	@echo "📋 Version Information - اطلاعات نسخه:"
	@echo "  .NET SDK: $$(dotnet --version)"
	@echo "  Project: $(PROJECT_NAME)"
	@echo "  Docker: $$(docker --version 2>/dev/null || echo 'Not installed')"

logs: ## View application logs - مشاهده لاگ‌های اپلیکیشن
	@echo "📜 Viewing application logs... - مشاهده لاگ‌های اپلیکیشن..."
	docker logs $(PROJECT_NAME)-container -f 2>/dev/null || echo "Container not running"

status: ## Show application status - نمایش وضعیت اپلیکیشن
	@echo "📊 Application Status - وضعیت اپلیکیشن:"
	@echo "  Docker Container: $$(docker ps --filter name=$(PROJECT_NAME)-container --format 'table {{.Status}}' | tail -n +2 || echo 'Not running')"
	@echo "  Database: $$(dotnet ef database list --project $(INFRASTRUCTURE_PROJECT) --startup-project $(API_PROJECT) 2>/dev/null | head -n 1 || echo 'Not connected')"

install-tools: ## Install required development tools - نصب ابزارهای توسعه
	@echo "🔧 Installing development tools... - نصب ابزارهای توسعه..."
	dotnet tool install -g dotnet-ef
	dotnet tool install -g dotnet-watch
	dotnet tool install -g dotnet-format
	dotnet tool install -g Microsoft.dotnet-httprepl
	dotnet tool install -g dotnet-reportgenerator-globaltool
	dotnet tool install -g Microsoft.CST.DevSkim.CLI
	@echo "✅ Development tools installed - ابزارهای توسعه نصب شد"

## CI/CD Commands - دستورات CI/CD
ci-build: ## CI/CD build pipeline - پایپ‌لاین ساخت CI/CD
	@echo "🤖 Running CI/CD build pipeline... - اجرای پایپ‌لاین ساخت CI/CD..."
	$(MAKE) clean
	$(MAKE) restore
	$(MAKE) check-format
	$(MAKE) build
	$(MAKE) test
	$(MAKE) security-scan
	$(MAKE) package
	@echo "✅ CI/CD pipeline completed successfully - پایپ‌لاین CI/CD با موفقیت کامل شد"

cd-deploy: ## CD deployment pipeline - پایپ‌لاین استقرار CD
	@echo "🚀 Running CD deployment pipeline... - اجرای پایپ‌لاین استقرار CD..."
	$(MAKE) docker-build
	$(MAKE) docker-push
	$(MAKE) deploy
	@echo "✅ CD pipeline completed successfully - پایپ‌لاین CD با موفقیت کامل شد"

## All-in-one Commands - دستورات همه-در-یک
full-build: ## Complete build with all checks - ساخت کامل با تمام بررسی‌ها
	@echo "🎯 Running complete build process... - اجرای فرآیند ساخت کامل..."
	$(MAKE) clean
	$(MAKE) ci-build
	$(MAKE) docs
	@echo "✅ Full build process completed - فرآیند ساخت کامل انجام شد"

first-run: ## First-time setup and run - راه‌اندازی و اجرای اولیه
	@echo "🎉 Setting up project for first run... - راه‌اندازی پروژه برای اجرای اولیه..."
	$(MAKE) dev-setup
	$(MAKE) build
	$(MAKE) db-seed
	@echo "✅ Project ready! Run 'make dev-run' to start development server - پروژه آماده! 'make dev-run' را برای شروع سرور توسعه اجرا کنید"

## Database Utility Commands - دستورات کاربردی دیتابیس
db-backup: ## Backup database - پشتیبان‌گیری از دیتابیس
	@echo "💾 Creating database backup... - ایجاد پشتیبان از دیتابیس..."
	pg_dump $(DB_CONNECTION) > backup_$$(date +%Y%m%d_%H%M%S).sql
	@echo "✅ Database backup created - پشتیبان دیتابیس ایجاد شد"

db-restore: ## Restore database from backup (requires BACKUP_FILE) - بازیابی دیتابیس از پشتیبان
	@echo "📥 Restoring database from backup... - بازیابی دیتابیس از پشتیبان..."
	psql $(DB_CONNECTION) < $(BACKUP_FILE)
	@echo "✅ Database restored from backup - دیتابیس از پشتیبان بازیابی شد"

## Monitoring Commands - دستورات نظارت
metrics: ## View application metrics - مشاهده معیارهای اپلیکیشن
	@echo "📈 Application Metrics - معیارهای اپلیکیشن:"
	curl -s http://localhost:5000/metrics 2>/dev/null || echo "Metrics endpoint not available"

traces: ## View application traces - مشاهده ردیابی‌های اپلیکیشن
	@echo "🔍 Application Traces - ردیابی‌های اپلیکیشن:"
	# Add your tracing commands here (Jaeger, Zipkin, etc.)
	@echo "Tracing not configured"

## Load Testing Commands - دستورات تست بار
load-test: ## Run load tests against the application - اجرای تست بار
	@echo "⚡ Running load tests... - اجرای تست بار..."
	# Requires k6 or similar load testing tool
	k6 run loadtest/shipping-service-test.js 2>/dev/null || echo "k6 not installed or test script missing"
	@echo "✅ Load tests completed - تست بار کامل شد"

stress-test: ## Run stress tests - اجرای تست استرس
	@echo "💪 Running stress tests... - اجرای تست استرس..."
	# Add your stress testing commands here
	@echo "Stress testing not configured"

## Environment Commands - دستورات محیط
env-dev: ## Setup development environment variables - تنظیم متغیرهای محیط توسعه
	@echo "🛠️ Setting up development environment... - تنظیم محیط توسعه..."
	cp .env.development .env 2>/dev/null || echo ".env.development not found"
	@echo "✅ Development environment configured - محیط توسعه پیکربندی شد"

env-staging: ## Setup staging environment variables - تنظیم متغیرهای محیط staging
	@echo "🎭 Setting up staging environment... - تنظیم محیط staging..."
	cp .env.staging .env 2>/dev/null || echo ".env.staging not found"
	@echo "✅ Staging environment configured - محیط staging پیکربندی شد"

env-prod: ## Setup production environment variables - تنظیم متغیرهای محیط تولید
	@echo "🏭 Setting up production environment... - تنظیم محیط تولید..."
	cp .env.production .env 2>/dev/null || echo ".env.production not found"
	@echo "✅ Production environment configured - محیط تولید پیکربندی شد"

## Cleanup Commands - دستورات پاک‌سازی
clean-all: ## Clean everything including Docker images and volumes - پاک‌سازی همه چیز
	@echo "🧹 Deep cleaning everything... - پاک‌سازی عمیق همه چیز..."
	$(MAKE) clean
	$(MAKE) docker-stop
	docker rmi $(DOCKER_IMAGE):$(DOCKER_TAG) 2>/dev/null || true
	docker system prune -f
	@echo "✅ Deep clean completed - پاک‌سازی عمیق کامل شد"

reset-dev: ## Reset development environment - بازنشانی محیط توسعه
	@echo "🔄 Resetting development environment... - بازنشانی محیط توسعه..."
	$(MAKE) clean-all
	$(MAKE) db-reset
	$(MAKE) dev-setup
	@echo "✅ Development environment reset - محیط توسعه بازنشانی شد"

## Show current configuration - نمایش پیکربندی فعلی
show-config: ## Display current configuration - نمایش پیکربندی فعلی
	@echo "⚙️ Current Configuration - پیکربندی فعلی:"
	@echo "  Project: $(PROJECT_NAME)"
	@echo "  Solution: $(SOLUTION_FILE)"
	@echo "  API Project: $(API_PROJECT)"
	@echo "  Docker Image: $(DOCKER_IMAGE):$(DOCKER_TAG)"
	@echo "  Database: $(DB_CONNECTION)"
	@echo "  Coverage Threshold: $(COVERAGE_THRESHOLD)%"

# ساخت کامل و تست
full-build: clean restore build test

# آماده‌سازی برای production
prepare-prod: clean restore build-release test

# ساخت و اجرا
build-run: build run

# ساخت Docker و اجرا
docker-full: docker-clean docker-build docker-run

# بررسی کامل کیفیت
quality-check: format-check analyze security outdated

# راه‌اندازی محیط توسعه
dev-setup: install-tools restore migrate seed

# پیکربندی CI/CD
ci: clean restore build test pack

# دستور پیش‌فرض
all: help
