---
type: "THEORY"
title: "Introduction"
---

**Estimated Time**: 55 minutes

Koin is a pragmatic, lightweight dependency injection framework designed specifically for Kotlin. Unlike reflection-based DI frameworks, Koin uses Kotlin DSLs and inline functions, making it fast, type-safe, and perfectly suited for Kotlin Multiplatform.

**Setting Up Koin**

Add Koin to your common module dependencies:

```kotlin
// gradle/libs.versions.toml
[versions]
koin = "3.5.6"

[dependencies]
koin-core = { module = "io.insert-koin:koin-core", version.ref = "koin" }
koin-compose = { module = "io.insert-koin:koin-compose", version.ref = "koin" }

// build.gradle.kts (commonMain)
implementation(libs.koin.core)
implementation(libs.koin.compose)
```

**Koin Core Concepts**

1. **Modules** - Organize related dependencies:

```kotlin
val appModule = module {
    // Singleton - one instance for entire app
    single<UserRepository> { UserRepositoryImpl(get()) }
    
    // Factory - new instance each request
    factory { UserViewModel(get()) }
    
    // ViewModel - scoped to ViewModelStore
    viewModel { ProfileViewModel(get()) }
}
```

2. **Declaring Dependencies** - Define how objects are created:

```kotlin
// Constructor injection with get()
class UserRepositoryImpl(
    private val api: ApiService
) : UserRepository

module {
    single { ApiService() }
    single<UserRepository> { UserRepositoryImpl(get()) }
}
```

3. **Starting Koin** - Initialize at application startup:

```kotlin
class MyApplication : Application() {
    override fun onCreate() {
        super.onCreate()
        startKoin {
            androidContext(this@MyApplication)
            modules(appModule, networkModule, dataModule)
        }
    }
}
```

4. **Injecting Dependencies** - Retrieve instances where needed:

```kotlin
// In a Composable
@Composable
fun UserScreen() {
    val viewModel: UserViewModel = koinViewModel()
    // Use viewModel...
}

// In non-Compose code
val repository: UserRepository by inject()
```

**Koin vs Alternatives**

| Feature | Koin | Dagger/Hilt | Manual DI |
|---------|------|-------------|-----------|
| Reflection | No | Yes (compile-time) | No |
| KMP Support | Native | Limited | Yes |
| Learning Curve | Low | High | Low |
| Boilerplate | Minimal | Annotations | Medium |

Koin's DSL-first approach makes it the de facto choice for Kotlin Multiplatform projects.
