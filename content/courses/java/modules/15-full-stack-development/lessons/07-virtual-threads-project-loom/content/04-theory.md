---
type: "THEORY"
title: "Virtual Threads with ExecutorService"
---

The most practical way to use Virtual Threads:

try (var executor = Executors.newVirtualThreadPerTaskExecutor()) {
    // Submit 100,000 tasks - each gets its own virtual thread!
    List<Future<String>> futures = new ArrayList<>();
    
    for (int i = 0; i < 100_000; i++) {
        final int taskId = i;
        futures.add(executor.submit(() -> {
            // Simulate network call (blocks, but that's OK!)
            Thread.sleep(Duration.ofMillis(100));
            return "Result from task " + taskId;
        }));
    }
    
    // Collect results
    for (Future<String> future : futures) {
        String result = future.get();  // Blocks until complete
        IO.println(result);
    }
}

// With platform threads: This would need 100,000 MB of RAM!
// With virtual threads: Runs with minimal memory overhead

PRODUCTION PATTERN:
@Bean
public ExecutorService virtualThreadExecutor() {
    return Executors.newVirtualThreadPerTaskExecutor();
}