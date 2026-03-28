---
type: "THEORY"
title: "Multi-Stage Dockerfile for Spring Boot"
---

BOTH PATHS

A multi-stage Dockerfile separates the build environment from the runtime environment, resulting in smaller, more secure production images. This Dockerfile is the same whether you chose Thymeleaf or React -- both paths share the same Spring Boot backend.

If you chose the **Thymeleaf path**, this single Dockerfile is ALL you need. Your templates and static assets are bundled inside the Spring Boot JAR, so one container serves everything. This is one of the big advantages of the Thymeleaf path -- your entire app is one JAR file.

If you chose the **React path**, you also need a separate frontend Dockerfile (shown in the next section).

Why Multi-Stage Builds?
- Build stage includes JDK, Gradle/Maven, source code (large)
- Runtime stage only includes JRE and the JAR (small)
- Final image is 200-300MB instead of 800MB+
- Reduced attack surface in production

```dockerfile
# Dockerfile (in project root)

# Stage 1: Build the application
FROM gradle:8.12-jdk25 AS builder

WORKDIR /app

# Copy only dependency files first (for layer caching)
COPY build.gradle settings.gradle ./
COPY gradle ./gradle

# Download dependencies (cached if build.gradle unchanged)
RUN gradle dependencies --no-daemon

# Copy source code
COPY src ./src

# Build the application
RUN gradle build -x test --no-daemon

# Stage 2: Create the runtime image
FROM eclipse-temurin:25-jre-alpine

WORKDIR /app

# Create non-root user for security
RUN addgroup -S spring && adduser -S spring -G spring

# Copy the JAR from builder stage
COPY --from=builder /app/build/libs/*.jar app.jar

# Set ownership to non-root user
RUN chown spring:spring app.jar
USER spring

# Expose port
EXPOSE 8080

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=30s \
  CMD curl -f http://localhost:8080/actuator/health || exit 1

# Run the application
ENTRYPOINT ["java", "-jar", "app.jar"]
```

For Maven projects, replace the builder stage:
```dockerfile
FROM maven:3.9-eclipse-temurin-25 AS builder
WORKDIR /app
COPY pom.xml .
COPY mvnw .mvnw/
COPY .mvn .mvn
RUN chmod +x mvnw
RUN mvn dependency:go-offline
COPY src ./src
RUN ./mvnw clean package -DskipTests
```

Build and run locally:
```bash
# Build the image
docker build -t taskmanager:latest .

# Run with environment variables
docker run -p 8080:8080 \
  -e SPRING_DATASOURCE_URL=jdbc:postgresql://host.docker.internal:5432/taskmanager \
  -e SPRING_DATASOURCE_USERNAME=postgres \
  -e SPRING_DATASOURCE_PASSWORD=password \
  -e JWT_SECRET=your-secret-key \
  taskmanager:latest
```

The alpine base image is just 50MB. Combined with JRE-only (no JDK), your final image stays lean.