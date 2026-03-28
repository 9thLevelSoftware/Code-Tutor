---
type: "EXAMPLE"
title: "Designing Error Hierarchies"
---


Create clear, domain-specific error types:



```kotlin
// Layer your errors by domain

// API Layer errors
sealed interface ApiError {
    data class NetworkError(val cause: Throwable) : ApiError
    data class ServerError(val code: Int, val message: String) : ApiError
    data class DeserializationError(val body: String) : ApiError
    data object Timeout : ApiError
    data object Unauthorized : ApiError
}

// Domain Layer errors
sealed interface OrderError {
    data class NotFound(val id: Long) : OrderError
    data class InvalidStatus(val current: String, val expected: String) : OrderError
    data class InsufficientInventory(val productId: Long, val requested: Int, val available: Int) : OrderError
}

// Validation errors (for form validation)
sealed interface ValidationError {
    data class Required(val field: String) : ValidationError
    data class InvalidFormat(val field: String, val expected: String) : ValidationError
    data class OutOfRange(val field: String, val min: Int, val max: Int) : ValidationError
}

// Combine at boundaries
sealed interface AppError {
    data class Api(val error: ApiError) : AppError
    data class Order(val error: OrderError) : AppError
    data class Validation(val errors: NonEmptyList<ValidationError>) : AppError
}
```
