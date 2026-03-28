---
type: "EXAMPLE"
title: "Backend Deployment"
---


**Deploying Serverpod to Production**

This example demonstrates deploying a Serverpod backend to Railway or Fly.io with proper database migration, environment configuration, health checks, and monitoring setup. These patterns apply whether you're using Serverpod, custom Dart backends, or other backend frameworks.



```yaml
# ============================================================
# Dockerfile for Serverpod Production Deployment
# ============================================================
# Dockerfile
FROM dart:stable AS build

WORKDIR /app

# Copy pubspec files first for better layer caching
COPY pubspec.* ./
COPY your_app_server/pubspec.* ./your_app_server/

# Get dependencies
RUN dart pub get
WORKDIR /app/your_app_server
RUN dart pub get

# Copy source code
WORKDIR /app
COPY . .

# Build the server
WORKDIR /app/your_app_server
RUN dart compile exe bin/main.dart -o bin/server

# Production image
FROM debian:bookworm-slim

RUN apt-get update && apt-get install -y \
    ca-certificates \
    curl \
    && rm -rf /var/lib/apt/lists/*

WORKDIR /app

# Copy compiled binary
COPY --from=build /app/your_app_server/bin/server /app/server
COPY --from=build /app/your_app_server/config/ /app/config/
COPY --from=build /app/your_app_server/web/ /app/web/
COPY --from=build /app/your_app_server/migrations/ /app/migrations/

# Health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=5s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

EXPOSE 8080 8081 8082

CMD ["./server", "--mode", "production", "--apply-migrations"]

---

# ============================================================
# Railway Configuration (railway.toml)
# ============================================================
[build]
builder = "dockerfile"
dockerfilePath = "Dockerfile"

[deploy]
startCommand = "./server --mode production --apply-migrations"
healthcheckPath = "/health"
healthcheckTimeout = 30
restartPolicyType = "always"
numReplicas = 2

---

# ============================================================
# Fly.io Configuration (fly.toml)
# ============================================================
app = "your-app-name"
primary_region = "iad"

[build]
  dockerfile = "Dockerfile"

[env]
  SERVERPOD_MODE = "production"
  PORT = "8080"

[http_service]
  internal_port = 8080
  force_https = true
  auto_stop_machines = false
  auto_start_machines = true
  min_machines_running = 2
  processes = ["app"]

  [http_service.concurrency]
    type = "requests"
    hard_limit = 250
    soft_limit = 200

[[services]]
  protocol = "tcp"
  internal_port = 8081

  [[services.ports]]
    port = 8081
    handlers = ["tls"]

[[vm]]
  memory = "1gb"
  cpu_kind = "shared"
  cpus = 2

[checks]
  [checks.health]
    port = 8080
    type = "http"
    interval = "15s"
    timeout = "10s"
    grace_period = "30s"
    method = "GET"
    path = "/health"

---

# ============================================================
# Production Configuration (config/production.yaml)
# ============================================================
# config/production.yaml
apiServer:
  port: 8080
  publicHost: api.yourapp.com
  publicPort: 443
  publicScheme: https

insightsServer:
  port: 8081
  publicHost: insights.yourapp.com
  publicPort: 443
  publicScheme: https

webServer:
  port: 8082
  publicHost: web.yourapp.com
  publicPort: 443
  publicScheme: https

database:
  host: ${DB_HOST}
  port: ${DB_PORT}
  name: ${DB_NAME}
  user: ${DB_USER}
  password: ${DB_PASSWORD}
  requireSsl: true

redis:
  enabled: true
  host: ${REDIS_HOST}
  port: ${REDIS_PORT}
  password: ${REDIS_PASSWORD}

---

# ============================================================
# Database Migration Script
# ============================================================
#!/bin/bash
# scripts/migrate.sh

set -e

echo "Starting database migration..."

# Run migrations
cd your_app_server
dart bin/main.dart --mode production --apply-migrations --role maintenance

echo "Migration completed successfully"

# Verify migration
psql "$DATABASE_URL" -c "SELECT * FROM serverpod_migrations ORDER BY installed_rank DESC LIMIT 5;"

echo "Recent migrations:"
psql "$DATABASE_URL" -c "SELECT version, description, installed_on FROM serverpod_migrations ORDER BY installed_rank DESC LIMIT 5;"

---

# ============================================================
# Health Check Endpoint Implementation
# ============================================================
// lib/src/endpoints/health_endpoint.dart
import 'package:serverpod/serverpod.dart';

class HealthEndpoint extends Endpoint {
  @override
  bool get requireLogin => false;

  Future<HealthStatus> check(Session session) async {
    final checks = <String, HealthCheck>{};
    var overallHealthy = true;

    // Database check
    try {
      final dbStart = DateTime.now();
      await session.db.query('SELECT 1');
      final dbLatency = DateTime.now().difference(dbStart).inMilliseconds;
      checks['database'] = HealthCheck(
        healthy: true,
        latencyMs: dbLatency,
        message: 'Connected',
      );
    } catch (e) {
      checks['database'] = HealthCheck(
        healthy: false,
        message: 'Connection failed: $e',
      );
      overallHealthy = false;
    }

    // Redis check
    try {
      final redisStart = DateTime.now();
      await session.caches.local.put('health_check', 'ok');
      final value = await session.caches.local.get<String>('health_check');
      final redisLatency = DateTime.now().difference(redisStart).inMilliseconds;
      checks['redis'] = HealthCheck(
        healthy: value == 'ok',
        latencyMs: redisLatency,
        message: value == 'ok' ? 'Connected' : 'Value mismatch',
      );
    } catch (e) {
      checks['redis'] = HealthCheck(
        healthy: false,
        message: 'Connection failed: $e',
      );
      overallHealthy = false;
    }

    // Memory check
    final memInfo = ProcessInfo.currentRss;
    final memoryMB = memInfo / 1024 / 1024;
    checks['memory'] = HealthCheck(
      healthy: memoryMB < 512, // Alert if > 512MB
      message: '${memoryMB.toStringAsFixed(1)} MB',
    );

    return HealthStatus(
      healthy: overallHealthy,
      version: '1.0.0',
      environment: session.serverpod.runMode,
      uptime: DateTime.now().difference(_startTime).inSeconds,
      checks: checks,
    );
  }

  static final _startTime = DateTime.now();
}

class HealthStatus {
  final bool healthy;
  final String version;
  final String environment;
  final int uptime;
  final Map<String, HealthCheck> checks;

  HealthStatus({
    required this.healthy,
    required this.version,
    required this.environment,
    required this.uptime,
    required this.checks,
  });

  Map<String, dynamic> toJson() => {
        'healthy': healthy,
        'version': version,
        'environment': environment,
        'uptime': uptime,
        'checks': checks.map((k, v) => MapEntry(k, v.toJson())),
      };
}

class HealthCheck {
  final bool healthy;
  final int? latencyMs;
  final String? message;

  HealthCheck({required this.healthy, this.latencyMs, this.message});

  Map<String, dynamic> toJson() => {
        'healthy': healthy,
        if (latencyMs != null) 'latencyMs': latencyMs,
        if (message != null) 'message': message,
      };
}

---

# ============================================================
# Monitoring Setup with Sentry
# ============================================================
// lib/src/monitoring/sentry_integration.dart
import 'package:sentry/sentry.dart';
import 'package:serverpod/serverpod.dart';

class SentryIntegration {
  static Future<void> init(String dsn, String environment) async {
    await Sentry.init((options) {
      options.dsn = dsn;
      options.environment = environment;
      options.tracesSampleRate = environment == 'production' ? 0.1 : 1.0;
      options.attachStacktrace = true;
      options.sendDefaultPii = false;
      options.beforeSend = (event, hint) {
        // Filter out non-critical errors
        if (event.throwable is SocketException) {
          return null; // Don't send network errors
        }
        return event;
      };
    });
  }

  static void captureException(
    dynamic exception,
    StackTrace? stackTrace, {
    String? userId,
    Map<String, dynamic>? extra,
  }) {
    Sentry.captureException(
      exception,
      stackTrace: stackTrace,
      withScope: (scope) {
        if (userId != null) {
          scope.setUser(SentryUser(id: userId));
        }
        if (extra != null) {
          extra.forEach((key, value) {
            scope.setExtra(key, value);
          });
        }
      },
    );
  }

  static ISentrySpan startTransaction(String name, String operation) {
    return Sentry.startTransaction(name, operation);
  }
}

// Serverpod error handler integration
class MonitoredSession {
  static Future<T> run<T>(
    Session session,
    String operation,
    Future<T> Function() work,
  ) async {
    final transaction = SentryIntegration.startTransaction(
      operation,
      'endpoint',
    );

    try {
      final result = await work();
      transaction.status = SpanStatus.ok();
      return result;
    } catch (e, stackTrace) {
      transaction.status = SpanStatus.internalError();
      SentryIntegration.captureException(
        e,
        stackTrace,
        userId: session.auth?.userId?.toString(),
        extra: {'operation': operation},
      );
      rethrow;
    } finally {
      await transaction.finish();
    }
  }
}

---

# ============================================================
# GitHub Actions Deployment Workflow
# ============================================================
# .github/workflows/deploy-backend.yml
name: Deploy Backend

on:
  push:
    branches: [main]
    paths:
      - 'your_app_server/**'
      - 'Dockerfile'
  workflow_dispatch:
    inputs:
      environment:
        description: 'Deployment environment'
        required: true
        default: 'staging'
        type: choice
        options:
          - staging
          - production

env:
  REGISTRY: ghcr.io
  IMAGE_NAME: ${{ github.repository }}/server

jobs:
  test:
    runs-on: ubuntu-latest
    services:
      postgres:
        image: postgres:15
        env:
          POSTGRES_PASSWORD: testpass
          POSTGRES_DB: test_db
        ports:
          - 5432:5432
        options: >-
          --health-cmd pg_isready
          --health-interval 10s
          --health-timeout 5s
          --health-retries 5

    steps:
      - uses: actions/checkout@v4
      - uses: dart-lang/setup-dart@v1

      - name: Install dependencies
        run: |
          cd your_app_server
          dart pub get

      - name: Run tests
        run: |
          cd your_app_server
          dart test --coverage=coverage

      - name: Upload coverage
        uses: codecov/codecov-action@v4
        with:
          files: your_app_server/coverage/lcov.info

  build-and-push:
    needs: test
    runs-on: ubuntu-latest
    permissions:
      contents: read
      packages: write

    steps:
      - uses: actions/checkout@v4

      - name: Log in to Container Registry
        uses: docker/login-action@v3
        with:
          registry: ${{ env.REGISTRY }}
          username: ${{ github.actor }}
          password: ${{ secrets.GITHUB_TOKEN }}

      - name: Extract metadata
        id: meta
        uses: docker/metadata-action@v5
        with:
          images: ${{ env.REGISTRY }}/${{ env.IMAGE_NAME }}
          tags: |
            type=sha,prefix=
            type=ref,event=branch
            type=raw,value=latest,enable=${{ github.ref == 'refs/heads/main' }}

      - name: Build and push
        uses: docker/build-push-action@v5
        with:
          context: .
          push: true
          tags: ${{ steps.meta.outputs.tags }}
          labels: ${{ steps.meta.outputs.labels }}
          cache-from: type=gha
          cache-to: type=gha,mode=max

  deploy-staging:
    needs: build-and-push
    if: github.ref == 'refs/heads/main' || github.event.inputs.environment == 'staging'
    runs-on: ubuntu-latest
    environment: staging

    steps:
      - name: Deploy to Railway
        run: |
          curl -X POST ${{ secrets.RAILWAY_DEPLOY_WEBHOOK }}

      - name: Wait for deployment
        run: sleep 60

      - name: Health check
        run: |
          response=$(curl -s -o /dev/null -w "%{http_code}" https://staging-api.yourapp.com/health)
          if [ $response != "200" ]; then
            echo "Health check failed with status $response"
            exit 1
          fi
          echo "Health check passed"

  deploy-production:
    needs: deploy-staging
    if: github.event.inputs.environment == 'production'
    runs-on: ubuntu-latest
    environment: production

    steps:
      - name: Deploy to Fly.io
        uses: superfly/flyctl-actions@v1
        with:
          args: "deploy --image ${{ env.REGISTRY }}/${{ env.IMAGE_NAME }}:${{ github.sha }}"
        env:
          FLY_API_TOKEN: ${{ secrets.FLY_API_TOKEN }}

      - name: Run migrations
        uses: superfly/flyctl-actions@v1
        with:
          args: "ssh console -C '/app/server --mode production --apply-migrations --role maintenance'"
        env:
          FLY_API_TOKEN: ${{ secrets.FLY_API_TOKEN }}

      - name: Verify deployment
        run: |
          for i in {1..5}; do
            response=$(curl -s https://api.yourapp.com/health)
            if echo $response | jq -e '.healthy == true' > /dev/null; then
              echo "Deployment verified successfully"
              exit 0
            fi
            echo "Attempt $i failed, retrying in 30s..."
            sleep 30
          done
          echo "Deployment verification failed"
          exit 1
```
