---
type: "THEORY"
title: "Creating Virtual Threads in Java 25"
---

BEFORE Virtual Threads - Platform Thread:

Thread platformThread = new Thread(() -> {
    IO.println("Running on platform thread");
});
platformThread.start();

// Or with ExecutorService:
ExecutorService executor = Executors.newFixedThreadPool(10);
executor.submit(() -> doWork());

Virtual Thread (standard since Java 21, mature in Java 25):

// Method 1: Thread.startVirtualThread()
Thread virtualThread = Thread.startVirtualThread(() -> {
    IO.println("Running on virtual thread!");
});

// Method 2: Thread.ofVirtual().start()
Thread vt = Thread.ofVirtual()
    .name("my-virtual-thread")
    .start(() -> doWork());

// Method 3: Virtual Thread Executor (RECOMMENDED)
ExecutorService executor = Executors.newVirtualThreadPerTaskExecutor();
executor.submit(() -> doWork());

CHECK IF VIRTUAL:
Thread.currentThread().isVirtual();  // true or false