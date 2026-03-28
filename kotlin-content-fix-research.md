# Kotlin Course Content Fix - Research Summary

**Date:** March 28, 2026  
**Purpose:** Provide accurate, current information for fixing 161 audit issues

---

## 1. Ktor 3.4 Backend Development

**Current Version:** 3.4.1 (January 2026)

### Key Features for Course:
- **Routing:** Type-safe routing DSL with `routing { get("/path") { } }`
- **Authentication:** JWT, OAuth, Bearer, Basic auth plugins
- **Content Negotiation:** kotlinx.serialization default
- **Request/Response:** `call.receive<T>()` and `call.respond()`
- **WebSockets:** First-class WebSocket support
- **New in 3.4:** OpenAPI generation, improved test client

### Best Practices:
- Use `Application.module()` for setup
- Install ContentNegotiation once in application config
- Use typed routes over string paths
- Leverage Ktor client for testing

---

## 2. Compose Multiplatform 1.10

**Current Version:** 1.10.3 stable / 1.11.0-beta

### Key Concepts:
- **@Composable:** UI functions marked as composable
- **State Management:** `remember { mutableStateOf() }` for local, ViewModel for shared
- **Recomposition:** UI updates when state changes
- **Layouts:** Row, Column, Box, ConstraintLayout (CMP 1.10+)
- **Material 3:** Complete Material 3 components available

### Pull to Refresh (Material 3):
**OLD (Deprecated):** `SwipeRefresh` from material 2
**NEW:** `PullToRefreshBox` from `androidx.compose.material3.pulltorefresh`

```kotlin
import androidx.compose.material3.pulltorefresh.PullToRefreshBox

PullToRefreshBox(
    isRefreshing = isRefreshing,
    onRefresh = { viewModel.refresh() }
) {
    LazyColumn { /* content */ }
}
```

### MVVM Pattern in KMP:
- Use `ViewModel` from lifecycle-viewmodel-compose
- `StateFlow` for state management
- `collectAsState()` for Compose integration
- moko-mvvm library for advanced needs

---

## 3. Arrow Functional Programming 2.x

**Current Version:** 2.1.0

### Core Types:
- **Either<L, R>:** Left (error) or Right (success)
- **Option<T>:** Some or None (nullable alternative)
- **Validated<E, A>:** Accumulates errors (great for forms)
- **Raise DSL:** Arrow 2.x preferred error handling

### Railway-Oriented Programming:
```kotlin
// Chain operations with flatMap
userInput
  .flatMap(::validateEmail)
  .flatMap(::validatePassword)
  .flatMap(::createUser)
```

### Raise DSL (Modern Arrow):
```kotlin
fun fetchUser(id: UserId): User = raise {
    ensure(id.value > 0) { InvalidUserId }
    repository.find(id) ?: raise(UserNotFound)
}
```

---

## 4. Kotlin 2.3 & K2 Compiler

**Current Version:** 2.3.20

### K2 Compiler:
- FIR (Frontend Intermediate Representation)
- 2x faster compilation
- Better type inference
- Default since Kotlin 2.0

### Context Parameters (Kotlin 2.2+):
**DEPRECATED:** Context receivers (will be removed in 2.2.20)
**NEW:** Context parameters (Beta in 2.2.0, Stable in 2.3)

```kotlin
// OLD (Deprecated)
context(Logger, Database)
fun process() { }

// NEW (Current)
fun process(context: Logger, context: Database) { }
// Or with named context
fun process(ctx: Logger, ctx: Database) { }
```

Migration is straightforward - use context parameters for new code.

---

## 5. Koin 4.x Dependency Injection

**Current Version:** 4.2.0

### Key Features:
- **Modules:** `module { single { Repository() } }`
- **Annotations:** `@Single`, `@Factory`, `@KoinViewModel`
- **KMP Support:** Common module with platform-specific bindings
- **Testing:** `KoinTestRule` (recommended) or `KoinTest` extension

### KSP Setup for Annotations:
```kotlin
// libs.versions.toml
[versions]
koin = "4.2.0"
ksp = "2.3.20-1.0.31"

[plugins]
ksp = { id = "com.google.devtools.ksp", version.ref = "ksp" }

// build.gradle.kts
plugins {
    alias(libs.plugins.ksp)
}

dependencies {
    kspCommonMainMetadata("io.insert-koin:koin-ksp-compiler:1.4.0")
}
```

### Testing Pattern:
```kotlin
@get:Rule
val koinTestRule = KoinTestRule.create {
    modules(testModule)
}
```

---

## 6. SQLDelight 2.x

**Current Version:** 2.3.2

### Migration from 1.x to 2.x:
**Package Change:**
- OLD: `com.squareup.sqldelight`
- NEW: `app.cash.sqldelight`

**Gradle Setup:**
```kotlin
plugins {
    id("app.cash.sqldelight") version "2.3.2"
}

sqldelight {
    databases {
        create("Database") {
            packageName.set("com.example")
        }
    }
}
```

**Multiplatform Drivers:**
- Android: `app.cash.sqldelight:android-driver`
- iOS: `app.cash.sqldelight:native-driver`
- JVM: `app.cash.sqldelight:sqlite-driver`

---

## 7. Testing with Turbine

**Current Version:** 1.2.1

### Turbine for Flow Testing:
```kotlin
// Dependency
commonTest.dependencies {
    implementation("app.cash.turbine:turbine:1.2.1")
}

// Usage
@Test
fun testFlow() = runTest {
    viewModel.state.test {
        assertEquals(Loading, awaitItem())
        assertEquals(Success(data), awaitItem())
        awaitComplete()
    }
}
```

### Compose UI Testing:
```kotlin
commonTest.dependencies {
    implementation("org.jetbrains.compose.ui:ui-test-junit4:1.10.3")
    implementation("org.jetbrains.compose.ui:ui-test-manifest:1.10.3")
}
```

---

## Summary of Version Changes Needed

| Library | Course Version | Current Version | Status |
|---------|---------------|-----------------|--------|
| Kotlin | 2.3 | 2.3.20 | ✅ Current |
| Ktor | 3.4 | 3.4.1 | ✅ Current |
| Compose MP | 1.10 | 1.10.3 | ✅ Current |
| SQLDelight | 2.2.1 | 2.3.2 | ⚠️ Update package verification |
| Arrow | 2.2.1 | 2.1.0 | ⚠️ Verify API compatibility |
| Koin | 4.0 | 4.2.0 | ✅ Current |
| Turbine | - | 1.2.1 | ✅ Add to course |

---

## Critical Fix Priorities

1. **Placeholder Content (35 lessons)** - Replace "sufficiently long piece of placeholder text"
2. **Empty Lessons (M06 L13-14, M03 L32)** - Create complete lesson structure
3. **Code Compilation (M10)** - Add missing iOS import
4. **Deprecated APIs** - SwipeRefresh → PullToRefreshBox, context receivers → context parameters
5. **Package Names** - Verify SQLDelight 2.x uses app.cash
