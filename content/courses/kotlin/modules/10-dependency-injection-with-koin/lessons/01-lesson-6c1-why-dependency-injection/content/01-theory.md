---
type: "THEORY"
title: "Introduction"
---

**Estimated Time**: 45 minutes

Dependency Injection (DI) is a design pattern that promotes loose coupling and makes your code more testable and maintainable. In Kotlin Multiplatform, DI becomes even more critical as you share business logic across platforms while still needing platform-specific implementations.

**Why Dependency Injection Matters**

Without DI, classes often create their dependencies directly, leading to tight coupling:

```kotlin
class UserViewModel {
    // Hard dependency - difficult to test or replace
    private val userRepository = UserRepository()
    private val analytics = FirebaseAnalytics()
}
```

With DI, dependencies are provided from outside, making the class focus on its primary responsibility:

```kotlin
class UserViewModel(
    private val userRepository: UserRepository,
    private val analytics: Analytics
) {
    // Uses provided dependencies - easy to swap implementations
}
```

**Core DI Principles**

1. **Inversion of Control**: Instead of classes creating their own dependencies, an external container manages object creation and lifecycle.

2. **Interface Segregation**: Depend on abstractions (interfaces) rather than concrete implementations:

```kotlin
interface Analytics {
    fun track(event: String, params: Map<String, String> = emptyMap())
}

// Platform-specific implementations
class AndroidAnalytics : Analytics { /* ... */ }
class IOSAnalytics : Analytics { /* ... */ }
```

3. **Single Responsibility**: Each class has one reason to change - your ViewModel handles UI logic, not object construction.

**DI Patterns in Kotlin**

- **Constructor Injection**: Pass dependencies via constructor (preferred)
- **Property Injection**: Set dependencies after construction
- **Service Locator**: Request dependencies from a central registry (less preferred)

For KMP projects, constructor injection shines because it works identically across all platforms and plays nicely with Compose Multiplatform. The next lessons will show you how Koin, a Kotlin-native DI framework, makes implementing these patterns effortless.
