---
type: "THEORY"
title: "Multiplatform Build Configuration"
---

**Estimated Time**: 90 minutes
**Difficulty**: Advanced
**Prerequisites**: Gradle basics, version catalogs, target platform knowledge

---

## Configuring KMP Build Targets

Kotlin Multiplatform projects compile to multiple target platforms—each with unique requirements, compiler options, and dependency needs. Understanding build configuration enables you to optimize compilation times, handle platform differences, and troubleshoot target-specific issues.

### Target Configuration Structure

```kotlin
// shared/build.gradle.kts
kotlin {
    // Android target
    androidTarget {
        compilations.all {
            kotlinOptions {
                jvmTarget = "1.8"
            }
        }
    }
    
    // iOS targets (device and simulator)
    iosX64()      // Simulator (Intel Macs)
    iosArm64()    // Physical devices
    iosSimulatorArm64()  // Simulator (Apple Silicon)
    
    // Desktop targets
    jvm("desktop") {
        compilations.all {
            kotlinOptions.jvmTarget = "17"
        }
    }
    
    // JavaScript target
    js(IR) {
        browser()
        nodejs()
    }
    
    // Source set hierarchy
    sourceSets {
        val commonMain by getting {
            dependencies {
                implementation(libs.coroutines.core)
                implementation(libs.ktor.client.core)
            }
        }
        
        val commonTest by getting {
            dependencies {
                implementation(kotlin("test"))
                implementation(libs.coroutines.test)
            }
        }
        
        val androidMain by getting {
            dependsOn(commonMain)
            dependencies {
                implementation(libs.ktor.client.okhttp)
            }
        }
        
        val iosMain by creating {
            dependsOn(commonMain)
            dependencies {
                implementation(libs.ktor.client.darwin)
            }
        }
        
        val iosX64Main by getting { dependsOn(iosMain) }
        val iosArm64Main by getting { dependsOn(iosMain) }
        val iosSimulatorArm64Main by getting { dependsOn(iosMain) }
    }
}
```

### Platform-Specific Compiler Options

Different platforms support different language features:

```kotlin
kotlin {
    // Android: Optimize for ART
    androidTarget {
        publishLibraryVariants("release", "debug")
    }
    
    // iOS: Configure memory management
    iosArm64 {
        compilations.configureEach {
            compilerOptions.configure {
                // Enable advanced optimizations
                freeCompilerArgs.add("-Xbinary=bundleId=com.example.shared")
            }
        }
        binaries.framework {
            baseName = "Shared"
            isStatic = true  // Link statically for simpler distribution
            
            // Export dependencies for Objective-C interop
            export(project(":shared"))
            transitiveExport = true
        }
    }
    
    // JVM/Desktop: Configure for specific JVM version
    jvm("desktop") {
        withJava()
        compilations.all {
            kotlinOptions.jvmTarget = "17"
        }
    }
}
```

### Common Build Pitfalls

| Issue | Solution |
|-------|----------|
| **Duplicate class errors** | Ensure no overlapping dependencies between targets |
| **iOS framework not found** | Verify `isStatic` setting and framework export |
| **Compilation speed** | Use Gradle build cache and configure daemon |
| **Memory issues** | Increase heap size in `gradle.properties` |

### Optimization Settings

```kotlin
// gradle.properties
# Build performance
org.gradle.jvmargs=-Xmx8g -XX:MaxMetaspaceSize=512m
org.gradle.caching=true
org.gradle.parallel=true
org.gradle.configureondemand=true

# Kotlin compiler
kotlin.compiler.execution.strategy=in-process
kotlin.incremental=true
kotlin.incremental.multiplatform=true

# iOS
kotlin.native.cacheKind.iosArm64=static
kotlin.native.cacheKind.iosSimulatorArm64=static
```

### Real-World Relevance

Proper multiplatform configuration directly impacts:
- **Build times**: Correct settings reduce compilation by 50%+
- **Binary sizes**: Optimization flags control framework bloat
- **Runtime performance**: Compiler options affect execution speed
- **Debugging**: Source map generation for JS, symbol generation for native

Production KMP projects require careful tuning of these settings. What works for a demo project often fails at scale—mastering build configuration separates prototype builders from production engineers.
