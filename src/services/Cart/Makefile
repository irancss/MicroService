# Cart Microservice Makefile
# میک‌فایل میکروسرویس سبد خرید

.PHONY: help build test run clean docker-build docker-run docker-stop restore format lint

# Default target
help: ## Show this help message
	@echo "Cart Microservice - Available Commands:"
	@echo "میکروسرویس سبد خرید - دستورات موجود:"
	@echo ""
	@grep -E '^[a-zA-Z_-]+:.*?## .*$$' $(MAKEFILE_LIST) | sort | awk 'BEGIN {FS = ":.*?## "}; {printf "\033[36m%-20s\033[0m %s\n", $$1, $$2}'

# Development Commands | دستورات توسعه
restore: ## Restore NuGet packages | بازیابی پکیج‌های NuGet
	@echo "🔄 Restoring packages..."
	dotnet restore Cart.sln

build: restore ## Build the solution | ساخت پروژه
	@echo "🏗️ Building solution..."
	dotnet build Cart.sln --configuration Release --no-restore

clean: ## Clean build artifacts | پاکسازی فایل‌های ساخت
	@echo "🧹 Cleaning..."
	dotnet clean Cart.sln
	rm -rf */bin */obj

format: ## Format code using dotnet format | فرمت کردن کد
	@echo "✨ Formatting code..."
	dotnet format Cart.sln

test: build ## Run all tests | اجرای تست‌ها
	@echo "🧪 Running tests..."
	dotnet test Cart.sln --configuration Release --no-build --verbosity normal

test-watch: ## Run tests in watch mode | اجرای تست در حالت watch
	@echo "👀 Running tests in watch mode..."
	dotnet watch test CartService.Tests --verbosity normal

# Local Development | توسعه محلی
run: ## Run the API locally | اجرای API در محیط محلی
	@echo "🚀 Starting Cart API..."
	cd CartService.API && dotnet run

run-watch: ## Run the API with hot reload | اجرای API با بارگذاری مجدد خودکار
	@echo "🔥 Starting Cart API with hot reload..."
	cd CartService.API && dotnet watch run

dev-setup: ## Setup development environment | راه‌اندازی محیط توسعه
	@echo "⚙️ Setting up development environment..."
	@echo "Installing dotnet tools..."
	dotnet tool install --global dotnet-ef
	dotnet tool install --global dotnet-format
	@echo "✅ Development environment ready!"

# Database Commands | دستورات پایگاه داده
redis-start: ## Start Redis container | شروع کانتینر Redis
	@echo "🔴 Starting Redis..."
	docker run -d --name cart-redis -p 6379:6379 redis:7-alpine
	@echo "✅ Redis started on port 6379"

redis-stop: ## Stop Redis container | توقف کانتینر Redis
	@echo "🛑 Stopping Redis..."
	docker stop cart-redis && docker rm cart-redis

redis-cli: ## Connect to Redis CLI | اتصال به Redis CLI
	@echo "💻 Connecting to Redis CLI..."
	docker exec -it cart-redis redis-cli

postgres-start: ## Start PostgreSQL for Hangfire | شروع PostgreSQL برای Hangfire
	@echo "🐘 Starting PostgreSQL..."
	docker run -d --name cart-postgres \
		-e POSTGRES_DB=cart_hangfire \
		-e POSTGRES_USER=postgres \
		-e POSTGRES_PASSWORD=password \
		-p 5432:5432 \
		postgres:15-alpine
	@echo "✅ PostgreSQL started on port 5432"

postgres-stop: ## Stop PostgreSQL container | توقف کانتینر PostgreSQL
	@echo "🛑 Stopping PostgreSQL..."
	docker stop cart-postgres && docker rm cart-postgres

rabbitmq-start: ## Start RabbitMQ container | شروع کانتینر RabbitMQ
	@echo "🐰 Starting RabbitMQ..."
	docker run -d --name cart-rabbitmq \
		-p 5672:5672 \
		-p 15672:15672 \
		rabbitmq:3-management-alpine
	@echo "✅ RabbitMQ started - Management UI: http://localhost:15672"

rabbitmq-stop: ## Stop RabbitMQ container | توقف کانتینر RabbitMQ
	@echo "🛑 Stopping RabbitMQ..."
	docker stop cart-rabbitmq && docker rm cart-rabbitmq

# Docker Commands | دستورات Docker
docker-build: ## Build Docker image | ساخت تصویر Docker
	@echo "🐳 Building Docker image..."
	docker build -t cart-microservice:latest -f CartService.API/Dockerfile .

docker-run: docker-build ## Run Docker container | اجرای کانتینر Docker
	@echo "🚀 Running Docker container..."
	docker run -d --name cart-api \
		-p 5001:8080 \
		-e ASPNETCORE_ENVIRONMENT=Development \
		cart-microservice:latest

docker-stop: ## Stop Docker container | توقف کانتینر Docker
	@echo "🛑 Stopping Docker container..."
	docker stop cart-api && docker rm cart-api

