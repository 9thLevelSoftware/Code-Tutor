---
type: "THEORY"
title: "Introduction"
---

**Estimated Time**: 40 minutes

Testing in Kotlin Multiplatform presents unique challenges: shared code must work across platforms, but tests often need platform-specific runners. Understanding the testing philosophy helps you write effective, maintainable tests.

**The KMP Testing Challenge**

Unlike single-platform development, KMP tests must consider:
- **Common Tests**: Run on all platforms (JVM, Android, iOS, JS, Native)
- **Platform Tests**: Platform-specific behavior verification
- **Integration Tests**: Real devices and simulators

```
commonTest/          <- Runs on every platform
├── Unit tests        <- Pure Kotlin logic
└── Integration tests <- Platform-agnostic integration

androidUnitTest/     <- JVM-based Android tests
androidInstrumented/ <- Real/emulated device tests
iosTest/             <- iOS simulator tests
jsTest/              <- JavaScript runtime tests
```

**Testing Pyramid for KMP**

```
         /\
        /  \  E2E Tests (1%) - Critical user journeys
       /____\    on real devices
      /      \
     /        \ Integration Tests (9%) - Repositories,
    /__________\ ViewModels, data flows
   /            \
  /              \ Unit Tests (90%) - Use cases,
 /________________\ utilities, pure logic
```

**Test Strategy Principles**

1. **Maximize Common Tests**: Place as many tests as possible in `commonTest` - they validate behavior across all platforms automatically.

2. **Platform Tests for Platform Code**: Only write platform-specific tests when the implementation itself is platform-specific (e.g., `expect/actual`).

3. **Test Shared Contracts**: Verify that `expect` declarations have consistent behavior across all `actual` implementations:

```kotlin
// commonTest
@Test
fun appSettingsContract() = runTest {
    val settings = getSettings()
    settings.setString("key", "value")
    assertEquals("value", settings.getString("key"))
}
```

**Testing Tools by Platform**

| Platform | Test Framework | Notes |
|----------|---------------|-------|
| Common | kotlin.test | Runs on all targets |
| JVM | JUnit 5 | Full feature support |
| Android | JUnit 4 + Espresso | Instrumentation tests |
| iOS | XCTest (via kotlin.test) | Simulator required |
| JS/Native | kotlin.test | Built-in support |

A well-designed KMP testing strategy maximizes confidence while minimizing maintenance overhead across platforms.
