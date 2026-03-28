---
type: "KEY_POINT"
title: "Virtual Threads are Like Lightweight Waiters"
---

TRADITIONAL THREADS (Platform Threads):
= Restaurant with 10 waiters
- Each waiter handles ONE table at a time
- Customer orders food → waiter WAITS at kitchen door
- While waiting, waiter can't serve other tables
- 10 waiters = 10 tables maximum at once

VIRTUAL THREADS:
= Restaurant with VIRTUAL waiters
- Waiter takes order, goes to kitchen, PUTS DOWN NOTEPAD
- While kitchen cooks, waiter serves OTHER tables
- When food ready, ANY waiter picks up notepad and delivers
- 10 real waiters can serve 1000 tables!

Java 25 Virtual Threads (standard since Java 21, mature in 25):
- Managed by JVM, not OS
- Cost: ~1KB each (vs 1MB for platform threads)
- Can have MILLIONS of them
- Write normal blocking code - JVM handles scheduling