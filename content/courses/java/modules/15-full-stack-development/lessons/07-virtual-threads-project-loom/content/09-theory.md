---
type: "THEORY"
title: "Performance Comparison: Platform vs Virtual Threads"
---

Benchmark: 10,000 concurrent HTTP requests to REST API

PLATFORM THREADS (200 thread pool):
- Throughput: ~1,800 req/sec
- Latency P99: 5,200ms
- Memory: 800MB
- Thread contention: HIGH

VIRTUAL THREADS:
- Throughput: ~9,500 req/sec (5x improvement!)
- Latency P99: 120ms
- Memory: 350MB
- Thread contention: MINIMAL

WHY SO MUCH FASTER?
1. No thread pool bottleneck
2. Near-zero context switch cost
3. Better CPU utilization (threads don't sit idle)
4. Reduced memory pressure

BUT REMEMBER:
- Virtual threads don't make SINGLE request faster
- They make MANY concurrent requests faster
- CPU-bound code won't benefit
- I/O-bound code (web servers) benefits MASSIVELY