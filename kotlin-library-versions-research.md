# Kotlin Library Versions & Best Practices Research Summary

**Research Date:** March 28, 2026  
**Purpose:** Verify current library versions and best practices for the Kotlin course

---

## 1. SQLDelight

### Current Version
- **Latest:** `2.3.2` (released March 16, 2026)
- **Previous stable:** `2.2.1` (November 2025)
- **Package:** `app.cash.sqldelight:gradle-plugin:2.3.2`

### Breaking Changes in 2.x
1. **Generated Query Return Types** (since 2.1.0):
   - Generated insert/update/delete statements now return `Long` (row count) instead of `Unit`
   - Functions relying on `Unit` return type need to be updated:
     ```kotlin
     // Before 2.1.0
     fun doAnInsert() = db.someQueries.insert()
     
     // After 2.1.0
     fun doAnInsert() {
       db.someQueries.insert()
     }
     ```

2. **Package Name Change** (since 2.0.0):
   - Changed from `com.squareup.sqldelight` to `app.cash.sqldelight`

3. **Gradle DSL Changes**:
   - New syntax for database configuration in Kotlin DSL:
     ```kotlin
     sqldelight {
       databases {
         create("Database") {
           packageName.set("com.sample")
         }
       }
     }
     ```

4. **Android minSdk** raised to 23 (was 21 in earlier versions)

### Documentation
- https://sqldelight.github.io/sqldelight/2.2.1/
- https://github.com/sqldelight/sqldelight/releases

---

## 2. Arrow (Functional Programming Library)

### Current Version
- **Latest:** `2.1.0` (released April 21, 2025)
- **Major stable:** `2.0.0` (released December 5, 2024)
- **Maven:** `io.arrow-kt:arrow-core:2.1.0`

### Key Changes from Arrow 1.x to 2.x

1. **Built with K2 compiler** - supports all Kotlin platforms including WebAssembly

2. **Binary compatibility broken** in several places (source-compatible if no deprecation warnings in 1.2.x)

3. **Optics improvements** (breaking changes):
   - Simplified hierarchy: traversals, optionals, lenses, prisms, and isos only
   - Nullable fields now represented as `Lens<T, String?>` instead of `Optional<T, String>`
   - To get 1.x behavior, apply `.notNull` after the optic
   - Generated optics no longer inlined by default (available under flag)

4. **Raise DSL improvements**:
   - New `accumulate` block for error accumulation:
     ```kotlin
     accumulate {
       val a by accumulating { checkOneThing() }
       val b by accumulating { checkOtherThing() }
       doSomething(a, b)
     }
     ```

5. **Retry improvements**:
   - Can now specify exception subclass for retrying:
     ```kotlin
     Schedule.recurs<Throwable>(2)
       .retry<IllegalArgumentException, _> { ... }
     ```

### Recommended Code Patterns
```kotlin
// Either with Raise DSL
suspend fun findUser(id: UserId): Either<UserNotFound, User> = TODO()

suspend fun fromTheSameCity(id1: UserId, id2: UserId): Either<UserNotFound, Boolean> =
  either {
    val user1 = findUser(id1).bind()
    val user2 = findUser(id2).bind()
    user1.city == user2.city
  }
```

### Documentation
- https://arrow-kt.io/learn/quickstart/
- https://arrow-kt.io/community/blog/2024/12/05/arrow-2-0/

---

## 3. Koin (Dependency Injection)

### Current Version
- **Latest:** `4.2.0` (released March 17, 2026)
- **Previous stable:** `4.1.1` (September 2025)
- **Maven:** `io.insert-koin:koin-core:4.2.0`

### Koin 4.0+ Changes

1. **Ktor 3.4 DI Bridge Integration**:
   - Full bidirectional DI between Koin and Ktor DI system
   - `KoinDependencyMapExtension` implements Ktor 3.2+'s DependencyMapExtension
   - New `KoinKtorApplication` DSL for bridging configuration

2. **AndroidX Navigation 3 Support**:
   - New `koin-compose-navigation3` module
   - EntryProvider API with metadata parameter passing
   - Generic EntryProvider support

3. **Compose Improvements**:
   - `koinActivityInject()` function for Activity scope retrieval
   - `UnboundKoinScope` API with `@KoinDelicateAPI`
   - Fixed premature scope release in onForgotten

4. **Core Resolver V2**:
   - Fixed parameter stack propagation
   - Injected params handling improvements
   - Child scope/ViewModel scope resolution fixes

