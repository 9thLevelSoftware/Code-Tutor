---
type: "THEORY"
title: "Migrating Projects to the K2 Compiler"
---

**Estimated Time**: 50 minutes
**Difficulty**: Intermediate
**Prerequisites**: Kotlin 1.x experience, Gradle build understanding

---

## Upgrading to the K2 Compiler

The K2 compiler represents a fundamental shift in Kotlin's architecture. While it's designed to be backward-compatible, migrating existing projects requires understanding potential edge cases, new language behaviors, and tooling updates. This lesson guides you through a safe, methodical migration process.

### Why Migration Matters

The K2 compiler offers significant improvements:
- **2x faster compilation** in many scenarios
- **Better type inference** reducing need for explicit types
- **Improved smart casts** handling more complex control flow
- **Enhanced error messages** with clearer explanations
- **Foundation for new features** like context parameters

However, the architectural differences mean some code that compiled with K1 may need adjustment.

### Migration Strategy

**Phase 1: Preparation**

```kotlin
// gradle.properties - Start with warnings
kotlin.experimental.tryK2=true
kotlin.suppress.experimental.compiler=warn
```

**Phase 2: Tooling Updates**

Ensure compatible versions:
- **Kotlin**: 2.0.0 or later
- **Gradle**: 8.5 or later
- **Android Gradle Plugin**: 8.2 or later
- **Compose Compiler**: Version matching Kotlin 2.0+

```kotlin
// libs.versions.toml
[versions]
kotlin = "2.0.0"
agp = "8.2.0"
compose-compiler = "1.5.10"  // For Kotlin 2.0

[plugins]
kotlin-multiplatform = { id = "org.jetbrains.kotlin.multiplatform", version.ref = "kotlin" }
android-application = { id = "com.android.application", version.ref = "agp" }
compose-compiler = { id = "org.jetbrains.kotlin.plugin.compose", version.ref = "kotlin" }
```

**Phase 3: Incremental Migration**

```kotlin
// build.gradle.kts
plugins {
    kotlin("multiplatform") version "2.0.0"
    kotlin("plugin.compose") version "2.0.0"  // New Compose compiler plugin
}

kotlin {
    // Explicitly opt into K2
    compilerOptions {
        languageVersion.set(org.jetbrains.kotlin.gradle.dsl.KotlinVersion.KOTLIN_2_0)
    }
}
```

### Common Migration Issues

**Issue 1: Changed Smart Cast Behavior**

```kotlin
// K1: This might have worked
var name: String? = null

fun updateName() {
    if (name != null) {
        // K2: Error - smart cast impossible due to mutable variable
        println(name.length)
    }
}

// Fix: Capture in local variable
fun updateNameFixed() {
    val localName = name
    if (localName != null) {
        println(localName.length)
    }
}
```

**Issue 2: Type Inference Differences**

```kotlin
// K1: May have inferred platform type
val list = javaList  // Inferred as List<String!>

// K2: More strict inference
// Fix: Explicit type if needed
val list: List<String> = javaList.filterNotNull()
```

**Issue 3: Annotation Processing Changes**

K2 uses KSP instead of kapt:

```kotlin
// Remove kapt plugin
// plugins { kotlin("kapt") }

// Add KSP
plugins {
    id("com.google.devtools.ksp") version "2.0.0-1.0.21"
}

// Update dependencies
dependencies {
    // kapt("some.processor:1.0") // OLD
    ksp("some.processor:1.0")     // NEW
}
```

**Issue 4: Compiler Plugin Compatibility**

Not all compiler plugins support K2:

```kotlin
// Check plugin documentation for K2 support
// Some plugins may need updates or alternatives

// Example: Serialization plugin is K2-ready
plugins {
    kotlin("plugin.serialization") version "2.0.0"
}
```

### Migration Checklist

- [ ] Update Gradle to 8.5+
- [ ] Update AGP to 8.2+ (Android projects)
- [ ] Update Kotlin to 2.0+
- [ ] Replace kapt with KSP
- [ ] Update Compose Compiler to new plugin
- [ ] Run tests with both K1 and K2
- [ ] Review smart cast usages
- [ ] Check compiler plugin compatibility
- [ ] Update CI/CD pipelines
- [ ] Document any required code changes

### Testing the Migration

```bash
# Build with verbose output to catch warnings
./gradlew build --info --stacktrace

# Run full test suite
./gradlew test

# Check for deprecations
./gradlew build -Pkotlin.experimental.tryK2=true

# Compare build times
# K1 baseline
./gradlew clean build
# K2 version
./gradlew clean build (with K2 enabled)
```

### Rollback Strategy

If issues arise:

```kotlin
// gradle.properties - Revert to K1
kotlin.experimental.tryK2=false
kotlin.languageVersion=1.9
```

Keep migration changes in a separate branch until full validation passes.

### Real-World Relevance

Migrating to K2 is becoming mandatory as:
- **JetBrains prioritizes K2** for new language features
- **IDE support** increasingly targets K2 architecture
- **Performance gains** directly impact developer productivity
- **Long-term support** for K1 will eventually end

Organizations with large Kotlin codebases should plan migration during Q2-Q3 2024 to avoid disruption when K2 becomes the default. The migration investment pays dividends in faster builds and access to modern language capabilities.
