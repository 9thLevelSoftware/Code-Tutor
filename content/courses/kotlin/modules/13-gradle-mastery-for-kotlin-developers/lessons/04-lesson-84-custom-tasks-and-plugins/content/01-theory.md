---
type: "THEORY"
title: "Custom Gradle Tasks and Plugins"
---

**Estimated Time**: 75 minutes
**Difficulty**: Advanced
**Prerequisites**: Gradle basics, Kotlin DSL familiarity

---

## Extending Gradle with Custom Automation

While Gradle's built-in tasks handle standard build operations, production projects need custom automation—code generation, deployment scripts, quality checks, and specialized workflows. Custom tasks and plugins let you treat build logic as code, versioned and tested like any other part of your project.

### Creating Custom Tasks

Custom tasks are Kotlin classes extending Gradle's task types:

```kotlin
// build.gradle.kts
abstract class GenerateApiClientTask : DefaultTask() {
    @get:InputFile
    abstract val specFile: RegularFileProperty
    
    @get:OutputDirectory
    abstract val outputDir: DirectoryProperty
    
    @get:Input
    abstract val packageName: Property<String>
    
    @TaskAction
    fun generate() {
        val spec = specFile.get().asFile.readText()
        val output = outputDir.get().asFile
        
        // Code generation logic
        logger.lifecycle("Generating API client from ${specFile.get()}")
        
        // Example: Generate Kotlin data classes from OpenAPI spec
        val generator = OpenApiGenerator(packageName.get())
        generator.generate(spec, output)
    }
}

// Register the task
tasks.register<GenerateApiClientTask>("generateApiClient") {
    specFile.set(file("api/openapi.yaml"))
    outputDir.set(layout.buildDirectory.dir("generated/api"))
    packageName.set("com.example.api")
}

// Wire into build pipeline
tasks.named("compileKotlin") {
    dependsOn("generateApiClient")
}
```

### Build Script Plugins

For simpler automation, use script plugins:

```kotlin
// buildSrc/src/main/kotlin/api-generation.gradle.kts
plugins {
    `kotlin-dsl`
}

tasks.register<GenerateApiClientTask>("generateApiClient") {
    group = "code generation"
    description = "Generates API client from OpenAPI specification"
}

// Apply in module build.gradle.kts
plugins {
    id("api-generation")
}
```

### Precompiled Script Plugins

More sophisticated plugins use the `buildSrc` or `build-logic` pattern:

```kotlin
// build-logic/convention-plugins/src/main/kotlin/kmp-library-convention.gradle.kts
plugins {
    kotlin("multiplatform")
    id("maven-publish")
}

kotlin {
    // Configure all common KMP targets
    androidTarget()
    iosX64()
    iosArm64()
    iosSimulatorArm64()
    
    // Standard source set configuration
    sourceSets {
        val commonMain by getting
        val commonTest by getting {
            dependencies {
                implementation(kotlin("test"))
            }
        }
    }
    
    // Configure publishing for all platforms
    publishing {
        publications.withType<MavenPublication>().configureEach {
            pom {
                name.set(project.name)
                description.set("A Kotlin Multiplatform library")
                licenses {
                    license {
                        name.set("Apache-2.0")
                        url.set("https://www.apache.org/licenses/LICENSE-2.0")
                    }
                }
            }
        }
    }
}
```

### Binary Plugins

For reusable plugins across projects, create standalone plugin projects:

```kotlin
// Plugin class
class KmpConventionPlugin : Plugin<Project> {
    override fun apply(project: Project) {
        project.plugins.apply("org.jetbrains.kotlin.multiplatform")
        
        project.extensions.configure<KotlinMultiplatformExtension> {
            androidTarget()
            iosArm64()
            iosSimulatorArm64()
            
            sourceSets {
                commonMain.dependencies {
                    implementation("org.jetbrains.kotlinx:kotlinx-coroutines-core:1.9.0")
                }
            }
        }
        
        // Add custom tasks
        project.tasks.register("validateKmpSetup") {
            doLast {
                check(project.hasProperty("android")) {
                    "Android SDK not configured. Add 'android' block to your build file."
                }
            }
        }
    }
}
```

### Real-World Automation Examples

```kotlin
// Task: Validate module structure
tasks.register("validateModuleStructure") {
    doLast {
        val requiredDirs = listOf(
            "src/commonMain/kotlin",
            "src/commonTest/kotlin",
            "src/androidMain/kotlin",
            "src/iosMain/kotlin"
        )
        
        requiredDirs.forEach { dir ->
            if (!file(dir).exists()) {
                logger.warn("Missing directory: $dir")
            }
        }
    }
}

// Task: Generate changelog
tasks.register<Exec>("generateChangelog") {
    group = "documentation"
    commandLine("git", "log", "--pretty=format:- %s", "$(git describe --tags --abbrev=0)..HEAD")
    standardOutput = file("CHANGELOG.md").outputStream()
}

// Task: Run quality checks
tasks.register("qualityCheck") {
    group = "verification"
    dependsOn("test", "detekt", "ktlintCheck")
    description = "Runs all quality checks including tests, linting, and static analysis"
}
```

### Real-World Relevance

Custom Gradle automation transforms manual, error-prone processes into reliable, versioned code:
- **API evolution**: Auto-generate clients from backend specs
- **Release management**: Automate versioning, tagging, and changelog generation
- **Quality gates**: Enforce code standards before CI/CD
- **Documentation**: Generate docs, API references, and reports
- **Developer experience**: Simplify complex workflows into single commands

For KMP teams, custom plugins enforce consistency across modules, automate platform-specific setup, and reduce boilerplate—enabling developers to focus on features rather than configuration.
