---
type: "THEORY"
title: "Production Monitoring and Health Checks"
---

Production applications need monitoring to detect and diagnose issues before users notice them.

Spring Boot Actuator:
Actuator provides production-ready endpoints for monitoring and management.

```gradle
implementation 'org.springframework.boot:spring-boot-starter-actuator'
```

Configure actuator endpoints:
```properties
# application.properties
management.endpoints.web.exposure.include=health,info,metrics,prometheus
management.endpoint.health.show-details=when-authorized
management.health.db.enabled=true
management.health.diskspace.enabled=true

# Info endpoint content
info.app.name=Task Manager API
info.app.version=@project.version@
info.app.description=Full-stack task management application
```

Custom Health Indicators:
```java
@Component
public class DatabaseHealthIndicator implements HealthIndicator {

    private final JdbcTemplate jdbcTemplate;

    public DatabaseHealthIndicator(JdbcTemplate jdbcTemplate) {
        this.jdbcTemplate = jdbcTemplate;
    }

    @Override
    public Health health() {
        try {
            jdbcTemplate.queryForObject("SELECT 1", Integer.class);
            return Health.up()
                .withDetail("database", "PostgreSQL")
                .withDetail("status", "Connected")
                .build();
        } catch (Exception e) {
            return Health.down()
                .withDetail("error", e.getMessage())
                .build();
        }
    }
}
```

Structured Logging:
```java
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

@Service
public class TaskService {
    private static final Logger log = LoggerFactory.getLogger(TaskService.class);

    public TaskResponse createTask(TaskRequest request, User user) {
        log.info("Creating task for user: {} with title: {}", 
            user.getEmail(), request.getTitle());
        
        try {
            Task task = // ... save task
            log.info("Task created successfully: id={}", task.getId());
            return mapToResponse(task);
        } catch (Exception e) {
            log.error("Failed to create task for user: {}", user.getEmail(), e);
            throw e;
        }
    }
}
```

JSON Logging for Production (logback-spring.xml):
```xml
<?xml version="1.0" encoding="UTF-8"?>
<configuration>
    <springProfile name="prod">
        <appender name="JSON" class="ch.qos.logback.core.ConsoleAppender">
            <encoder class="net.logstash.logback.encoder.LogstashEncoder">
                <includeMdcKeyName>userId</includeMdcKeyName>
                <includeMdcKeyName>requestId</includeMdcKeyName>
            </encoder>
        </appender>
        <root level="INFO">
            <appender-ref ref="JSON"/>
        </root>
    </springProfile>
</configuration>
```

Request Tracing:
```java
@Component
public class RequestLoggingFilter extends OncePerRequestFilter {

    @Override
    protected void doFilterInternal(HttpServletRequest request,
                                    HttpServletResponse response,
                                    FilterChain chain) throws ServletException, IOException {
        String requestId = UUID.randomUUID().toString().substring(0, 8);
        MDC.put("requestId", requestId);
        response.setHeader("X-Request-Id", requestId);

        long start = System.currentTimeMillis();
        try {
            chain.doFilter(request, response);
        } finally {
            long duration = System.currentTimeMillis() - start;
            log.info("Request completed: {} {} - {} ({}ms)",
                request.getMethod(), request.getRequestURI(),
                response.getStatus(), duration);
            MDC.clear();
        }
    }
}
```

Monitoring Checklist:
- Health endpoint returns UP
- Database connections healthy
- Response times under 200ms (p95)
- Error rate under 1%
- Memory usage stable (no leaks)
- Disk space adequate

## Pre-Deployment Security Checklist

- [ ] Change default database passwords
- [ ] Set strong JWT secret (256+ bit random string)
- [ ] Configure CORS for production domain only
- [ ] Enable HTTPS (never deploy with HTTP)
- [ ] Review environment variables (no secrets in code)
- [ ] Enable Spring Boot actuator security
- [ ] Set up application logging