docker-logs: ## View Docker container logs | مشاهده لاگ‌های کانتینر
	@echo "📋 Viewing container logs..."
	docker logs -f cart-api

# Docker Compose Commands | دستورات Docker Compose
compose-up: ## Start all services with docker-compose | شروع تمام سرویس‌ها با docker-compose
	@echo "🚀 Starting all services..."
	docker-compose up -d
	@echo "✅ All services started!"
	@echo "🌐 Cart API: https://localhost:5001"
	@echo "📚 Swagger: https://localhost:5001/swagger"
	@echo "🔧 Hangfire: https://localhost:5001/hangfire"

compose-down: ## Stop all services | توقف تمام سرویس‌ها
	@echo "🛑 Stopping all services..."
	docker-compose down

compose-logs: ## View all services logs | مشاهده لاگ‌های تمام سرویس‌ها
	@echo "📋 Viewing all services logs..."
	docker-compose logs -f

compose-build: ## Build and start all services | ساخت و شروع تمام سرویس‌ها
	@echo "🏗️ Building and starting all services..."
	docker-compose up -d --build

compose-restart: ## Restart all services | راه‌اندازی مجدد تمام سرویس‌ها
	@echo "🔄 Restarting all services..."
	docker-compose restart

# Health Checks | بررسی سلامت
health-check: ## Check API health | بررسی سلامت API
	@echo "🏥 Checking API health..."
	@curl -f http://localhost:5001/health || echo "❌ API is not healthy"

health-redis: ## Check Redis connection | بررسی اتصال Redis
	@echo "🔴 Checking Redis..."
	@docker exec cart-redis redis-cli ping || echo "❌ Redis is not running"

health-postgres: ## Check PostgreSQL connection | بررسی اتصال PostgreSQL
	@echo "🐘 Checking PostgreSQL..."
	@docker exec cart-postgres pg_isready || echo "❌ PostgreSQL is not running"

health-all: health-check health-redis health-postgres ## Check all services health | بررسی سلامت تمام سرویس‌ها

# Testing Commands | دستورات تست
test-unit: ## Run unit tests only | اجرای تست‌های واحد فقط
	@echo "🧪 Running unit tests..."
	dotnet test --filter "Category=Unit" --verbosity normal

test-integration: compose-up ## Run integration tests | اجرای تست‌های یکپارچگی
	@echo "🔗 Running integration tests..."
	sleep 10  # Wait for services to be ready
	dotnet test --filter "Category=Integration" --verbosity normal

test-coverage: ## Generate test coverage report | تولید گزارش پوشش تست
	@echo "📊 Generating test coverage..."
	dotnet test --collect:"XPlat Code Coverage" --results-directory:./TestResults
	@echo "📈 Coverage report generated in ./TestResults"

# Code Quality | کیفیت کد
lint: ## Run code analysis | تحلیل کد
	@echo "🔍 Running code analysis..."
	dotnet format --verify-no-changes Cart.sln

security-scan: ## Run security scan | اسکن امنیتی
	@echo "🔒 Running security scan..."
	dotnet list Cart.sln package --vulnerable --include-transitive

outdated: ## Check for outdated packages | بررسی پکیج‌های قدیمی
	@echo "📦 Checking for outdated packages..."
	dotnet list Cart.sln package --outdated

# Documentation | مستندات
docs-serve: ## Serve API documentation | سرو مستندات API
	@echo "📚 Starting documentation server..."
	@echo "🌐 Swagger UI: https://localhost:5001/swagger"
	@echo "📖 Persian Docs: README_FA.md"
	@echo "🏗️ Architecture: ARCHITECTURE.md"

# Performance | عملکرد
benchmark: ## Run performance benchmarks | اجرای بنچمارک عملکرد
	@echo "⚡ Running benchmarks..."
	@echo "Use tools like NBomber or Artillery for load testing"
	@echo "Artillery example: artillery quick --duration 60 --rate 10 https://localhost:5001/api/v1/carts"

load-test: compose-up ## Run basic load test | اجرای تست بار اولیه
	@echo "🚛 Running basic load test..."
	@echo "Installing artillery if not present..."
	@npm install -g artillery || echo "Please install artillery: npm install -g artillery"
	@echo "Running load test..."
	@artillery quick --duration 30 --rate 5 https://localhost:5001/health

# Deployment | استقرار
deploy-staging: ## Deploy to staging | استقرار در staging
	@echo "🚀 Deploying to staging..."
	@echo "This would typically involve:"
	@echo "1. docker build -t cart-microservice:staging ."
	@echo "2. docker tag cart-microservice:staging registry.company.com/cart:staging"
	@echo "3. docker push registry.company.com/cart:staging"
	@echo "4. kubectl apply -f k8s/staging/"

deploy-prod: ## Deploy to production | استقرار در تولید
	@echo "🚀 Deploying to production..."
	@echo "⚠️ This would typically involve:"
	@echo "1. Thorough testing"
	@echo "2. Database migrations"
	@echo "3. Blue-green deployment"
	@echo "4. Health checks"
	@echo "Please implement proper CI/CD pipeline!"

