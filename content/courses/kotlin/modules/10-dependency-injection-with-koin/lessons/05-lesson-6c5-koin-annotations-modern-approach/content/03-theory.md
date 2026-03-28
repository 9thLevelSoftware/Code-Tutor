---
type: "THEORY"
title: "Setup for Koin Annotations"
---

### Add KSP and Annotations

```toml
# gradle/libs.versions.toml
[versions]
koin = "4.1.1"
ksp = "2.3.4"

[libraries]
koin-annotations = { module = "io.insert-koin:koin-annotations", version.ref = "koin" }
koin-ksp-compiler = { module = "io.insert-koin:koin-ksp-compiler", version.ref = "koin" }

[plugins]
ksp = { id = "com.google.devtools.ksp", version.ref = "ksp" }
```

```kotlin
// build.gradle.kts (project level)
plugins {
    alias(libs.plugins.ksp) apply false
}

// shared/build.gradle.kts
plugins {
    alias(libs.plugins.ksp)
}

kotlin {
    sourceSets {
        commonMain.dependencies {
            implementation(libs.koin.core)
            implementation(libs.koin.annotations)
        }
    }
}

dependencies {
    add("kspCommonMainMetadata", libs.koin.ksp.compiler)
    add("kspAndroid", libs.koin.ksp.compiler)
    add("kspIosArm64", libs.koin.ksp.compiler)
    add("kspIosSimulatorArm64", libs.koin.ksp.compiler)
}
```

### KSP Plugin Configuration (Multiplatform)

For Kotlin Multiplatform projects, KSP requires special configuration to work across all targets:

```kotlin
// shared/build.gradle.kts
plugins {
    kotlin("multiplatform")
    alias(libs.plugins.ksp)
}

kotlin {
    androidTarget()
    iosX64()
    iosArm64()
    iosSimulatorArm64()
    
    sourceSets {
        commonMain.dependencies {
            implementation(libs.koin.core)
            implementation(libs.koin.annotations)
        }
    }
}

// KSP configuration for multiplatform
dependencies {
    // Common metadata
    add("kspCommonMainMetadata", libs.koin.ksp.compiler)
    
    // Android targets
    add("kspAndroid", libs.koin.ksp.compiler)
    
    // iOS targets
    add("kspIosX64", libs.koin.ksp.compiler)
    add("kspIosArm64", libs.koin.ksp.compiler)
    add("kspIosSimulatorArm64", libs.koin.ksp.compiler)
    
    // JVM/Desktop (if applicable)
    add("kspJvm", libs.koin.ksp.compiler)
}

// Task dependencies for KSP
// Ensure generated code is available before compilation
tasks.withType<org.jetbrains.kotlin.gradle.dsl.KotlinCompile<*>>().configureEach {
    if (name != "kspCommonMainKotlinMetadata") {
        dependsOn("kspCommonMainKotlinMetadata")
    }
}
```

### Generated Code Location

Koin KSP generates code in the following locations:

| Platform | Generated Code Location |
|----------|------------------------|
| Common | `build/generated/ksp/metadata/commonMain/` |
| Android | `build/generated/ksp/android/` |
| iOS ARM64 | `build/generated/ksp/iosArm64/` |
| iOS Simulator | `build/generated/ksp/iosSimulatorArm64/` |

Add the generated sources to your IDE's source sets (optional, for IDE completion):

```kotlin
// shared/build.gradle.kts
kotlin {
    sourceSets {
        commonMain {
            kotlin.srcDirs("build/generated/ksp/metadata/commonMain/kotlin")
        }
    }
}
```

### Build Requirements

1. **First build**: Run `./gradlew kspCommonMainKotlinMetadata` before compiling
2. **Incremental builds**: KSP automatically regenerates when annotations change
3. **Clean builds**: Use `./gradlew clean` when KSP generates stale code

### Troubleshooting KSP

**Error: "Cannot find generated module"**
- Ensure KSP task ran before compilation
- Check `build/generated/ksp/` directory exists
- Verify annotation syntax is correct

**Error: "Unresolved reference: GeneratedModule"**
- Add generated source directories to sourceSets
- Rebuild the project with `./gradlew clean build`

**Slow compilation**
- KSP runs per target; consider using `kspCommonMainMetadata` only for shared code
- Exclude unnecessary targets from KSP if not using Koin annotations there

### KSP Plugin Configuration (Multiplatform)

For Kotlin Multiplatform projects, KSP requires special configuration to work across all targets:

```kotlin
// shared/build.gradle.kts
plugins {
    kotlin("multiplatform")
    alias(libs.plugins.ksp)
}

kotlin {
    androidTarget()
    iosX64()
    iosArm64()
    iosSimulatorArm64()
    
    sourceSets {
        commonMain.dependencies {
            implementation(libs.koin.core)
            implementation(libs.koin.annotations)
        }
    }
}

// KSP configuration for multiplatform
dependencies {
    // Common metadata
    add("kspCommonMainMetadata", libs.koin.ksp.compiler)
    
    // Android targets
    add("kspAndroid", libs.koin.ksp.compiler)
    
    // iOS targets
    add("kspIosX64", libs.koin.ksp.compiler)
    add("kspIosArm64", libs.koin.ksp.compiler)
    add("kspIosSimulatorArm64", libs.koin.ksp.compiler)
    
    // JVM/Desktop (if applicable)
    add("kspJvm", libs.koin.ksp.compiler)
}

// Task dependencies for KSP
// Ensure generated code is available before compilation
tasks.withType<org.jetbrains.kotlin.gradle.dsl.KotlinCompile<*>>().configureEach {
    if (name != "kspCommonMainKotlinMetadata") {
        dependsOn("kspCommonMainKotlinMetadata")
    }
}
```

### Generated Code Location

Koin KSP generates code in the following locations:

| Platform | Generated Code Location |
|----------|------------------------|
| Common | `build/generated/ksp/metadata/commonMain/` |
| Android | `build/generated/ksp/android/` |
| iOS ARM64 | `build/generated/ksp/iosArm64/` |
| iOS Simulator | `build/generated/ksp/iosSimulatorArm64/` |

Add the generated sources to your IDE's source sets (optional, for IDE completion):

```kotlin
// shared/build.gradle.kts
kotlin {
    sourceSets {
        commonMain {
            kotlin.srcDirs("build/generated/ksp/metadata/commonMain/kotlin")
        }
    }
}
```

### Build Requirements

1. **First build**: Run `./gradlew kspCommonMainKotlinMetadata` before compiling
2. **Incremental builds**: KSP automatically regenerates when annotations change
3. **Clean builds**: Use `./gradlew clean` when KSP generates stale code

### Troubleshooting KSP

**Error: "Cannot find generated module"**
- Ensure KSP task ran before compilation
- Check `build/generated/ksp/` directory exists
- Verify annotation syntax is correct

**Error: "Unresolved reference: GeneratedModule"**
- Add generated source directories to sourceSets
- Rebuild the project with `./gradlew clean build`

**Slow compilation**
- KSP runs per target; consider using `kspCommonMainMetadata` only for shared code
- Exclude unnecessary targets from KSP if not using Koin annotations there