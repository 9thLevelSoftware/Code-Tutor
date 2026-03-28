---
type: "THEORY"
title: "Introduction"
---

**Estimated Time**: 45 minutes

Traditional mocking libraries like Mockito don't work in Kotlin Multiplatform. This lesson covers why, and what alternatives you have.

### Testing Dependencies for KMP

When testing multiplatform code, these libraries are essential:

```toml
# gradle/libs.versions.toml
[versions]
turbine = "1.2.0"
compose-test = "1.10.0"

[libraries]
# For testing Kotlin Flows
turbine = { module = "app.cash.turbine:turbine", version.ref = "turbine" }

# For Compose Multiplatform UI testing
compose-ui-test = { module = "org.jetbrains.compose.ui:ui-test-junit4", version.ref = "compose-test" }
compose-ui-test-manifest = { module = "org.jetbrains.compose.ui:ui-test-manifest", version.ref = "compose-test" }
```

```kotlin
// commonTest dependencies
commonTest.dependencies {
    // Flow testing
    implementation(libs.turbine)
    
    // Koin testing
    implementation(libs.koin.test)
    
    // Coroutines test
    implementation(libs.kotlinx.coroutines.test)
}

// androidTest dependencies (for Compose tests)
androidTest.dependencies {
    implementation(libs.compose.ui.test)
    debugImplementation(libs.compose.ui.test.manifest)
}
```

### Why Turbine?

Turbine makes testing Kotlin Flows simple:

```kotlin
@Test
fun `state flow emits values`() = runTest {
    viewModel.state.test {
        assertEquals(Loading, awaitItem())     // Initial state
        assertEquals(Success, awaitItem())     // Loaded state
        cancelAndIgnoreRemainingEvents()
    }
}
```

### Testing Dependencies for KMP

When testing multiplatform code, these libraries are essential:

```toml
# gradle/libs.versions.toml
[versions]
turbine = "1.2.0"
compose-test = "1.10.0"

[libraries]
# For testing Kotlin Flows
turbine = { module = "app.cash.turbine:turbine", version.ref = "turbine" }

# For Compose Multiplatform UI testing
compose-ui-test = { module = "org.jetbrains.compose.ui:ui-test-junit4", version.ref = "compose-test" }
compose-ui-test-manifest = { module = "org.jetbrains.compose.ui:ui-test-manifest", version.ref = "compose-test" }
```

```kotlin
// commonTest dependencies
commonTest.dependencies {
    // Flow testing
    implementation(libs.turbine)
    
    // Koin testing
    implementation(libs.koin.test)
    
    // Coroutines test
    implementation(libs.kotlinx.coroutines.test)
}

// androidTest dependencies (for Compose tests)
androidTest.dependencies {
    implementation(libs.compose.ui.test)
    debugImplementation(libs.compose.ui.test.manifest)
}
```

### Why Turbine?

Turbine makes testing Kotlin Flows simple:

```kotlin
@Test
fun `state flow emits values`() = runTest {
    viewModel.state.test {
        assertEquals(Loading, awaitItem())     // Initial state
        assertEquals(Success, awaitItem())     // Loaded state
        cancelAndIgnoreRemainingEvents()
    }
}
```