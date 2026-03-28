---
type: "THEORY"
title: "Introduction"
---

**Estimated Time**: 50 minutes

Kotlin Multiplatform's expect/actual mechanism pairs perfectly with dependency injection. While shared code defines interfaces (expect), platform modules provide concrete implementations (actual). Koin makes wiring these together seamless.

**Expect/Actual with DI**

Define shared interfaces in commonMain:

```kotlin
// commonMain/kotlin/AppSettings.kt
interface AppSettings {
    suspend fun getString(key: String): String?
    suspend fun setString(key: String, value: String)
}
```

Provide platform implementations:

```kotlin
// androidMain/kotlin/AndroidSettings.kt
actual fun getPlatformSettings(): AppSettings = 
    AndroidSettings(context)

// iosMain/kotlin/IOSSettings.kt
import platform.Foundation.*  // Required for NSUserDefaults

actual fun getPlatformSettings(): AppSettings = 
    IOSSettings(NSUserDefaults.standardUserDefaults)
```

**Platform-Specific Modules**

Create separate Koin modules for each platform:

```kotlin
// commonMain/kotlin/di/PlatformModule.kt
expect val platformModule: Module

// commonMain/kotlin/di/AppModule.kt
val appModule = module {
    single { NetworkClient() }
    includes(platformModule) // Platform-specific bindings
}

// androidMain/kotlin/di/PlatformModule.kt
actual val platformModule = module {
    single<AppSettings> { AndroidSettings(androidContext()) }
    single<Analytics> { FirebaseAnalyticsWrapper() }
}

// iosMain/kotlin/di/PlatformModule.kt
actual val platformModule = module {
    single<AppSettings> { IOSSettings() }
    single<Analytics> { MixpanelAnalytics() }
}
```

**Common Pattern: Platform Factories**

For cases where you need different behavior per platform:

```kotlin
// commonMain
interface DatabaseDriver {
    fun createDriver(): SqlDriver
}

expect fun createPlatformDriver(): DatabaseDriver

// In your module
module {
    single<SqlDriver> { createPlatformDriver().createDriver() }
}
```

**Best Practices**

1. Keep platform modules focused on platform-specific implementations only
2. Use naming conventions: `AndroidSettings`, `IOSSettings`, `DesktopSettings`
3. Prefer interfaces in shared code, implementations in platform modules
4. Document which dependencies are platform-specific to avoid confusion

This pattern ensures your shared business logic remains pure Kotlin while platform details are properly abstracted.
