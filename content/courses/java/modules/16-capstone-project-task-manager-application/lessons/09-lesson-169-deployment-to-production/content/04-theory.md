---
type: "THEORY"
title: "Environment Variables and Secrets Management"
---

Never commit secrets to version control. Use environment variables and secret management for all sensitive configuration.

Development vs Production Configuration:

```properties
# src/main/resources/application.properties
# Base configuration - no secrets here!
spring.application.name=taskmanager
server.port=8080

spring.jpa.hibernate.ddl-auto=validate
spring.jpa.show-sql=false

# These come from environment variables
spring.datasource.url=${DATABASE_URL}
spring.datasource.username=${DATABASE_USERNAME}
spring.datasource.password=${DATABASE_PASSWORD}

jwt.secret=${JWT_SECRET}
jwt.expiration=${JWT_EXPIRATION:86400000}

cors.allowed-origins=${CORS_ALLOWED_ORIGINS:http://localhost:5173}
```

Local Development (.env file - never commit!):
```bash
# .env - add to .gitignore!
DATABASE_URL=jdbc:postgresql://localhost:5432/taskmanager
DATABASE_USERNAME=postgres
DATABASE_PASSWORD=localdev123
JWT_SECRET=local-dev-secret-key-32-chars-min
JWT_EXPIRATION=86400000
CORS_ALLOWED_ORIGINS=http://localhost:5173
```

.gitignore entries:
```gitignore
# Environment files
.env
.env.local
.env.*.local
application-local.properties

# Secrets
*.pem
*.key
secrets/
```

Secure Secret Storage in Production:

1. Fly.io/Heroku: Add in dashboard under Environment Variables

2. AWS Secrets Manager:
```java
@Configuration
public class SecretsConfig {
    @Bean
    public String jwtSecret(SecretsManagerClient client) {
        GetSecretValueRequest request = GetSecretValueRequest.builder()
            .secretId("taskmanager/jwt-secret")
            .build();
        return client.getSecretValue(request).secretString();
    }
}
```

3. Kubernetes Secrets:
```yaml
apiVersion: v1
kind: Secret
metadata:
  name: taskmanager-secrets
type: Opaque
stringData:
  jwt-secret: your-production-secret
  db-password: secure-database-password
```

Secret Rotation Strategy:
- JWT secrets: Rotate every 30-90 days
- Database passwords: Rotate quarterly
- API keys: Rotate immediately if exposed

Always assume secrets can be exposed. Use short-lived tokens where possible, and implement secret rotation without downtime.