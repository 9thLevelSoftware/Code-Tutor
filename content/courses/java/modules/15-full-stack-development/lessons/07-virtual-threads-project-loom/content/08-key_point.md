---
type: "KEY_POINT"
title: "⚠️ Virtual Thread Pinning - The One Gotcha"
---

PINNING = Virtual thread gets "stuck" to platform thread

This happens with:
1. synchronized blocks/methods
2. Native code (JNI)

Example of BAD code:

synchronized (lock) {
    // Virtual thread is PINNED here
    database.query();  // Can't yield to other virtual threads!
}

FIX - Use ReentrantLock instead:

private final ReentrantLock lock = new ReentrantLock();

lock.lock();
try {
    database.query();  // Virtual thread CAN yield here!
} finally {
    lock.unlock();
}

DETECT PINNING:
# JVM flag to warn about pinning
-Djdk.tracePinnedThreads=full

CHECKLIST FOR VIRTUAL THREAD READINESS:
✓ Replace synchronized with ReentrantLock
✓ Avoid ThreadLocal (use ScopedValue instead - stable in Java 25)
✓ Don't pool virtual threads (create fresh ones)
✓ Update JDBC drivers to latest versions
✓ Check third-party libraries for synchronized usage