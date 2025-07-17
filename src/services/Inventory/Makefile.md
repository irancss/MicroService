# 🛠️ **Makefile برای میکروسرویس موجودی**

## ⚡ **دستورات سریع:**

```bash
# 🧹 پاک‌سازی کامل
make clean

# 📦 بازیابی وابستگی‌ها
make restore

# 🏗️ ساخت پروژه
make build

# 🧪 اجرای تست‌ها
make test

# 🚀 اجرای پروژه
make run

# 📊 بررسی سلامت
make health

# 🐳 کار با Docker
make docker-build
make docker-run

# 🛠️ توسعه
make watch
```

---

## 📋 **جزئیات دستورات:**

### تنظیم اولیه:
```bash
make setup        # نصب همه dependency ها
make db-setup     # تنظیم دیتابیس
```

### توسعه:
```bash
make dev          # محیط توسعه کامل
make watch        # حالت نظارت
make format       # قالب‌بندی کد
```

### تست:
```bash
make test-unit    # تست‌های واحد
make test-integration  # تست‌های یکپارچگی
make coverage     # پوشش تست‌ها
```

### استقرار:
```bash
make package      # بسته‌بندی
make deploy       # استقرار
```

---

## 🎯 **مقدار بازگشتی:**
- **0**: موفقیت
- **1**: خطا در کامپایل
- **2**: خطا در تست‌ها
- **3**: خطا در استقرار
