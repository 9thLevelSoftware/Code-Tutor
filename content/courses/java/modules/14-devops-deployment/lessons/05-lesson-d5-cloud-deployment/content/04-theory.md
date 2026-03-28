---
type: "THEORY"
title: "Environment Configuration"
---

Production applications need different configuration:

12-FACTOR APP PRINCIPLE:
Configuration should come from ENVIRONMENT VARIABLES,
not hardcoded in files.

SPRING BOOT READS ENV VARS AUTOMATICALLY:

Environment variable:          Property:
SPRING_DATASOURCE_URL    ->   spring.datasource.url
SPRING_PROFILES_ACTIVE   ->   spring.profiles.active
JWT_SECRET               ->   jwt.secret (custom)

EXAMPLE CONFIGURATION:

# application.properties (defaults for local dev)
spring.datasource.url=jdbc:postgresql://localhost:5432/myapp
spring.datasource.username=postgres
spring.datasource.password=postgres

# In Fly.io, set environment variables:
DATABASE_URL=postgresql://user:pass@host:5432/db
# Spring Boot reads this automatically!

# For custom properties:
@Value("${jwt.secret}")
private String jwtSecret;

# Set in Fly.io:
JWT_SECRET=super-secret-production-key

ENVIRONMENT-SPECIFIC PROFILES:

# application-production.properties
spring.jpa.show-sql=false
logging.level.root=WARN

# In Fly.io:
SPRING_PROFILES_ACTIVE=production