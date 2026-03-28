---
type: "THEORY"
title: "Migration Checklist: Adopting Virtual Threads"
---

STEP 1: Use Java 25 and Spring Boot 4.0.x
<java.version>25</java.version>
<parent>
    <groupId>org.springframework.boot</groupId>
    <artifactId>spring-boot-starter-parent</artifactId>
    <version>4.0.0</version>
</parent>

STEP 2: Virtual Threads -- Enabled by Default
Spring Boot 4.0 enables virtual threads by default.
No configuration needed! Every request handler uses virtual threads automatically.

STEP 3: Update Dependencies
- JDBC drivers (PostgreSQL 42.7+, MySQL 8.3+)
- HikariCP (5.1+)
- Hibernate (6.4+)

STEP 4: Check for Pinning
# Run with tracing
java -Djdk.tracePinnedThreads=full -jar app.jar

STEP 5: Replace synchronized
// Before
synchronized (this) { ... }

// After
private final ReentrantLock lock = new ReentrantLock();
lock.lock();
try { ... } finally { lock.unlock(); }

STEP 6: Load Test
- Use JMeter or Gatling
- Simulate 10,000 concurrent users
- Compare metrics before/after

STEP 7: Monitor in Production
- Watch thread metrics in Actuator
- Monitor memory usage
- Check response times