---
type: "THEORY"
title: "Arrow Core: Either, Option, and Validated"
---

**Estimated Time**: 55 minutes
**Difficulty**: Advanced
**Prerequisites**: Kotlin Result type, generics, functional programming

---

The Arrow library brings advanced functional programming to Kotlin. Its core module provides three fundamental types that elevate your error handling and data modeling: `Either`, `Option`, and `Validated`.

**Either<L, R>: Explicit Error Channels**

Unlike Kotlin's `Result` which always carries a `Throwable`, `Either` lets you define your error types explicitly:

```kotlin
sealed class UserError {
    object NotFound : UserError()
    data class InvalidId(val reason: String) : UserError()
    object NetworkError : UserError()
}

fun fetchUser(id: String): Either<UserError, User> =
    if (id.isBlank()) Either.Left(UserError.InvalidId("ID cannot be blank"))
    else if (userNotInDatabase(id)) Either.Left(UserError.NotFound)
    else Either.Right(User(id, "Alice"))
```

By convention, `Left` holds the error, `Right` holds the success. This makes your function signatures honest about what can go wrong.

**Option<T>: Handling Absence Without Null**

Null is the billion-dollar mistake. `Option` provides a type-safe alternative:

```kotlin
val someValue: Option<Int> = Some(42)
val noValue: Option<Int> = None

// No risk of NullPointerException
val result = someValue.getOrElse { 0 }

// Transform with map
val doubled = someValue.map { it * 2 }  // Some(84)
val emptyDoubled = None.map { it * 2 }  // None
```

**Validated<E, A>: Accumulating Multiple Errors**

Sometimes you want to report all validation failures at once, not stop at the first error. `Validated` accumulates errors:

```kotlin
data class UserRegistration(val email: String, val password: String, val age: Int)

fun validateEmail(email: String): ValidatedNel<String, String> =
    if (email.contains("@")) email.validNel()
    else "Invalid email".invalidNel()

fun validatePassword(password: String): ValidatedNel<String, String> =
    if (password.length >= 8) password.validNel()
    else "Password too short".invalidNel()

// Combine validations - reports ALL errors, not just the first
val result = validateEmail(input.email)
    .zip(validatePassword(input.password)) { e, p -> e to p }
// Either valid data or list of all errors
```

**Working with Arrow Types**

All Arrow types support functional operations:

```kotlin
// Map transforms success values
val processed: Either<Error, Int> = result.map { it.length }

// FlatMap (bind) chains operations that return Arrow types
val chained: Either<Error, User> = fetchUserId()
    .flatMap { id -> fetchUser(id) }

// Fold handles both cases
result.fold(
    ifLeft = { error -> handleError(error) },
    ifRight = { value -> handleSuccess(value) }
)
```

**Real-World Relevance**

Arrow types are essential for:
- **Domain modeling**: Make invalid states unrepresentable
- **API clients**: Explicit error types for different failure modes
- **Validation pipelines**: Accumulate all form errors for user feedback
- **Null safety**: Eliminate null checks from your codebase
- **Functional composition**: Chain operations with confidence

Arrow transforms Kotlin into a more powerful functional language, giving you the tools to write safer, more expressive code.
