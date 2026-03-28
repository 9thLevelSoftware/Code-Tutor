---
type: "EXAMPLE"
title: "Deploying to Fly.io"
---

Step-by-step deployment of your Spring Boot application:

```bash
# Step 1: Prepare your application

# Ensure you have a Dockerfile (Fly.io will use it)
# Or use fly deploy for simple deployments

# Add health endpoint for Fly.io
# In pom.xml, add Spring Boot Actuator:
<dependency>
    <groupId>org.springframework.boot</groupId>
    <artifactId>spring-boot-starter-actuator</artifactId>
</dependency>

# In application.properties:
management.endpoints.web.exposure.include=health
management.endpoint.health.show-details=always

# Step 2: Configure for production

# application-production.properties
spring.datasource.url=${DATABASE_URL}
server.port=8080
spring.jpa.hibernate.ddl-auto=update

# Step 3: Install Fly.io CLI and deploy

# Install the Fly.io CLI (flyctl)
# See: https://fly.io/docs/hands-on/install-flyctl/

# Login to Fly.io
fly auth login

# Launch your app (creates fly.toml config)
fly launch --no-deploy

# Create a PostgreSQL database
fly postgres create --name myapp-db

# Attach the database to your app (sets DATABASE_URL automatically)
fly postgres attach myapp-db

# Set environment variables
fly secrets set SPRING_PROFILES_ACTIVE=production
fly secrets set JWT_SECRET=your-production-secret

# Deploy your application
fly deploy

# Step 4: View your deployed app
fly open
```

**Note:** Railway discontinued their free tier in 2024. This lesson now uses Fly.io which offers a generous free tier with 256MB RAM and 3GB transfer per month.
