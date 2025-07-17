# راهنمای استقرار (Deployment) سرویس تخفیف

## 📋 فهرست

1. [آماده‌سازی محیط Production](#production-environment)
2. [Docker Deployment](#docker-deployment)
3. [Kubernetes Deployment](#kubernetes-deployment)
4. [CI/CD Pipeline](#cicd-pipeline)
5. [مانیتورینگ و لاگ‌ها](#monitoring-logging)
6. [بک‌آپ و بازیابی](#backup-recovery)

## 🏭 آماده‌سازی محیط Production

### پیش‌نیازها:
- Docker Engine 20.10+
- Docker Compose 2.0+
- PostgreSQL 15+
- Redis 7.0+
- RabbitMQ 3.11+

### متغیرهای محیطی Production:
```bash
# .env.production
# Database
POSTGRES_HOST=your-postgres-server.com
POSTGRES_PORT=5432
POSTGRES_DB=discount_production
POSTGRES_USER=discount_user
POSTGRES_PASSWORD=your_strong_password

# Redis
REDIS_CONNECTION=your-redis-server.com:6379,password=your_redis_password

# RabbitMQ
RABBITMQ_HOST=your-rabbitmq-server.com
RABBITMQ_PORT=5672
RABBITMQ_USERNAME=discount_user
RABBITMQ_PASSWORD=your_rabbitmq_password
RABBITMQ_VHOST=/discount

# JWT
JWT_SECRET=your_very_strong_jwt_secret_key_here_minimum_256_bits
JWT_ISSUER=https://your-company.com
JWT_AUDIENCE=discount-service

# Application
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=http://0.0.0.0:8080
CORS_ORIGINS=https://your-frontend.com,https://your-admin-panel.com

# External Services
USER_SERVICE_BASE_URL=https://api.your-company.com/users
ORDER_SERVICE_BASE_URL=https://api.your-company.com/orders

# Logging
SEQ_SERVER_URL=https://your-seq-server.com
SEQ_API_KEY=your_seq_api_key

# Health Checks
HEALTH_CHECK_UI_PATH=/health-ui
HEALTH_CHECK_API_PATH=/health

# Rate Limiting
RATE_LIMIT_REQUESTS_PER_MINUTE=60
RATE_LIMIT_REQUESTS_PER_HOUR=1000
```

## 🐳 Docker Deployment

### Production Dockerfile:
```dockerfile
# Dockerfile.production
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# Create non-root user
RUN addgroup --system --gid 1001 discount \
    && adduser --system --uid 1001 --ingroup discount discount

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files
COPY ["DiscountService.API/DiscountService.API.csproj", "DiscountService.API/"]
COPY ["DiscountService.Application/DiscountService.Application.csproj", "DiscountService.Application/"]
COPY ["DiscountService.Domain/DiscountService.Domain.csproj", "DiscountService.Domain/"]
COPY ["DiscountService.Infrastructure/DiscountService.Infrastructure.csproj", "DiscountService.Infrastructure/"]

# Restore dependencies
RUN dotnet restore "DiscountService.API/DiscountService.API.csproj"

# Copy source code
COPY . .

# Build application
WORKDIR "/src/DiscountService.API"
RUN dotnet build "DiscountService.API.csproj" -c Release -o /app/build --no-restore

FROM build AS publish
RUN dotnet publish "DiscountService.API.csproj" -c Release -o /app/publish --no-restore

FROM base AS final
WORKDIR /app

# Copy published app
COPY --from=publish /app/publish .

# Set ownership
RUN chown -R discount:discount /app
USER discount

# Health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=60s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "DiscountService.API.dll"]
```

### Docker Compose Production:
```yaml
# docker-compose.production.yml
version: '3.8'

services:
  discount-api:
    image: your-registry.com/discount-service:${VERSION:-latest}
    container_name: discount-api
    restart: unless-stopped
    ports:
      - "8080:8080"
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
    env_file:
      - .env.production
    depends_on:
      - postgres
      - redis
      - rabbitmq
    volumes:
      - ./logs:/app/logs
    networks:
      - discount-network
    healthcheck:
      test: ["CMD", "curl", "-f", "http://localhost:8080/health"]
      interval: 30s
      timeout: 10s
      retries: 3
      start_period: 60s
    deploy:
      resources:
        limits:
          cpus: '1.0'
          memory: 512M
        reservations:
          cpus: '0.5'
          memory: 256M

  postgres:
    image: postgres:15-alpine
    container_name: discount-postgres
    restart: unless-stopped
    environment:
      POSTGRES_DB: ${POSTGRES_DB}
      POSTGRES_USER: ${POSTGRES_USER}
      POSTGRES_PASSWORD: ${POSTGRES_PASSWORD}
    volumes:
      - postgres_data:/var/lib/postgresql/data
      - ./init-scripts:/docker-entrypoint-initdb.d
    networks:
      - discount-network
    ports:
      - "5432:5432"
    healthcheck:
      test: ["CMD-SHELL", "pg_isready -U ${POSTGRES_USER} -d ${POSTGRES_DB}"]
      interval: 10s
      timeout: 5s
      retries: 5

  redis:
    image: redis:7-alpine
    container_name: discount-redis
    restart: unless-stopped
    command: redis-server --requirepass ${REDIS_PASSWORD}
    volumes:
      - redis_data:/data
    networks:
      - discount-network
    ports:
      - "6379:6379"
    healthcheck:
      test: ["CMD", "redis-cli", "--raw", "incr", "ping"]
      interval: 10s
      timeout: 3s
      retries: 5

  rabbitmq:
    image: rabbitmq:3.11-management-alpine
    container_name: discount-rabbitmq
    restart: unless-stopped
    environment:
      RABBITMQ_DEFAULT_USER: ${RABBITMQ_USERNAME}
      RABBITMQ_DEFAULT_PASS: ${RABBITMQ_PASSWORD}
      RABBITMQ_DEFAULT_VHOST: ${RABBITMQ_VHOST}
    volumes:
      - rabbitmq_data:/var/lib/rabbitmq
    networks:
      - discount-network
    ports:
      - "5672:5672"
      - "15672:15672"
    healthcheck:
      test: rabbitmq-diagnostics -q ping
      interval: 30s
      timeout: 10s
      retries: 3

volumes:
  postgres_data:
    driver: local
  redis_data:
    driver: local
  rabbitmq_data:
    driver: local

networks:
  discount-network:
    driver: bridge
```

### اسکریپت Deployment:
```bash
#!/bin/bash
# deploy.sh

set -e

# Variables
VERSION=${1:-latest}
ENV=${2:-production}
REGISTRY="your-registry.com"
SERVICE_NAME="discount-service"

echo "🚀 Starting deployment of $SERVICE_NAME:$VERSION to $ENV environment"

# Build and push Docker image
echo "📦 Building Docker image..."
docker build -f Dockerfile.production -t $REGISTRY/$SERVICE_NAME:$VERSION .

echo "📤 Pushing to registry..."
docker push $REGISTRY/$SERVICE_NAME:$VERSION

# Deploy with Docker Compose
echo "🔄 Deploying services..."
VERSION=$VERSION docker-compose -f docker-compose.$ENV.yml up -d

# Wait for services to be healthy
echo "⏳ Waiting for services to be healthy..."
timeout 300 bash -c '
  while [[ "$(docker inspect --format="{{.State.Health.Status}}" discount-api)" != "healthy" ]]; do
    echo "Waiting for discount-api to be healthy..."
    sleep 10
  done
'

# Run database migrations
echo "🗄️ Running database migrations..."
docker exec discount-api dotnet ef database update --no-build

# Verify deployment
echo "✅ Verifying deployment..."
curl -f http://localhost:8080/health || exit 1

echo "🎉 Deployment completed successfully!"
```

## ☸️ Kubernetes Deployment

### Namespace:
```yaml
# k8s/namespace.yaml
apiVersion: v1
kind: Namespace
metadata:
  name: discount-service
  labels:
    name: discount-service
```

### ConfigMap:
```yaml
# k8s/configmap.yaml
apiVersion: v1
kind: ConfigMap
metadata:
  name: discount-config
  namespace: discount-service
data:
  ASPNETCORE_ENVIRONMENT: "Production"
  ASPNETCORE_URLS: "http://0.0.0.0:8080"
  POSTGRES_HOST: "postgres-service"
  POSTGRES_PORT: "5432"
  POSTGRES_DB: "discount_production"
  REDIS_HOST: "redis-service"
  REDIS_PORT: "6379"
  RABBITMQ_HOST: "rabbitmq-service"
  RABBITMQ_PORT: "5672"
```

### Secret:
```yaml
# k8s/secret.yaml
apiVersion: v1
kind: Secret
metadata:
  name: discount-secrets
  namespace: discount-service
type: Opaque
data:
  # Base64 encoded values
  POSTGRES_PASSWORD: eW91cl9zdHJvbmdfcGFzc3dvcmQ=
  JWT_SECRET: eW91cl92ZXJ5X3N0cm9uZ19qd3Rfc2VjcmV0X2tleQ==
  REDIS_PASSWORD: eW91cl9yZWRpc19wYXNzd29yZA==
  RABBITMQ_PASSWORD: eW91cl9yYWJiaXRtcV9wYXNzd29yZA==
```

### Deployment:
```yaml
# k8s/deployment.yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: discount-api
  namespace: discount-service
  labels:
    app: discount-api
spec:
  replicas: 3
  selector:
    matchLabels:
      app: discount-api
  template:
    metadata:
      labels:
        app: discount-api
    spec:
      containers:
      - name: discount-api
        image: your-registry.com/discount-service:latest
        imagePullPolicy: Always
        ports:
        - containerPort: 8080
        env:
        - name: POSTGRES_PASSWORD
          valueFrom:
            secretKeyRef:
              name: discount-secrets
              key: POSTGRES_PASSWORD
        - name: JWT_SECRET
          valueFrom:
            secretKeyRef:
              name: discount-secrets
              key: JWT_SECRET
        envFrom:
        - configMapRef:
            name: discount-config
        resources:
          requests:
            memory: "256Mi"
            cpu: "250m"
          limits:
            memory: "512Mi"
            cpu: "500m"
        livenessProbe:
          httpGet:
            path: /health
            port: 8080
          initialDelaySeconds: 60
          periodSeconds: 30
          timeoutSeconds: 10
          failureThreshold: 3
        readinessProbe:
          httpGet:
            path: /health/ready
            port: 8080
          initialDelaySeconds: 30
          periodSeconds: 10
          timeoutSeconds: 5
          failureThreshold: 3
      imagePullSecrets:
      - name: registry-secret
```

### Service & Ingress:
```yaml
# k8s/service.yaml
apiVersion: v1
kind: Service
metadata:
  name: discount-api-service
  namespace: discount-service
spec:
  selector:
    app: discount-api
  ports:
  - protocol: TCP
    port: 80
    targetPort: 8080
  type: ClusterIP

---
# k8s/ingress.yaml
apiVersion: networking.k8s.io/v1
kind: Ingress
metadata:
  name: discount-api-ingress
  namespace: discount-service
  annotations:
    kubernetes.io/ingress.class: nginx
    cert-manager.io/cluster-issuer: letsencrypt-prod
    nginx.ingress.kubernetes.io/rate-limit: "100"
spec:
  tls:
  - hosts:
    - api.yourcompany.com
    secretName: discount-api-tls
  rules:
  - host: api.yourcompany.com
    http:
      paths:
      - path: /discount
        pathType: Prefix
        backend:
          service:
            name: discount-api-service
            port:
              number: 80
```

### HPA (Horizontal Pod Autoscaler):
```yaml
# k8s/hpa.yaml
apiVersion: autoscaling/v2
kind: HorizontalPodAutoscaler
metadata:
  name: discount-api-hpa
  namespace: discount-service
spec:
  scaleTargetRef:
    apiVersion: apps/v1
    kind: Deployment
    name: discount-api
  minReplicas: 2
  maxReplicas: 10
  metrics:
  - type: Resource
    resource:
      name: cpu
      target:
        type: Utilization
        averageUtilization: 70
  - type: Resource
    resource:
      name: memory
      target:
        type: Utilization
        averageUtilization: 80
```

## 🔄 CI/CD Pipeline

### GitHub Actions:
```yaml
# .github/workflows/deploy.yml
name: Deploy to Production

on:
  push:
    tags:
      - 'v*'

env:
  REGISTRY: your-registry.com
  IMAGE_NAME: discount-service

jobs:
  build-and-deploy:
    runs-on: ubuntu-latest
    
    steps:
    - name: Checkout code
      uses: actions/checkout@v3

    - name: Set up Docker Buildx
      uses: docker/setup-buildx-action@v2

    - name: Login to Container Registry
      uses: docker/login-action@v2
      with:
        registry: ${{ env.REGISTRY }}
        username: ${{ secrets.REGISTRY_USERNAME }}
        password: ${{ secrets.REGISTRY_PASSWORD }}

    - name: Extract metadata
      id: meta
      uses: docker/metadata-action@v4
      with:
        images: ${{ env.REGISTRY }}/${{ env.IMAGE_NAME }}
        tags: |
          type=ref,event=tag
          type=raw,value=latest

    - name: Build and push Docker image
      uses: docker/build-push-action@v3
      with:
        context: .
        file: ./Dockerfile.production
        push: true
        tags: ${{ steps.meta.outputs.tags }}
        labels: ${{ steps.meta.outputs.labels }}
        cache-from: type=gha
        cache-to: type=gha,mode=max

    - name: Deploy to Kubernetes
      uses: azure/k8s-deploy@v1
      with:
        manifests: |
          k8s/namespace.yaml
          k8s/configmap.yaml
          k8s/secret.yaml
          k8s/deployment.yaml
          k8s/service.yaml
          k8s/ingress.yaml
          k8s/hpa.yaml
        images: |
          ${{ env.REGISTRY }}/${{ env.IMAGE_NAME }}:${{ github.ref_name }}
        kubectl-version: 'latest'

    - name: Verify deployment
      run: |
        kubectl rollout status deployment/discount-api -n discount-service
        kubectl get pods -n discount-service
```

### GitLab CI/CD:
```yaml
# .gitlab-ci.yml
stages:
  - build
  - test
  - deploy

variables:
  DOCKER_DRIVER: overlay2
  DOCKER_TLS_CERTDIR: "/certs"

build:
  stage: build
  image: docker:latest
  services:
    - docker:dind
  script:
    - docker build -f Dockerfile.production -t $CI_REGISTRY_IMAGE:$CI_COMMIT_SHA .
    - docker push $CI_REGISTRY_IMAGE:$CI_COMMIT_SHA
  only:
    - main
    - tags

deploy-production:
  stage: deploy
  image: bitnami/kubectl:latest
  script:
    - kubectl config use-context $KUBE_CONTEXT
    - kubectl set image deployment/discount-api discount-api=$CI_REGISTRY_IMAGE:$CI_COMMIT_SHA -n discount-service
    - kubectl rollout status deployment/discount-api -n discount-service
  only:
    - tags
  when: manual
```

## 📊 مانیتورینگ و لاگ‌ها

### Prometheus Configuration:
```yaml
# monitoring/prometheus.yml
global:
  scrape_interval: 15s

scrape_configs:
  - job_name: 'discount-api'
    static_configs:
      - targets: ['discount-api-service:80']
    metrics_path: '/metrics'
    scrape_interval: 10s
```

### Grafana Dashboard:
```json
{
  "dashboard": {
    "title": "Discount Service Metrics",
    "panels": [
      {
        "title": "Request Rate",
        "type": "graph",
        "targets": [
          {
            "expr": "rate(http_requests_total{job=\"discount-api\"}[5m])"
          }
        ]
      },
      {
        "title": "Response Time",
        "type": "graph",
        "targets": [
          {
            "expr": "histogram_quantile(0.95, rate(http_request_duration_seconds_bucket{job=\"discount-api\"}[5m]))"
          }
        ]
      },
      {
        "title": "Error Rate",
        "type": "graph",
        "targets": [
          {
            "expr": "rate(http_requests_total{job=\"discount-api\",status=~\"5..\"}[5m])"
          }
        ]
      }
    ]
  }
}
```

### ELK Stack Configuration:
```yaml
# logging/docker-compose.elk.yml
version: '3.8'

services:
  elasticsearch:
    image: docker.elastic.co/elasticsearch/elasticsearch:8.5.0
    environment:
      - discovery.type=single-node
      - xpack.security.enabled=false
    volumes:
      - es_data:/usr/share/elasticsearch/data

  logstash:
    image: docker.elastic.co/logstash/logstash:8.5.0
    volumes:
      - ./logstash.conf:/usr/share/logstash/pipeline/logstash.conf

  kibana:
    image: docker.elastic.co/kibana/kibana:8.5.0
    ports:
      - "5601:5601"
    environment:
      - ELASTICSEARCH_HOSTS=http://elasticsearch:9200

volumes:
  es_data:
```

## 💾 بک‌آپ و بازیابی

### Database Backup Script:
```bash
#!/bin/bash
# scripts/backup-database.sh

DATE=$(date +%Y%m%d_%H%M%S)
BACKUP_DIR="/backups"
DATABASE="discount_production"

# Create backup
pg_dump -h $POSTGRES_HOST -U $POSTGRES_USER -d $DATABASE > $BACKUP_DIR/discount_$DATE.sql

# Compress backup
gzip $BACKUP_DIR/discount_$DATE.sql

# Upload to S3 (optional)
aws s3 cp $BACKUP_DIR/discount_$DATE.sql.gz s3://your-backups/discount/

# Clean old backups (keep last 30 days)
find $BACKUP_DIR -name "discount_*.sql.gz" -mtime +30 -delete

echo "Backup completed: discount_$DATE.sql.gz"
```

### Restore Script:
```bash
#!/bin/bash
# scripts/restore-database.sh

BACKUP_FILE=$1

if [ -z "$BACKUP_FILE" ]; then
  echo "Usage: $0 <backup_file>"
  exit 1
fi

# Download from S3 if needed
if [[ $BACKUP_FILE == s3://* ]]; then
  aws s3 cp $BACKUP_FILE /tmp/
  BACKUP_FILE="/tmp/$(basename $BACKUP_FILE)"
fi

# Decompress if needed
if [[ $BACKUP_FILE == *.gz ]]; then
  gunzip $BACKUP_FILE
  BACKUP_FILE=${BACKUP_FILE%.gz}
fi

# Restore database
psql -h $POSTGRES_HOST -U $POSTGRES_USER -d $DATABASE < $BACKUP_FILE

echo "Database restored from $BACKUP_FILE"
```

## 🚀 عملیات روزانه

### Deployment Commands:
```bash
# Production deployment
make deploy ENV=production VERSION=v1.2.3

# Rolling update
kubectl rolling-update discount-api --image=your-registry.com/discount-service:v1.2.3

# Rollback
kubectl rollout undo deployment/discount-api -n discount-service

# Scale up/down
kubectl scale deployment discount-api --replicas=5 -n discount-service

# Check logs
kubectl logs -f deployment/discount-api -n discount-service

# Database migration
kubectl exec -it deployment/discount-api -n discount-service -- dotnet ef database update
```

### Health Checks:
```bash
# Service health
curl https://api.yourcompany.com/discount/health

# Database connectivity
curl https://api.yourcompany.com/discount/health/db

# Cache connectivity  
curl https://api.yourcompany.com/discount/health/cache

# Message queue connectivity
curl https://api.yourcompany.com/discount/health/messagequeue
```

---

**نکته مهم:** همواره قبل از deployment در production، تست‌های یکپارچگی را در محیط staging اجرا کنید.
