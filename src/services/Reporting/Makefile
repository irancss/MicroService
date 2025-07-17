# Reporting Microservice Makefile
# این Makefile شامل تمام دستورات مورد نیاز برای توسعه، بیلد، تست و دیپلوی سرویس گزارش‌گیری می‌باشد

# متغیرها
SOLUTION_NAME = Reporting.sln
SOLUTION_PATH = .
API_PROJECT = ReportingService.API
DOMAIN_PROJECT = ReportingService.Domain  
APPLICATION_PROJECT = ReportingService.Application
INFRASTRUCTURE_PROJECT = ReportingService.Infrastructure

# تنظیمات Docker
DOCKER_IMAGE_NAME = reporting-service
DOCKER_TAG = latest
DOCKER_REGISTRY = localhost:5000

# تنظیمات دیتابیس
DB_CONNECTION_STRING = "Host=localhost;Database=ReportingService_Dev;Username=reporting_user;Password=reporting_pass;"
HANGFIRE_DB_CONNECTION_STRING = "Host=localhost;Database=ReportingService_Hangfire_Dev;Username=reporting_user;Password=reporting_pass;"

# رنگ‌ها برای خروجی
RED = \033[0;31m
GREEN = \033[0;32m
YELLOW = \033[1;33m
BLUE = \033[0;34m
NC = \033[0m # No Color

.PHONY: help install build test clean run dev deploy docker-build docker-run setup-db migrate create-migration

# نمایش راهنما
help: ## نمایش راهنمای دستورات
	@echo "$(BLUE)📋 دستورات موجود برای سرویس گزارش‌گیری:$(NC)"
	@echo ""
	@grep -E '^[a-zA-Z_-]+:.*?## .*$$' $(MAKEFILE_LIST) | sort | awk 'BEGIN {FS = ":.*?## "}; {printf "$(GREEN)%-20s$(NC) %s\n", $$1, $$2}'
	@echo ""

# نصب وابستگی‌ها
install: ## نصب وابستگی‌ها و packages
	@echo "$(YELLOW)📦 نصب وابستگی‌ها...$(NC)"
	dotnet restore $(SOLUTION_PATH)
	@echo "$(GREEN)✅ وابستگی‌ها نصب شدند$(NC)"

# بیلد پروژه
build: ## بیلد تمام پروژه‌ها
	@echo "$(YELLOW)🔨 شروع بیلد...$(NC)"
	dotnet build $(SOLUTION_PATH) --configuration Release --no-restore
	@echo "$(GREEN)✅ بیلد با موفقیت انجام شد$(NC)"

# بیلد سریع برای توسعه
build-dev: ## بیلد سریع برای محیط توسعه
	@echo "$(YELLOW)🔨 بیلد محیط توسعه...$(NC)"
	dotnet build $(SOLUTION_PATH) --configuration Debug --no-restore
	@echo "$(GREEN)✅ بیلد توسعه انجام شد$(NC)"

# تمیزکاری فایل‌های بیلد
clean: ## تمیزکاری فایل‌های بیلد
	@echo "$(YELLOW)🧹 تمیزکاری...$(NC)"
	dotnet clean $(SOLUTION_PATH)
	rm -rf **/bin **/obj
	@echo "$(GREEN)✅ تمیزکاری انجام شد$(NC)"

# اجرای تست‌ها
test: ## اجرای تمام تست‌ها
	@echo "$(YELLOW)🧪 اجرای تست‌ها...$(NC)"
	dotnet test $(SOLUTION_PATH) --configuration Release --no-build --verbosity normal
	@echo "$(GREEN)✅ تست‌ها انجام شدند$(NC)"

# اجرای تست‌ها با coverage
test-coverage: ## اجرای تست‌ها با گزارش coverage
	@echo "$(YELLOW)🧪 اجرای تست‌ها با coverage...$(NC)"
	dotnet test $(SOLUTION_PATH) --collect:"XPlat Code Coverage" --results-directory ./TestResults/
	@echo "$(GREEN)✅ تست‌ها با coverage انجام شدند$(NC)"

# اجرای برنامه
run: ## اجرای API در محیط توسعه
	@echo "$(YELLOW)🚀 اجرای سرویس...$(NC)"
	cd $(API_PROJECT) && dotnet run --configuration Debug
	@echo "$(GREEN)✅ سرویس متوقف شد$(NC)"

# اجرای برنامه در حالت production
run-prod: ## اجرای API در محیط production
	@echo "$(YELLOW)🚀 اجرای سرویس در حالت production...$(NC)"
	cd $(API_PROJECT) && dotnet run --configuration Release --environment Production
	@echo "$(GREEN)✅ سرویس متوقف شد$(NC)"

# اجرای در حالت watch
dev: ## اجرای با auto-reload (حالت توسعه)
	@echo "$(YELLOW)👨‍💻 اجرای حالت توسعه با auto-reload...$(NC)"
	cd $(API_PROJECT) && dotnet watch run
	@echo "$(GREEN)✅ حالت توسعه متوقف شد$(NC)"

# راه‌اندازی دیتابیس
setup-db: ## راه‌اندازی و ایجاد دیتابیس‌ها
	@echo "$(YELLOW)🗄️  راه‌اندازی دیتابیس‌ها...$(NC)"
	@echo "$(BLUE)ایجاد دیتابیس ReportingService...$(NC)"
	-createdb -h localhost -U postgres ReportingService_Dev
	@echo "$(BLUE)ایجاد دیتابیس Hangfire...$(NC)"
	-createdb -h localhost -U postgres ReportingService_Hangfire_Dev
	@echo "$(GREEN)✅ دیتابیس‌ها آماده شدند$(NC)"

# اجرای migrations
migrate: ## اجرای database migrations
	@echo "$(YELLOW)📊 اجرای migrations...$(NC)"
	cd $(INFRASTRUCTURE_PROJECT) && dotnet ef database update --startup-project ../$(API_PROJECT)
	@echo "$(GREEN)✅ Migrations اعمال شدند$(NC)"

# ایجاد migration جدید
create-migration: ## ایجاد migration جدید (NAME=migration_name)
	@echo "$(YELLOW)📝 ایجاد migration جدید...$(NC)"
	@if [ -z "$(NAME)" ]; then \
		echo "$(RED)❌ لطفاً نام migration را مشخص کنید: make create-migration NAME=YourMigrationName$(NC)"; \
		exit 1; \
	fi
	cd $(INFRASTRUCTURE_PROJECT) && dotnet ef migrations add $(NAME) --startup-project ../$(API_PROJECT)
	@echo "$(GREEN)✅ Migration $(NAME) ایجاد شد$(NC)"

# حذف آخرین migration
remove-migration: ## حذف آخرین migration
	@echo "$(YELLOW)🗑️  حذف آخرین migration...$(NC)"
	cd $(INFRASTRUCTURE_PROJECT) && dotnet ef migrations remove --startup-project ../$(API_PROJECT)
	@echo "$(GREEN)✅ آخرین migration حذف شد$(NC)"

# ایجاد Docker image
docker-build: ## ایجاد Docker image
	@echo "$(YELLOW)🐳 ایجاد Docker image...$(NC)"
	docker build -t $(DOCKER_IMAGE_NAME):$(DOCKER_TAG) .
	@echo "$(GREEN)✅ Docker image ایجاد شد$(NC)"

# اجرای container
docker-run: ## اجرای Docker container
	@echo "$(YELLOW)🐳 اجرای Docker container...$(NC)"
	docker run -d --name reporting-service-container \
		-p 8080:80 \
		-e ASPNETCORE_ENVIRONMENT=Production \
		-e ConnectionStrings__DefaultConnection="$(DB_CONNECTION_STRING)" \
		-e ConnectionStrings__HangfireConnection="$(HANGFIRE_DB_CONNECTION_STRING)" \
		$(DOCKER_IMAGE_NAME):$(DOCKER_TAG)
	@echo "$(GREEN)✅ Container در حال اجرا است$(NC)"

# متوقف کردن container
docker-stop: ## متوقف کردن Docker container
	@echo "$(YELLOW)🛑 متوقف کردن container...$(NC)"
	-docker stop reporting-service-container
	-docker rm reporting-service-container
	@echo "$(GREEN)✅ Container متوقف شد$(NC)"

# مشاهده logs
docker-logs: ## مشاهده logs container
	@echo "$(YELLOW)📋 نمایش logs...$(NC)"
	docker logs -f reporting-service-container

# push کردن image به registry
docker-push: ## push کردن image به registry
	@echo "$(YELLOW)📤 push کردن image...$(NC)"
	docker tag $(DOCKER_IMAGE_NAME):$(DOCKER_TAG) $(DOCKER_REGISTRY)/$(DOCKER_IMAGE_NAME):$(DOCKER_TAG)
	docker push $(DOCKER_REGISTRY)/$(DOCKER_IMAGE_NAME):$(DOCKER_TAG)
	@echo "$(GREEN)✅ Image push شد$(NC)"

# راه‌اندازی محیط کامل با Docker Compose
compose-up: ## راه‌اندازی محیط کامل با Docker Compose
	@echo "$(YELLOW)🐳 راه‌اندازی محیط کامل...$(NC)"
	docker-compose up -d
	@echo "$(GREEN)✅ محیط کامل راه‌اندازی شد$(NC)"
	@echo "$(BLUE)📋 Swagger UI: http://localhost:8080$(NC)"
	@echo "$(BLUE)📋 Hangfire Dashboard: http://localhost:8080/hangfire$(NC)"

# متوقف کردن Docker Compose
compose-down: ## متوقف کردن Docker Compose
	@echo "$(YELLOW)🛑 متوقف کردن محیط...$(NC)"
	docker-compose down
	@echo "$(GREEN)✅ محیط متوقف شد$(NC)"

# مشاهده وضعیت services
compose-status: ## نمایش وضعیت services
	@echo "$(YELLOW)📊 وضعیت services:$(NC)"
	docker-compose ps

# نمایش logs تمام services
compose-logs: ## نمایش logs تمام services
	@echo "$(YELLOW)📋 نمایش logs:$(NC)"
	docker-compose logs -f

# تولید sample data برای تست
generate-test-data: ## تولید نمونه داده‌ها برای تست
	@echo "$(YELLOW)📊 تولید نمونه داده‌ها...$(NC)"
	curl -X POST "http://localhost:5000/api/testdata/generate-sample-orders/50" \
		-H "Content-Type: application/json"
	@echo "$(GREEN)✅ نمونه داده‌ها تولید شدند$(NC)"

# اجرای aggregation دستی
run-aggregation: ## اجرای دستی daily sales aggregation
	@echo "$(YELLOW)📊 اجرای aggregation...$(NC)"
	curl -X POST "http://localhost:5000/api/datamanagement/aggregate/daily/$$(date -d yesterday +%Y-%m-%d)" \
		-H "Content-Type: application/json"
	@echo "$(GREEN)✅ Aggregation انجام شد$(NC)"

# بررسی health
health-check: ## بررسی سلامت سرویس
	@echo "$(YELLOW)🏥 بررسی سلامت سرویس...$(NC)"
	curl -s http://localhost:5000/health | jq '.'
	@echo "$(GREEN)✅ بررسی انجام شد$(NC)"

# نمایش متریک‌ها
show-metrics: ## نمایش sample reports
	@echo "$(YELLOW)📊 نمایش گزارش‌های نمونه...$(NC)"
	@echo "$(BLUE)📈 گزارش فروش 30 روز اخیر:$(NC)"
	curl -s "http://localhost:5000/api/salesreports/daily/last30days" | jq '.summary'
	@echo ""
	@echo "$(BLUE)🏆 محصولات پرفروش:$(NC)"
	curl -s "http://localhost:5000/api/productanalytics/top-selling/last30days?topCount=5" | jq '.products[] | {productName, totalRevenue, rank}'

# backup دیتابیس
backup-db: ## تهیه backup از دیتابیس
	@echo "$(YELLOW)💾 تهیه backup...$(NC)"
	pg_dump -h localhost -U postgres -d ReportingService_Dev > backup_$$(date +%Y%m%d_%H%M%S).sql
	@echo "$(GREEN)✅ Backup تهیه شد$(NC)"

# restore دیتابیس
restore-db: ## بازگردانی دیتابیس از backup (FILE=backup.sql)
	@echo "$(YELLOW)🔄 بازگردانی دیتابیس...$(NC)"
	@if [ -z "$(FILE)" ]; then \
		echo "$(RED)❌ لطفاً فایل backup را مشخص کنید: make restore-db FILE=backup.sql$(NC)"; \
		exit 1; \
	fi
	psql -h localhost -U postgres -d ReportingService_Dev < $(FILE)
	@echo "$(GREEN)✅ دیتابیس بازگردانی شد$(NC)"

# بررسی کیفیت کد
lint: ## بررسی کیفیت کد
	@echo "$(YELLOW)🔍 بررسی کیفیت کد...$(NC)"
	dotnet format $(SOLUTION_PATH) --verify-no-changes
	@echo "$(GREEN)✅ کیفیت کد بررسی شد$(NC)"

# فرمت کردن کد
format: ## فرمت کردن کد
	@echo "$(YELLOW)✨ فرمت کردن کد...$(NC)"
	dotnet format $(SOLUTION_PATH)
	@echo "$(GREEN)✅ کد فرمت شد$(NC)"

# آنالیز امنیتی
security-scan: ## اسکن امنیتی dependencies
	@echo "$(YELLOW)🔐 اسکن امنیتی...$(NC)"
	dotnet list $(SOLUTION_PATH) package --vulnerable
	@echo "$(GREEN)✅ اسکن امنیتی انجام شد$(NC)"

# بررسی outdated packages
check-updates: ## بررسی packages قدیمی
	@echo "$(YELLOW)📦 بررسی packages قدیمی...$(NC)"
	dotnet list $(SOLUTION_PATH) package --outdated
	@echo "$(GREEN)✅ بررسی انجام شد$(NC)"

# آپدیت packages
update-packages: ## آپدیت تمام packages
	@echo "$(YELLOW)⬆️  آپدیت packages...$(NC)"
	dotnet list $(SOLUTION_PATH) package --outdated --format json | jq -r '.projects[].frameworks[].topLevelPackages[] | select(.resolvedVersion != .latestVersion) | "\(.id):\(.latestVersion)"' | while read package; do \
		dotnet add package $${package%:*} -v $${package#*:}; \
	done
	@echo "$(GREEN)✅ Packages آپدیت شدند$(NC)"

# پیکربندی development environment
setup-dev: install setup-db migrate ## راه‌اندازی کامل محیط توسعه
	@echo "$(GREEN)🎉 محیط توسعه آماده است!$(NC)"
	@echo "$(BLUE)📋 برای اجرا: make dev$(NC)"
	@echo "$(BLUE)📋 برای تولید داده نمونه: make generate-test-data$(NC)"

# deploy production
deploy-prod: clean install build test docker-build docker-push ## deploy کامل production
	@echo "$(GREEN)🚀 Deploy production انجام شد!$(NC)"

# نمایش اطلاعات پروژه
info: ## نمایش اطلاعات پروژه
	@echo "$(BLUE)📋 اطلاعات پروژه:$(NC)"
	@echo "Solution: $(SOLUTION_NAME)"
	@echo "API Project: $(API_PROJECT)"
	@echo "Domain Project: $(DOMAIN_PROJECT)"
	@echo "Application Project: $(APPLICATION_PROJECT)"
	@echo "Infrastructure Project: $(INFRASTRUCTURE_PROJECT)"
	@echo "Docker Image: $(DOCKER_IMAGE_NAME):$(DOCKER_TAG)"
	@echo ""
	@echo "$(GREEN)🔗 لینک‌های مفید:$(NC)"
	@echo "Swagger UI: http://localhost:5000"
	@echo "Hangfire Dashboard: http://localhost:5000/hangfire"
	@echo "Health Check: http://localhost:5000/health"

# Quick start
quick-start: setup-dev generate-test-data ## شروع سریع (نصب + راه‌اندازی + داده نمونه)
	@echo "$(GREEN)🎉 شروع سریع انجام شد!$(NC)"
	@echo "$(BLUE)🚀 حالا می‌توانید با 'make dev' سرویس را اجرا کنید$(NC)"

# نمایش پیش‌فرض
default: help
