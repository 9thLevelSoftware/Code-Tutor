---
type: "THEORY"
title: "Migration: Context Receivers to Context Parameters"
---


### Migrating from Context Receivers

If you have code using the deprecated context receivers syntax, here is how to migrate:

**1. Syntax Change**
```kotlin
// OLD (deprecated context receivers, -Xcontext-receivers)
context(Logger)
fun doWork() {
    info("working")  // Implicit member access
}

// NEW (context parameters, -Xcontext-parameters)
context(logger: Logger)
fun doWork() {
    logger.info("working")  // Explicit qualified access
}
```

**2. Compiler Flag**
```kotlin
// build.gradle.kts -- replace the old flag
kotlin {
    compilerOptions {
        // Remove: freeCompilerArgs.add("-Xcontext-receivers")
        freeCompilerArgs.add("-Xcontext-parameters")
    }
}
```

**3. Key Differences**
- Context parameters are **named**: `context(name: Type)` instead of `context(Type)`
- Access is **explicit**: `name.member()` instead of implicit `member()`
- Multiple contexts: `context(a: TypeA, b: TypeB)` -- no ambiguity
- **Kotlin 2.3.4 Status**: Context parameters are experimental. Enable with `-Xcontext-parameters`
- Historical note: context receivers were experimental in Kotlin 1.6.20-1.9.x, deprecated in 2.0.20

---

