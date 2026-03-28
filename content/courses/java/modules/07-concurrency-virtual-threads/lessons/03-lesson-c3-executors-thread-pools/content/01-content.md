---
type: "THEORY"
title: "Executors and Thread Pools in Java"
---

**Estimated Time**: 60 minutes

**Learning Objectives**: By the end of this lesson, you will understand Java's Executor framework, how to use thread pools effectively, and best practices for managing concurrent execution.

---

## Introduction to Executors

Creating a new thread for every concurrent task is expensive and inefficient. The `java.util.concurrent.Executor` framework provides a higher-level API for managing thread execution through thread pools—reusable collections of worker threads.

**Real-World Relevance**: Web servers, database connection pools, and batch processing systems all rely on thread pools to handle thousands of concurrent operations efficiently. Understanding Executors is crucial for building scalable Java applications.

## Thread Pool Types

### Fixed Thread Pool
```java
// Creates a pool with exactly 4 threads
ExecutorService executor = Executors.newFixedThreadPool(4);

// Submit tasks
executor.submit(() -> {
    System.out.println("Task running on " + Thread.currentThread().getName());
});
```

Use when you know exactly how many threads you need, such as CPU-bound tasks where the pool size matches available processors.

### Cached Thread Pool
```java
// Creates threads as needed, reuses idle threads
ExecutorService executor = Executors.newCachedThreadPool();
```

Ideal for many short-lived asynchronous tasks. Threads are created on demand and cached for reuse.

### Single Thread Executor
```java
// Sequential execution on a single thread
ExecutorService executor = Executors.newSingleThreadExecutor();
```

Guarantees tasks execute sequentially, useful for maintaining order or accessing non-thread-safe resources.

### Scheduled Thread Pool
```java
// For delayed or periodic execution
ScheduledExecutorService scheduler = Executors.newScheduledThreadPool(2);

scheduler.schedule(() -> System.out.println("Delayed task"), 5, TimeUnit.SECONDS);
scheduler.scheduleAtFixedRate(() -> System.out.println("Periodic task"), 0, 10, TimeUnit.SECONDS);
```

## Best Practices

### Always Shutdown Executors
```java
executor.shutdown(); // Graceful shutdown
try {
    if (!executor.awaitTermination(60, TimeUnit.SECONDS)) {
        executor.shutdownNow(); // Force shutdown
    }
} catch (InterruptedException e) {
    executor.shutdownNow();
}
```

### Handle Exceptions
```java
Future<?> future = executor.submit(() -> {
    // Task that might throw
    return result;
});

try {
    future.get();
} catch (ExecutionException e) {
    // Handle task exception
    e.getCause().printStackTrace();
}
```

### Use Appropriate Pool Sizes
- CPU-bound tasks: pool size = number of CPU cores
- IO-bound tasks: larger pools (50-200) depending on latency
- Mixed workloads: profile and adjust based on metrics

## Modern Alternative: Virtual Threads (Java 21+)

```java
// Virtual threads - lightweight alternative to platform threads
try (var executor = Executors.newVirtualThreadPerTaskExecutor()) {
    IntStream.range(0, 10_000).forEach(i ->
        executor.submit(() -> {
            Thread.sleep(Duration.ofSeconds(1));
            return i;
        })
    );
}
```

Virtual threads can handle millions of concurrent tasks with minimal overhead.

The Executor framework remains the foundation for concurrent programming in Java, providing the abstraction needed to build efficient, scalable applications.