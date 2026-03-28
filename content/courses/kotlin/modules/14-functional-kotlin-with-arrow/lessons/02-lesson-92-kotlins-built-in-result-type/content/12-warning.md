---
type: "WARNING"
title: "Result Type Limitations"
---


### Cannot Use as Return Type Directly

```kotlin
// This is discouraged by Kotlin
fun getValue(): Result<Int> = Result.success(42)  // Works but warned

// Prefer inline functions or suspend functions
suspend fun getValue(): Result<Int> = runCatching { 
    fetchValue() 
}
```

### Single Error Type

Result only holds `Throwable`, limiting typed error handling:

```kotlin
// You lose error type information
fun parse(s: String): Result<Int>  // What kind of error?

// For typed errors, use Arrow's Either (next lesson)
fun parse(s: String): Either<ParseError, Int>  // Error type is explicit!
```

### No Accumulation

Can't collect multiple errors:

```kotlin
// First error stops everything
val results = listOf("1", "a", "2", "b")
    .map { parseNumber(it) }
// Only know about first failure, not all
```

### Stack Trace Overhead

`runCatching` captures full stack traces even for expected errors:

```kotlin
// SLOW for expected errors - captures stack trace
val result = runCatching { parseInput(userInput) }

// BETTER for control flow - use Either or nullable returns
val result = parseInput(userInput)
    .mapLeft { ParseError(it) }
```

---

