---
type: "WARNING"
title: "Production Checklist"
---

Before deploying to production, verify:

SECURITY:
[ ] No secrets in code or git history
[ ] HTTPS only (Fly.io handles this automatically)
[ ] Secure password/JWT generation
[ ] CORS configured correctly
[ ] Rate limiting if needed

DATABASE:
[ ] Connection pooling configured
[ ] Migrations work (Flyway/Liquibase)
[ ] Backups configured (Fly.io PostgreSQL includes backups)
[ ] DDL-auto set to 'validate' or 'none' in production

PERFORMANCE:
[ ] Logging level set to WARN/ERROR
[ ] Debug endpoints disabled
[ ] Caching configured if needed

MONITORING:
[ ] Health endpoints enabled
[ ] Error tracking (Sentry, etc.)
[ ] Logging aggregation

DEPLOYMENT:
[ ] Zero-downtime deployments
[ ] Rollback plan
[ ] Environment variables set

COMMON PRODUCTION SETTINGS:

# application-production.properties
spring.jpa.hibernate.ddl-auto=validate
spring.jpa.show-sql=false
logging.level.root=WARN
logging.level.com.yourcompany=INFO
server.error.include-stacktrace=never
server.error.include-message=never