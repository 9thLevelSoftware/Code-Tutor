---
type: "THEORY"
title: "Introduction"
---

**Estimated Time**: 60 minutes

As your KMP application grows, you'll need more sophisticated dependency management. Koin provides advanced features like scopes, qualifiers, and lazy initialization to handle complex scenarios.

**Scopes for Lifecycle Management**

Scopes ensure dependencies exist only within specific boundaries:

```kotlin
// Define a scope
module {
    // Create scope for a user session
    scope<UserSession> {
        // Scoped to session lifecycle
        scoped<AuthToken> { createAuthToken() }
        scoped { UserRepository(get(), get()) }
    }
}

// Use the scope
val sessionScope = koin.createScope<UserSession>()
val repository: UserRepository = sessionScope.get()
// When session ends
sessionScope.close() // All scoped instances cleared
```

**Qualifiers for Multiple Implementations**

When you need multiple instances of the same type:

```kotlin
module {
    single(named("production")) { ApiClient(BASE_URL_PROD) }
    single(named("staging")) { ApiClient(BASE_URL_STAGING) }
    single { DatabaseClient(get(qualifier = named("production"))) }
}

// Inject with qualifier
val prodApi: ApiClient by inject(named("production"))
```

**Lazy and Singleton Patterns**

```kotlin
module {
    // Lazy initialization - created on first access
    single { HeavyResource() } createOnStart false
    
    // Eager singleton - created immediately
    single(createdAtStart = true) { StartupInitializer() }
    
    // Bind multiple interfaces to single instance
    single<Repository> { UserRepository() } bind AnalyticsProvider::class
}
```

**Conditional Dependencies**

```kotlin
// Different implementations based on build configuration
module {
    single<Logger> { 
        if (BuildConfig.DEBUG) DebugLogger() else ReleaseLogger() 
    }
}
```

**Best Practices**

1. Use scopes for resources tied to specific lifecycles (sessions, screens)
2. Prefer constructor injection over field injection
3. Keep qualifiers descriptive: `named("cached")` not `named("impl1")`
4. Avoid deeply nested dependency graphs - flatten where possible

These patterns help maintain clean dependency graphs even as your application scales to dozens of modules across multiple platforms.