5. **Lazy Modules**:
   - Parallel loading at startup for improved performance
   - Use `lazyModule { }` instead of `module { }` for lazy loading

### Testing Patterns

**JUnit 4 with KoinTestRule:**
```kotlin
class MyTest : KoinTest {
    @get:Rule
    val koinTestRule = KoinTestRule.create {
        modules(myModule)
    }
    
    @get:Rule
    val mockProvider = MockProviderRule.create { clazz ->
        Mockito.mock(clazz.java)
    }
    
    val component: Component by inject()
    
    @Test
    fun test() {
        val mock = declareMock<Component>()
        // test with mock
    }
}
```

**JUnit 5 with KoinTestExtension:**
```kotlin
class ExtensionTests : KoinTest {
    @JvmField
    @RegisterExtension
    val koinTestExtension = KoinTestExtension.create {
        modules(
            module {
                single { ComponentA() }
                single { ComponentB(get()) }
            }
        )
    }
    
    @Test
    fun contextIsCreatedForTheTest(koin: Koin) {
        Assertions.assertNotNull(koin.get<ComponentA>())
    }
}
```

### Breaking Changes (4.2.0)
- `minSdk` raised to 23 for Android
- `module` renamed to `lazyModule` in some contexts
- `BeanDef` constructor change (API signature update)

### Documentation
- https://insert-koin.io/
- https://insert-koin.io/docs/reference/koin-test/testing

---

## 4. Compose Multiplatform

### Current Version
- **Latest stable:** `1.10.3` (released March 19, 2026)
- **Latest beta:** `1.11.0-beta01` (released March 26, 2026)
- **Gradle plugin:** `org.jetbrains.compose:1.10.3`

### Material 3 Status
- **Compose Multiplatform Material3:** `1.10.0-alpha05`
- **Based on Jetpack:** Material3 `1.5.0-alpha08`
- **Coordinates:** `org.jetbrains.compose.material3:material3:1.10.0-alpha05`

### PullToRefreshContainer Status
`PullToRefreshContainer` (now called `PullToRefreshBox`) is available in Material3:
```kotlin
import androidx.compose.material3.pulltorefresh.PullToRefreshBox

PullToRefreshBox(
    isRefreshing = isRefreshing,
    onRefresh = { /* handle refresh */ }
) {
    // content
}
```

### Key Features in 1.10.x
- Unified `@Preview` annotation for all platforms
- Navigation 3 support on non-Android targets
- Compose Hot Reload bundled and stable
- Common @Preview annotation

### 1.11.0-beta01 Changes
- **Native iOS Text Input** mode (opt-in) for BasicTextField
- Skia updated to m144
- iOS: Dialog/Popup container views now on system transition view

### Breaking Changes (1.11.x)
- Non-Android `Shader` is now a dedicated Compose wrapper type
- Migration required for Skia/Skiko shaders: use `SkShader.asComposeShader()`
- `Key.Home` deprecated (use `Key.MoveHome`)

### Documentation
- https://www.jetbrains.com/help/kotlin-multiplatform-dev/compose-multiplatform.html
- https://github.com/JetBrains/compose-multiplatform/releases

---

## 5. Kotlin Language & K2 Compiler

### Current Version
- **Latest:** `2.3.20` (released March 16, 2026)
- **Previous stable:** `2.3.10` (February 2026)

### K2 Compiler Status
- **K2 is the default compiler** since Kotlin 2.0.0 (stable)
- All library versions listed in this document require/use K2 compiler

### Context Parameters vs Context Receivers

**Context receivers** (deprecated, experimental since 1.6.20):
```kotlin
context(Logger, Database)
suspend fun fetchData(): Data {
    // implicitly access Logger and Database
}
```

**Context parameters** (new in 2.2.0, Beta status):
- Replace context receivers (planned removal around 2.3 release)
- Require a name for each context parameter
- Migration supported via IntelliJ IDEA 2025.1+

```kotlin
context(logger: Logger, db: Database)
suspend fun fetchData(): Data {
    // access via logger.log() or db.query()
}
```

### Migration Path
1. Change compiler argument from `-Xcontext-receivers` to `-Xcontext-parameters`
2. Use IntelliJ IDEA quick-fix "Replace context receivers with context parameters"
3. Context receivers and parameters can coexist in project (but not within same module)