# Maintenance | نگهداری
logs: ## View application logs | مشاهده لاگ‌های برنامه
	@echo "📋 Recent application logs:"
	@tail -f CartService.API/logs/*.log 2>/dev/null || echo "No log files found. Run the application first."

monitor: ## Show monitoring commands | نمایش دستورات مانیتورینگ
	@echo "📊 Monitoring commands:"
	@echo "🔧 Hangfire Dashboard: https://localhost:5001/hangfire"
	@echo "🔴 Redis Monitor: docker exec -it cart-redis redis-cli monitor"
	@echo "🐘 PostgreSQL Stats: docker exec -it cart-postgres psql -U postgres -d cart_hangfire -c 'SELECT * FROM hangfire.job;'"

backup: ## Backup data | پشتیبان‌گیری داده‌ها
	@echo "💾 Creating backup..."
	@echo "Redis backup:"
	@docker exec cart-redis redis-cli BGSAVE
	@echo "PostgreSQL backup:"
	@docker exec cart-postgres pg_dump -U postgres cart_hangfire > backup_$(shell date +%Y%m%d_%H%M%S).sql
	@echo "✅ Backup completed"

# Development Utilities | ابزارهای توسعه
sample-data: ## Load sample data for testing | بارگذاری داده‌های نمونه برای تست
	@echo "📊 Loading sample data..."
	@echo "Creating sample carts..."
	@curl -X POST "https://localhost:5001/api/v1/carts/items" \
		-H "Content-Type: application/json" \
		-H "Authorization: Bearer sample-token" \
		-d '{"productId": "sample-product-1", "quantity": 2, "priceAtTimeOfAddition": 50000}' \
		|| echo "❌ Failed to create sample data. Make sure API is running."

reset: compose-down clean ## Reset everything | بازنشانی همه چیز
	@echo "🔄 Resetting everything..."
	docker system prune -f
	@echo "✅ Reset completed"

# Quick Start | شروع سریع
quick-start: restore build compose-up health-all ## Quick start for development | شروع سریع برای توسعه
	@echo "🎉 Quick start completed!"
	@echo ""
	@echo "🌐 Cart API: https://localhost:5001"
	@echo "📚 Swagger UI: https://localhost:5001/swagger"
	@echo "🔧 Hangfire Dashboard: https://localhost:5001/hangfire"
	@echo "🔴 Redis Commander: http://localhost:8081"
	@echo "🐘 pgAdmin: http://localhost:8080 (admin@example.com / admin)"
	@echo "🐰 RabbitMQ Management: http://localhost:15672 (guest / guest)"
	@echo ""
	@echo "✨ Happy coding! / کدنویسی موفق!"

# Status | وضعیت
status: ## Show services status | نمایش وضعیت سرویس‌ها
	@echo "📊 Services Status:"
	@echo ""
	@echo "🐳 Docker Containers:"
	@docker ps --format "table {{.Names}}\t{{.Status}}\t{{.Ports}}" | grep -E "(cart|redis|postgres|rabbitmq)" || echo "No containers running"
	@echo ""
	@echo "🔌 Port Usage:"
	@lsof -i :5001 2>/dev/null | head -2 || echo "Port 5001: Not in use"
	@lsof -i :6379 2>/dev/null | head -2 || echo "Port 6379: Not in use"
	@lsof -i :5432 2>/dev/null | head -2 || echo "Port 5432: Not in use"
	@lsof -i :15672 2>/dev/null | head -2 || echo "Port 15672: Not in use"

# بخش راهنما باللای فایل
info: ## Show project information | نمایش اطلاعات پروژه
	@echo "🛒 Cart Microservice - Dual-Cart E-commerce Solution"
	@echo "میکروسرویس سبد خرید - راهکار تجارت الکترونیک دوگانه"
	@echo ""
	@echo "📋 Project Structure:"
	@echo "├── CartService.API/          # RESTful API Layer"
	@echo "├── CartService.Application/  # CQRS + MediatR Layer"
	@echo "├── CartService.Infrastructure/ # External Services Layer"
	@echo "├── CartService.Domain/       # Domain Entities & Events"
	@echo "├── CartService.Tests/        # Unit & Integration Tests"
	@echo "├── docker-compose.yml        # Multi-service setup"
	@echo "└── README_FA.md             # Complete Persian documentation"
	@echo ""
	@echo "🎯 Key Features:"
	@echo "• Dual-cart system (Active + Next-Purchase)"
	@echo "• Intelligent next-purchase activation"
	@echo "• Advanced abandonment recovery"
	@echo "• CQRS with MediatR"
	@echo "• Redis for high-performance storage"
	@echo "• Hangfire for background jobs"
	@echo "• gRPC for service communication"
	@echo ""
	@echo "🚀 Quick Start: make quick-start"
	@echo "📚 Help: make help"
