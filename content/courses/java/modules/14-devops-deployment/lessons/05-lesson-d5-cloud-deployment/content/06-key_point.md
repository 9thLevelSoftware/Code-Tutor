---
type: "KEY_POINT"
title: "Health Checks and Monitoring"
---

Production applications need health monitoring:

SPRING BOOT ACTUATOR:

<dependency>
    <groupId>org.springframework.boot</groupId>
    <artifactId>spring-boot-starter-actuator</artifactId>
</dependency>

ENDPOINTS:

GET /actuator/health
{
  "status": "UP",
  "components": {
    "db": { "status": "UP" },
    "diskSpace": { "status": "UP" }
  }
}

CONFIGURATION:

# application.properties
management.endpoints.web.exposure.include=health,info,metrics
management.endpoint.health.show-details=when-authorized

HEALTH CHECK TYPES:
- Liveness: Is the app running? (/actuator/health/liveness)
- Readiness: Is the app ready for traffic? (/actuator/health/readiness)

FLY.IO HEALTH CHECKS:

Fly.io checks /actuator/health automatically.
If it returns non-200, Fly.io:
1. Marks the deploy as failed
2. Rolls back to previous version
3. Sends notification

CUSTOM HEALTH INDICATOR:

@Component
public class ExternalApiHealthIndicator implements HealthIndicator {
    @Override
    public Health health() {
        boolean apiReachable = checkExternalApi();
        if (apiReachable) {
            return Health.up().withDetail("api", "reachable").build();
        }
        return Health.down().withDetail("api", "unreachable").build();
    }
}