---
type: "THEORY"
title: "Kotlin's Built-in Result<T> Type"
---

**Estimated Time**: 45 minutes
**Difficulty**: Intermediate
**Prerequisites**: Exception handling, generics, functional programming basics

---

Exception handling with try-catch is problematic: it breaks the normal flow of code, makes function signatures lie (they don't show what can fail), and creates invisible control paths that are hard to reason about. Kotlin's `Result<T>` type offers a better way—explicit, functional error handling.

**What is Result<T>?**

`Result<T>` is a sealed class that represents either:
- **Success**: Contains a value of type T
- **Failure**: Contains a Throwable exception

Unlike exceptions that jump up the call stack, `Result` wraps the outcome and keeps execution flowing normally.

**Creating Results**

```kotlin
// Success case
val success: Result<Int> = Result.success(42)

// Failure case
val failure: Result<Int> = Result.failure(IllegalArgumentException("Invalid input"))

// Catching exceptions automatically
val result = runCatching {
    riskyOperation()  // If this throws, Result wraps the exception
}
```

**Working with Results**

`Result` provides functional methods to transform and handle outcomes:

```kotlin
val result = runCatching { fetchUserData() }

// Transform success value
val uppercased = result.map { it.uppercase() }

// Handle both cases
result.fold(
    onSuccess = { data -> println("Got: $data") },
    onFailure = { error -> println("Error: ${error.message}") }
)

// Provide a default on failure
val dataOrDefault = result.getOrDefault("Default Value")

// Recover from specific errors
val recovered = result.recover { 
    if (it is NetworkException) "Offline Mode" else throw it 
}
```

**Why Prefer Result Over Exceptions?**

**Visible in the type**: `fun fetchData(): Result<String>` clearly signals potential failure

**Composable**: Chain multiple operations without nested try-catch blocks

**Functional**: Use map, flatMap, fold—same patterns as collections

**Explicit**: Callers must acknowledge failure; no surprise exceptions

**Real-World Usage**

```kotlin
suspend fun fetchUserProfile(userId: String): Result<UserProfile> = runCatching {
    val response = httpClient.get("/users/$userId")
    response.body<UserProfile>()
}

// In your ViewModel or presenter
val profile = fetchUserProfile("user123")
    .map { it.toDisplayModel() }
    .onSuccess { displayUser(it) }
    .onFailure { showError(it.message ?: "Unknown error") }
```

**Limitations and Arrow**

Kotlin's `Result` is great for simple cases but has limitations: it always carries a `Throwable`, making typed errors impossible. For production systems needing precise error handling, the Arrow library's `Either` type (explored in the next lesson) provides more power and flexibility.

**Real-World Relevance**

`Result` is ideal for:
- API calls in Android ViewModels
- File I/O operations
- Parsing operations that might fail
- Any operation where you want explicit error handling without the ceremony of try-catch
