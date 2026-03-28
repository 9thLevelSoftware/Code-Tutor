---
type: "THEORY"
title: "KSP: Kotlin Symbol Processing"
---

**Estimated Time**: 60 minutes
**Difficulty**: Intermediate
**Prerequisites**: Annotation processing concepts, Gradle build basics

---

## Modern Annotation Processing with KSP

**Kotlin Symbol Processing (KSP)** is Google's next-generation annotation processing tool for Kotlin. It replaces the older kapt system with a Kotlin-native approach that offers dramatically faster build times, better multiplatform support, and cleaner APIs for code generation.

### KSP vs Kapt: The Performance Difference

| Aspect | Kapt | KSP |
|--------|------|-----|
| Build Speed | Slower (generates Java stubs) | 2x faster (direct Kotlin) |
| Multiplatform | Limited | Full KMP support |
| Incremental | Limited | Excellent |
| IDE Integration | Good | Better |
| Maintenance | Deprecated | Actively developed |

Kapt worked by generating Java stubs from Kotlin code, then running Java annotation processors. KSP processes Kotlin code directly—no intermediate stubs, no Java compilation overhead.

### Setting Up KSP

**Step 1: Add KSP Plugin**

```kotlin
// libs.versions.toml
[versions]
ksp = "2.0.0-1.0.21"

[plugins]
ksp = { id = "com.google.devtools.ksp", version.ref = "ksp" }

// build.gradle.kts (root)
plugins {
    alias(libs.plugins.ksp) apply false  // Version for all modules
}

// build.gradle.kts (module)
plugins {
    kotlin("multiplatform")
    alias(libs.plugins.ksp)  // Apply without version
}
```

**Step 2: Configure KSP for Multiplatform**

```kotlin
// shared/build.gradle.kts
dependencies {
    // Add KSP processor as dependency
    add("kspCommonMainMetadata", "com.squareup.moshi:moshi-ksp:1.15.1")
    
    // Or for specific targets
    add("kspAndroid", "some.processor:1.0")
    add("kspIosArm64", "some.processor:1.0")
}

// Configure KSP output
tasks.withType<com.google.devtools.ksp.processing.KspTask>().configureEach {
    // Output directory for generated code
    ksp {
        arg("option_name", "value")
    }
}
```

### Popular KSP-Compatible Libraries

| Library | KSP Artifact | Purpose |
|---------|--------------|---------|
| Moshi | `moshi-ksp` | JSON serialization |
| Room | `room-compiler` | Database ORM |
| Koin | `koin-annotations` | Dependency injection |
| SealedX | `sealedx-processor` | Sealed class extensions |
| Poko | `poko-processor` | Immutable data classes |

### Migration from Kapt

**Before (Kapt):**
```kotlin
plugins {
    kotlin("kapt")
}

dependencies {
    kapt("com.squareup.moshi:moshi-kotlin-codegen:1.14.0")
    implementation("com.squareup.moshi:moshi:1.14.0")
}
```

**After (KSP):**
```kotlin
plugins {
    id("com.google.devtools.ksp") version "2.0.0-1.0.21"
}

dependencies {
    add("kspCommonMainMetadata", "com.squareup.moshi:moshi-ksp:1.15.1")
    implementation("com.squareup.moshi:moshi:1.15.1")
}
```

### KSP with Version Catalogs

```toml
# libs.versions.toml
[versions]
ksp = "2.0.0-1.0.21"
moshi = "1.15.1"
koin = "3.6.0"

[libraries]
moshi-core = { module = "com.squareup.moshi:moshi", version.ref = "moshi" }
moshi-ksp = { module = "com.squareup.moshi:moshi-ksp", version.ref = "moshi" }
koin-annotations = { module = "io.insert-koin:koin-annotations", version.ref = "koin" }
koin-ksp = { module = "io.insert-koin:koin-ksp-compiler", version.ref = "koin" }

[plugins]
ksp = { id = "com.google.devtools.ksp", version.ref = "ksp" }
```

```kotlin
// build.gradle.kts
dependencies {
    implementation(libs.moshi.core)
    add("kspCommonMainMetadata", libs.moshi.ksp)
    
    implementation(libs.koin.annotations)
    add("kspCommonMainMetadata", libs.koin.ksp)
}
```

### Writing a Simple KSP Processor

```kotlin
// Processor implementation
class BuilderProcessor : SymbolProcessor {
    override fun process(resolver: Resolver): List<KSAnnotated> {
        val symbols = resolver.getSymbolsWithAnnotation("com.example.Builder")
        
        symbols.filterIsInstance<KSClassDeclaration>()
            .forEach { classDeclaration ->
                generateBuilder(classDeclaration)
            }
        
        return emptyList()
    }
    
    private fun generateBuilder(classDeclaration: KSClassDeclaration) {
        val className = classDeclaration.simpleName.asString()
        val packageName = classDeclaration.packageName.asString()
        val properties = classDeclaration.getAllProperties()
        
        // Generate builder class
        val file = codeGenerator.createNewFile(
            dependencies = Dependencies(false, classDeclaration.containingFile!!),
            packageName = packageName,
            fileName = "${className}Builder"
        )
        
        file.write(
            """
            package $packageName
            
            class ${className}Builder {
                ${properties.joinToString("\n                ") { "private var ${it.simpleName.asString()}: ${it.type}? = null" }}
                
                ${properties.joinToString("\n                ") { 
                    "fun ${it.simpleName.asString()}(value: ${it.type}) = apply { this.${it.simpleName.asString()} = value }" 
                }}
                
                fun build(): $className {
                    return $className(
                        ${properties.joinToString(", ") { "${it.simpleName.asString()}!!" }}
                    )
                }
            }
            """.trimIndent().toByteArray()
        )
        file.close()
    }
}
```

### Real-World Relevance

KSP adoption is critical because:
- **K2 compiler requires KSP** - kapt won't work with K2
- **Build performance** directly impacts developer experience
- **Multiplatform support** enables code generation across all targets
- **Active ecosystem** with major libraries already migrated

Projects still using kapt should prioritize KSP migration as part of their Kotlin 2.0 upgrade path. The performance benefits and future-proofing make this a high-value investment for any Kotlin project using annotation processing.
