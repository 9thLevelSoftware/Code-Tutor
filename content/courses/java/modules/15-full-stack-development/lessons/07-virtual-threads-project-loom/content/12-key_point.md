---
type: "KEY_POINT"
title: "TL;DR - Virtual Threads Summary"
---

WHAT: Lightweight threads managed by JVM (not OS)
WHY: Handle millions of concurrent connections
WHEN: Standard since Java 21, mature in Java 25. Spring Boot 4.0 enables them by default.

Spring Boot 4.0.x: Virtual threads enabled by default -- no configuration needed!
Every @RestController handler runs on a virtual thread automatically.

Create manually:
Thread.startVirtualThread(() -> doWork());
Executors.newVirtualThreadPerTaskExecutor();

BEST FOR:
✓ Web servers (HTTP requests)
✓ Microservices
✓ Database-heavy applications
✓ API gateways
✓ Any I/O-bound workload

AVOID:
✗ CPU-intensive math/processing
✗ synchronized blocks (use ReentrantLock)
✗ Pooling virtual threads (create new ones)

IMPACT:
- 5-10x throughput improvement
- Simpler code than reactive programming
- Lower memory usage
- Better latency under load

This is the BIGGEST change to Java concurrency since threads were introduced!