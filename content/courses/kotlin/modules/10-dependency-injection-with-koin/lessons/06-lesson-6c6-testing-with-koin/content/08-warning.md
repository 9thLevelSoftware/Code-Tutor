---
type: "WARNING"
title: "Testing Anti-Patterns"
---

### ❌ Forgetting to clean up Koin

```kotlin
@Test
fun test1() {
    startKoin { ... }
    // Koin stays running, may cause conflicts
}

@Test
fun test2() {
    startKoin { ... }  // Error: Koin already started!
}
```

**Fix**: Use `AutoCloseKoinTest` for automatic cleanup:

```kotlin
class MyKoinTest : KoinTest, AutoCloseKoinTest {
    // Koin automatically stopped after each test - no tearDown needed!
}
```

### ❌ Testing with production dependencies

```kotlin
// BAD: Tests hit real database/network
startKoin {
    modules(productionModule)
}
```

**Fix**: Create dedicated test modules with fakes.

### ❌ Not verifying modules

```kotlin
// Module has missing dependency, but no test catches it
val module = module {
    single { ServiceA(get()) }  // Needs ServiceB, not defined
}
```

**Fix**: Add module verification test:
```kotlin
@Test
fun `verify modules`() {
    module.verify()
}
```