### Documentation
- https://kotlinlang.org/docs/whatsnew23.html
- https://blog.jetbrains.com/kotlin/2025/04/update-on-context-parameters/
- https://github.com/JetBrains/kotlin/releases

---

## 6. Ktor (Server/Client Framework)

### Current Version
- **Latest:** `3.4.1` (released March 4, 2026)
- **Previous:** `3.4.0` (January 2026)
- **Maven:** `io.ktor:ktor-server-core:3.4.1`

### Ktor 3.4 Features
1. **OpenAPI Specification Generation** (major new feature):
   - Automatic OpenAPI spec generation for Ktor Client and Server
   - Integration with authentication plugin
   - JSON schema inference for Jackson and Gson
   - Runtime-generated spec support

2. **Zstd compression support**

3. **HTTP QUERY method support**

4. **Duplex streaming for OkHttpClient**

5. **Dependency Injection improvements**:
   - Ktor 3.2+ DI extension support
   - File configuration for DI
   - Override DI conflict policy

### API Changes from 3.x
- **ByteReadChannel/ByteWriteChannel** now backed by kotlinx-io
- Previous implementations deprecated
- CIO engine uses `Dispatchers.IO` instead of `Dispatchers.Unconfined`

### Sample Code
```kotlin
// OpenAPI configuration
install(OpenAPI) {
    codeInferenceEnabled = true
    // OpenAPI spec auto-generated from routes
}

// Basic server
fun main() {
    embeddedServer(Netty, port = 8080) {
        routing {
            get("/") {
                call.respondText("Hello, World!")
            }
        }
    }.start(wait = true)
}
```

### Documentation
- https://ktor.io/
- https://ktor.io/docs/releases.html
- https://github.com/ktorio/ktor/releases

---

## 7. Turbine (Flow Testing)

### Current Version
- **Latest:** `1.2.1` (released June 11, 2025)
- **Previous:** `1.2.0` (October 2024)
- **Maven:** `app.cash.turbine:turbine:1.2.1`

### Changes in 1.2.x
1. **Fixed:** Calling `testIn` with a `CoroutineScope` that does not contain a `Job` no longer throws `IllegalStateException`

2. **Added:** `wasmWasi` target (1.2.0)

### Recommended Testing Pattern
```kotlin
// Basic usage with test block
myFlow.test {
    assertEquals("item", awaitItem())
    awaitComplete()
}

// Using testIn with scope
turbineScope {
    val turbine = myFlow.testIn(this)
    assertEquals("item", turbine.awaitItem())
    turbine.cancel()
}
```

### Notes
- Default wall-clock timeout increased to 3s (from 1s in earlier versions)
- Built with Kotlin 1.8.22 and kotlinx.coroutines 1.7.1
- Supports all Kotlin Multiplatform targets

### Documentation
- https://github.com/cashapp/turbine
- https://cashapp.github.io/turbine/docs/0.13.0/

---

## Summary Table

| Library | Current Version | Previous Version | Maven Coordinates |
|---------|----------------|------------------|-------------------|
| SQLDelight | 2.3.2 | 2.2.1 | app.cash.sqldelight:2.3.2 |
| Arrow | 2.1.0 | 2.0.0 | io.arrow-kt:arrow-core:2.1.0 |
| Koin | 4.2.0 | 4.1.1 | io.insert-koin:koin-core:4.2.0 |
| Compose Multiplatform | 1.10.3 | 1.10.0 | org.jetbrains.compose:1.10.3 |
| Kotlin | 2.3.20 | 2.3.10 | org.jetbrains.kotlin:kotlin-stdlib:2.3.20 |
| Ktor | 3.4.1 | 3.4.0 | io.ktor:ktor-server-core:3.4.1 |
| Turbine | 1.2.1 | 1.2.0 | app.cash.turbine:turbine:1.2.1 |

---

## Recommended Versions for Course Content

Based on stability and widespread adoption:

1. **SQLDelight:** Use `2.3.2` (latest, stable)
2. **Arrow:** Use `2.1.0` (latest stable, K2 ready)
3. **Koin:** Use `4.2.0` (latest, with Ktor 3.4 support)
4. **Compose Multiplatform:** Use `1.10.3` (stable) or `1.11.0-beta01` for latest features
5. **Kotlin:** Use `2.3.20` (latest stable with K2 compiler)
6. **Ktor:** Use `3.4.1` (latest, OpenAPI support)
7. **Turbine:** Use `1.2.1` (latest)

All these versions are compatible with each other and use the K2 compiler.
