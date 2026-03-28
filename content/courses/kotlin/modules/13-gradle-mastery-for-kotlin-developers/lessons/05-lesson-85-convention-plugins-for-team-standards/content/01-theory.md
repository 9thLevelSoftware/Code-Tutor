---
type: "THEORY"
title: "Convention Plugins for Team Standards"
---

**Estimated Time**: 90 minutes
**Difficulty**: Advanced
**Prerequisites**: Custom tasks, Gradle plugins, multi-module projects

---

## Enforcing Team Standards with Convention Plugins

As Kotlin Multiplatform projects grow across multiple modules and teams, maintaining consistency becomes critical. Convention plugins encode your team's best practices into reusable, versioned build logic—ensuring every module follows the same standards for dependencies, quality checks, and platform configuration.

### Why Convention Plugins?

Without convention plugins:
```kotlin
// Module A - one configuration
plugins {
    kotlin("multiplatform") version "2.0.0"
}

// Module B - different version
plugins {
    kotlin("multiplatform") version "2.1.0"
}

// Module C - missing critical configuration
plugins {
    kotlin("multiplatform") version "2.0.0"
}
```

With convention plugins:
```kotlin
// Every module uses the same configuration
plugins {
    id("myapp.kotlin.multiplatform")  // Version managed centrally
}
```

### Creating a Convention Plugin Project

```
build-logic/
├── convention-plugins/
│   ├── build.gradle.kts
│   └── src/main/kotlin/
│       ├── kmp-library.convention.gradle.kts
│       ├── kmp-application.convention.gradle.kts
│       ├── quality-checks.convention.gradle.kts
│       └── publishing.convention.gradle.kts
├── settings.gradle.kts
└── gradle.properties
```

### The Library Convention Plugin

```kotlin
// build-logic/convention-plugins/src/main/kotlin/kmp-library.convention.gradle.kts
plugins {
    kotlin("multiplatform")
    id("maven-publish")
    id("signing")
}

kotlin {
    // Standard targets for all library modules
    androidTarget {
        publishLibraryVariants("release")
    }
    
    iosX64()
    iosArm64()
    iosSimulatorArm64()
    
    jvm()
    
    // Common compiler settings
    compilerOptions {
        freeCompilerArgs.add("-Xexpect-actual-classes")
    }
    
    sourceSets {
        commonMain.dependencies {
            // Standard library dependencies
            implementation(libs.kotlinx.coroutines.core)
            implementation(libs.kotlinx.serialization.json)
        }
        
        commonTest.dependencies {
            implementation(kotlin("test"))
            implementation(libs.kotlinx.coroutines.test)
        }
    }
}

// Standard quality tasks
tasks.register<Copy>("validateSources") {
    from("src") {
        include("**/*.kt")
    }
    into(layout.buildDirectory.dir("validated-sources"))
}

// Publishing configuration
publishing {
    publications.withType<MavenPublication>().configureEach {
        pom {
            name.set(provider { "${project.group}:${project.name}" })
            description.set(provider { project.description ?: "A Kotlin Multiplatform library" })
            url.set("https://github.com/your-org/your-repo")
            
            licenses {
                license {
                    name.set("Apache License 2.0")
                    url.set("https://www.apache.org/licenses/LICENSE-2.0.txt")
                }
            }
            
            developers {
                developer {
                    id.set("team")
                    name.set("Your Team")
                    email.set("team@example.com")
                }
            }
            
            scm {
                connection.set("scm:git:git://github.com/your-org/your-repo.git")
                developerConnection.set("scm:git:ssh://github.com:your-org/your-repo.git")
                url.set("https://github.com/your-org/your-repo")
            }
        }
    }
}
```

### The Quality Checks Convention Plugin

```kotlin
// build-logic/convention-plugins/src/main/kotlin/quality-checks.convention.gradle.kts
plugins {
    id("io.gitlab.arturbosch.detekt")
    id("org.jlleitschuh.gradle.ktlint")
    id("org.jetbrains.kotlinx.kover")
}

// Detekt configuration
detekt {
    toolVersion = "1.23.4"
    config.setFrom(files("$rootDir/config/detekt/detekt.yml"))
    buildUponDefaultConfig = true
    allRules = false
    autoCorrect = true
}

tasks.withType<io.gitlab.arturbosch.detekt.Detekt>().configureEach {
    reports {
        html.required.set(true)
        xml.required.set(true)
        txt.required.set(true)
    }
}

// ktlint configuration
ktlint {
    version.set("1.0.1")
    verbose.set(true)
    outputToConsole.set(true)
    filter {
        exclude("**/generated/**")
        include("**/kotlin/**")
    }
}

// Kover coverage configuration
kover {
    filters {
        classes {
            excludes += listOf(
                "*.di.*",
                "*Test",
                "*Mock*",
                "*.BuildConfig"
            )
        }
    }
    
    verify {
        rule {
            bound {
                minValue = 80
                metric = kotlinx.kover.gradle.plugin.dsl.MetricType.LINE
            }
        }
    }
}

// Unified quality task
tasks.register("qualityCheck") {
    group = "verification"
    description = "Runs all quality checks: tests, linting, coverage, and static analysis"
    dependsOn("test", "detekt", "ktlintCheck", "koverVerify")
}
```

### Using Convention Plugins

```kotlin
// settings.gradle.kts (root)
pluginManagement {
    includeBuild("build-logic")
    repositories {
        google()
        mavenCentral()
        gradlePluginPortal()
    }
}

// build.gradle.kts (module)
plugins {
    id("myapp.kotlin.multiplatform")
    id("myapp.quality.checks")
}

// Optional: Add module-specific configuration
kotlin {
    sourceSets {
        commonMain.dependencies {
            // Additional module-specific dependencies
            implementation(libs.ktor.client.core)
        }
    }
}
```

### Benefits of Convention Plugins

| Aspect | Without Conventions | With Conventions |
|--------|---------------------|------------------|
| Consistency | Manual enforcement | Automatic |
| Updates | Modify every module | Single location |
| Onboarding | Document everything | Apply plugin |
| Mistakes | Easy to make | Hard to make |
| Version Alignment | Often divergent | Always synchronized |

### Real-World Relevance

Convention plugins are essential for:
- **Multi-module KMP projects**: Ensure all modules use identical Kotlin versions
- **Team scaling**: New developers follow established patterns automatically
- **CI/CD standardization**: Every module has the same test and quality tasks
- **Security compliance**: Centralized vulnerability scanning and dependency management
- **Open source libraries**: Consistent publishing configuration across artifacts

By encoding standards into build logic, you eliminate "works on my machine" issues, reduce code review friction, and enable teams to focus on features rather than configuration debates.
