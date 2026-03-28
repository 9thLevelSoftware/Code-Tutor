---
type: "WARNING"
title: "Context Parameters Beta Stability"
---

**Context parameters are Beta—APIs may change** before stabilization. Code using `-Xcontext-parameters` may break in future Kotlin versions as the feature evolves.

**Context parameters are experimental in Kotlin 2.3.4** and require the `-Xcontext-parameters` compiler flag. In Kotlin 2.2 they were Beta, and in 2.3.x they remain experimental pending further feedback.

**Don't use Beta features in production libraries** you distribute publicly. Your users would need to enable experimental flags, and your API might break with Kotlin updates, forcing them to update immediately.

**Beta features are acceptable in application code** where you control the entire codebase and can update atomically. The risk is isolated to your team, not external users.

**Context parameter naming conflicts**:
```kotlin
context(logger: Logger)
fun process() {
    log("message")  // Which log? Context Logger or global function?
}
```

If multiple contexts provide similar names, ambiguity arises. Use explicit context parameters: `logger.log("message")`.

**Overuse creates implicit dependencies**:
```kotlin
context(logger: Logger, database: Database, cache: Cache, analytics: Analytics, config: Config)
fun complex() { ... }
```

Too many contexts make it unclear what a function depends on. Consider explicit parameters or dependency injection for better clarity.

**Testing difficulties**—tests need to provide all contexts. This can be verbose:
```kotlin
with(mockLogger) {
    with(mockDatabase) {
        with(mockCache) {
            testFunction()  // Deep nesting
        }
    }
}
```

Use context parameters judiciously for cross-cutting concerns, not every dependency.
