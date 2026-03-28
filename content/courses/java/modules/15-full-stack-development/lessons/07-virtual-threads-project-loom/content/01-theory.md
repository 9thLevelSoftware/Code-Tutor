---
type: "THEORY"
title: "The Problem: Thread-Per-Request is Expensive"
---

Traditional Java web servers use one OS thread per HTTP request:

Request 1 → Thread 1
Request 2 → Thread 2
Request 3 → Thread 3
...
Request 1000 → Thread 1000

PROBLEM:
- Each OS thread costs ~1MB of memory
- 1000 threads = 1GB of RAM just for threads!
- Context switching between threads is expensive
- Most threads spend time WAITING (for database, network)

Traditional server: ~200-500 concurrent connections MAX

This is why reactive programming (WebFlux) became popular:
- Don't block threads!
- But... callback hell, hard to debug, complex code

What if we could have MILLIONS of threads that are CHEAP?