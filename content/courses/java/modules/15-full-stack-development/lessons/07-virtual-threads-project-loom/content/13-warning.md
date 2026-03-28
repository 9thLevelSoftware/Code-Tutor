---
type: "WARNING"
title: "Virtual Thread Adoption Considerations"
---

PINNING IS STILL A CONCERN:
Virtual threads can still be pinned by synchronized blocks.
Fix: Replace synchronized with ReentrantLock for blocking operations.

DON'T POOL VIRTUAL THREADS:
Creating a fixed pool defeats the purpose of virtual threads.
Fix: Use Executors.newVirtualThreadPerTaskExecutor() - create per task.

THREAD LOCALS HAVE OVERHEAD:
Virtual threads copy ThreadLocal values at creation time.
Fix: Use ScopedValues (stable in Java 25) for request-scoped data.

UPDATE YOUR LIBRARIES:
Older JDBC drivers and libraries may not be virtual-thread friendly.
Fix: Use latest versions of PostgreSQL driver (42.7+), HikariCP (5.1+).

MONITOR CARRIER THREAD USAGE:
If all carrier threads are pinned, your app will stall.
Fix: Use -Djdk.tracePinnedThreads=full to detect pinning issues.