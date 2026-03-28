---
type: "THEORY"
title: "Dependency Management with Version Catalogs"
---

**Estimated Time**: 75 minutes
**Difficulty**: Intermediate
**Prerequisites**: Gradle basics, understanding of build.gradle.kts

---

## Centralized Dependency Management

Managing dependencies across a multiplatform project can become chaotic fast. Version catalogs solve this by centralizing all dependency versions in a single TOML file, ensuring consistency across modules and making updates safer.

### The Version Catalog File

Located at `gradle/libs.versions.toml`, this file defines your project's dependency universe:

```toml
[versions]
# Kotlin and core
kotlin = "2.1.0"
ksp = "2.1.0-1.0.28"
coroutines = "1.9.0"

# Android
agp = "8.2.0"
compose = "1.6.0"
activity-compose = "1.8.2"

# KMP libraries
ktor = "3.0.0"
sqldelight = "2.0.1"
koin = "3.6.0"

# Testing
junit = "4.13.2"
turbine = "1.1.0"

[libraries]
# Coroutines
coroutines-core = { module = "org.jetbrains.kotlinx:kotlinx-coroutines-core", version.ref = "coroutines" }
coroutines-test = { module = "org.jetbrains.kotlinx:kotlinx-coroutines-test", version.ref = "coroutines" }

# Ktor client
ktor-client-core = { module = "io.ktor:ktor-client-core", version.ref = "ktor" }
ktor-client-okhttp = { module = "io.ktor:ktor-client-okhttp", version.ref = "ktor" }
ktor-client-darwin = { module = "io.ktor:ktor-client-darwin", version.ref = "ktor" }
ktor-content-negotiation = { module = "io.ktor:ktor-client-content-negotiation", version.ref = "ktor" }
ktor-kotlinx-json = { module = "io.ktor:ktor-serialization-kotlinx-json", version.ref = "ktor" }

# SQLDelight
sqldelight-runtime = { module = "app.cash.sqldelight:runtime", version.ref = "sqldelight" }
sqldelight-android = { module = "app.cash.sqldelight:android-driver", version.ref = "sqldelight" }
sqldelight-native = { module = "app.cash.sqldelight:native-driver", version.ref = "sqldelight" }

# Koin DI
koin-core = { module = "io.insert-koin:koin-core", version.ref = "koin" }
koin-compose = { module = "io.insert-koin:koin-compose", version.ref = "koin" }
koin-android = { module = "io.insert-koin:koin-android", version.ref = "koin" }

# Testing
turbine = { module = "app.cash.turbine:turbine", version.ref = "turbine" }
junit = { module = "junit:junit", version.ref = "junit" }

[bundles]
# Group related dependencies for common use cases
ktor-client = ["ktor-client-core", "ktor-content-negotiation", "ktor-kotlinx-json"]
coroutines = ["coroutines-core", "coroutines-test"]

[plugins]
android-application = { id = "com.android.application", version.ref = "agp" }
kotlin-multiplatform = { id = "org.jetbrains.kotlin.multiplatform", version.ref = "kotlin" }
compose = { id = "org.jetbrains.compose", version.ref = "compose" }
ksp = { id = "com.google.devtools.ksp", version.ref = "ksp" }
sqldelight = { id = "app.cash.sqldelight", version.ref = "sqldelight" }
```

### Using Catalog References in Build Scripts

```kotlin
// build.gradle.kts
plugins {
    alias(libs.plugins.kotlin.multiplatform)
    alias(libs.plugins.android.application)
    alias(libs.plugins.sqldelight)
}

kotlin {
    sourceSets {
        commonMain.dependencies {
            // Use version catalog references
            implementation(libs.coroutines.core)
            implementation(libs.bundles.ktor.client)
            implementation(libs.koin.core)
        }
        
        commonTest.dependencies {
            implementation(libs.coroutines.test)
            implementation(libs.turbine)
        }
        
        androidMain.dependencies {
            implementation(libs.ktor.client.okhttp)
            implementation(libs.sqldelight.android)
            implementation(libs.koin.android)
        }
        
        iosMain.dependencies {
            implementation(libs.ktor.client.darwin)
            implementation(libs.sqldelight.native)
        }
    }
}
```

### Benefits of Version Catalogs

| Benefit | Explanation |
|---------|-------------|
| **Single Source of Truth** | One version number for all modules |
| **Type-Safe Access** | IDE autocomplete for `libs.*` references |
| **Renaming Support** | Change coordinates without updating all files |
| **Bundle Grouping** | Related dependencies as single reference |
| **Update Safety** | Version updates propagate automatically |

### Migration Strategy

When adding a new library:

1. Add version to `[versions]` section
2. Define the library in `[libraries]` section
3. Use `alias(libs.library.name)` in `plugins` or `implementation(libs.library.name)` in dependencies
4. Sync project to verify resolution

### Real-World Relevance

Version catalogs become essential as projects grow:
- **Team consistency** prevents version mismatch bugs
- **Automated updates** via tools like Renovate or Dependabot
- **Security patches** apply across all modules instantly
- **Documentation**—the TOML file becomes a dependency inventory

For KMP projects specifically, catalogs manage the complexity of platform-specific dependencies with different versions for Android, iOS, Desktop, and Web targets.
