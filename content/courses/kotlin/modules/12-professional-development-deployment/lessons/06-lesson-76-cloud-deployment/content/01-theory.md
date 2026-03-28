---
type: "THEORY"
title: "Introduction"
---

**Estimated Time**: 45 minutes

Deploying KMP applications to the cloud requires platform-specific strategies. Whether you're deploying a Ktor backend, hosting a web app, or setting up backend services, understanding cloud deployment is essential.

**Ktor Backend Deployment**

Deploy your Kotlin backend to popular cloud providers:

```kotlin
// Application configuration for production
fun Application.module() {
    // Environment-based configuration
    val isProd = environment.config.property("ktor.environment").getString() == "prod"
    
    install(DefaultHeaders)
    install(CallLogging) {
        level = if (isProd) Level.INFO else Level.DEBUG
    }
    
    routing {
        // Health check for load balancers
        get("/health") {
            call.respond(mapOf("status" to "healthy", "version" to "1.0.0"))
        }
    }
}
```

**Docker Deployment**

```dockerfile
# Dockerfile
FROM openjdk:21-jdk-slim

WORKDIR /app

# Copy and build
COPY build/libs/*.jar app.jar

EXPOSE 8080

# Production JVM flags
ENTRYPOINT ["java", "-Xmx512m", "-XX:+UseContainerSupport", "-jar", "app.jar"]
```

**Platform-Specific Deployment**

| Platform | Service | Best For |
|----------|---------|----------|
| AWS | Elastic Beanstalk, ECS | Enterprise, complex needs |
| Google Cloud | App Engine, Cloud Run | Simplicity, auto-scaling |
| Azure | Container Apps, AKS | Microsoft ecosystem |
| Railway, Fly.io | Managed containers | Small-medium projects |
| Heroku | Platform | Quick prototyping |

**Environment Configuration**

```yaml
# docker-compose.yml for self-hosting
version: '3.8'
services:
  app:
    build: .
    ports:
      - "8080:8080"
    environment:
      - DATABASE_URL=${DATABASE_URL}
      - JWT_SECRET=${JWT_SECRET}
    depends_on:
      - db
  
  db:
    image: postgres:15
    environment:
      POSTGRES_DB: appdb
      POSTGRES_USER: app
      POSTGRES_PASSWORD: ${DB_PASSWORD}
```

**Compose Web Deployment**

```kotlin
// build.gradle.kts (web module)
kotlin {
    js {
        browser {
            webpackTask {
                output.libraryTarget = "umd"
            }
        }
        binaries.executable()
    }
}

// Deploy to static hosting: Netlify, Vercel, GitHub Pages
```

**Monitoring and Logging**

```kotlin
// Integrate with monitoring services
install(CallLogging) {
    filter { call ->
        call.request.path().startsWith("/api")
    }
    mdc("traceId") { call.request.headers["X-Trace-ID"] }
}

// Health endpoint with dependency checks
get("/ready") {
    val dbHealthy = checkDatabase()
    val cacheHealthy = checkCache()
    
    val status = if (dbHealthy && cacheHealthy) HttpStatusCode.OK else HttpStatusCode.ServiceUnavailable
    call.respond(status, mapOf(
        "database" to dbHealthy,
        "cache" to cacheHealthy
    ))
}
```

**Best Practices**

1. Use environment variables for configuration, never hardcode secrets
2. Implement health checks for container orchestration
3. Configure proper JVM heap sizes for containers
4. Use multi-stage Docker builds to minimize image size
5. Set up log aggregation (CloudWatch, DataDog, etc.)
6. Enable HTTPS/TLS termination at the load balancer

Cloud deployment transforms your local development project into a globally accessible service.
