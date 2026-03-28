---
type: "THEORY"
title: "Context Parameters: The Evolution from Receivers"
---

**Estimated Time**: 45 minutes
**Difficulty**: Advanced
**Prerequisites**: Extension functions, type classes, K2 compiler basics

---

**Important**: Context receivers (introduced experimentally in Kotlin 1.6) are deprecated. Context parameters are their replacement—a cleaner, more powerful mechanism for implicit dependency injection.

**What Are Context Parameters?**

Context parameters allow functions to require capabilities (type class instances, dependencies, or context) without explicitly passing them as parameters. They're declared in angle brackets after the function name:

```kotlin
// A type class defining a capability
interface Logger {
    fun log(message: String)
}

// Function requiring Logger context
fun <context Logger> doWork() {
    log("Starting work")  // Implicitly uses the Logger in context
    // ... do work ...
    log("Work completed")
}

// Providing context
with(ConsoleLogger()) {
    doWork()  // Logger is implicitly available
}
```

**Why Replace Context Receivers?**

Context receivers had issues:
- Confusing interaction with extension functions
- Unclear resolution rules with multiple receivers
- Complex mental model for developers

Context parameters solve these:
- Clear separation between "this" (receiver) and context
- Predictable resolution order
- Better composition with existing Kotlin patterns

**Basic Syntax**

```kotlin
// Single context parameter
fun <context Ctx> process(): Result

// Multiple context parameters
fun <context Logger, context Metrics> handleRequest(request: Request): Response

// With regular parameters
fun <context Database> queryUser(id: String): User?
```

**Use Cases**

**1. Type Classes (Ad-Hoc Polymorphism)**

Define capabilities that types can implement:

```kotlin
interface Serializer<T> {
    fun serialize(value: T): String
    fun deserialize(data: String): T
}

// Generic function requiring serialization capability
fun <T, context Serializer<T>> saveToCache(key: String, value: T) {
    val data = value.serialize()  // Uses context Serializer
    redis.set(key, data)
}

// Different types, different serializers
with(JsonSerializer<User>()) {
    saveToCache("user:1", user)
}

with(ProtobufSerializer<Order>()) {
    saveToCache("order:123", order)
}
```

**2. Dependency Injection Without Frameworks**

```kotlin
interface UserRepository {
    fun findById(id: String): User?
}

interface EmailService {
    fun sendEmail(to: String, subject: String, body: String)
}

// Service function requiring multiple dependencies
fun <context UserRepository, context EmailService> 
        welcomeNewUser(userId: String) {
    val user = findById(userId) ?: throw NotFoundException()
    sendEmail(user.email, "Welcome!", "Thanks for joining...")
}

// Application wiring
class ApplicationContext : UserRepository, EmailService {
    override fun findById(id: String): User? = ...
    override fun sendEmail(to: String, subject: String, body: String) = ...
}

// Usage
with(ApplicationContext()) {
    welcomeNewUser("user-123")
}
```

**3. Scoped Capabilities**

```kotlin
interface Transaction {
    fun commit()
    fun rollback()
}

fun <context Transaction> transfer(
    from: Account,
    to: Account,
    amount: Money
) {
    from.debit(amount)
    to.credit(amount)
    // Transaction context available for rollback if needed
}
```

**Comparison with Alternatives**

| Approach | Explicit? | Overhead | Flexibility |
|----------|-----------|----------|-------------|
| Context Parameters | At definition | Minimal | High |
| Implicit Parameters | At call site | Low | Medium |
| Dependency Injection | Config-based | Framework | High |
| Manual Passing | Always | Verbose | Low |

**Current Status**

Context parameters are in **preview** in Kotlin 2.1+:
```kotlin
@OptIn(ContextParameters::class)
fun <context Logger> example() { }
```

Full stabilization expected in Kotlin 2.2 or 2.3.

**Real-World Relevance**

Context parameters enable:
- **Type-class style programming** for generic algorithms
- **Zero-cost dependency injection** without frameworks
- **Scoped capabilities** for transactions, logging, security
- **Cleaner domain logic** without threading dependencies through every call

This feature positions Kotlin alongside Haskell, Scala, and Rust in offering powerful abstraction capabilities while maintaining zero-overhead and type safety.
