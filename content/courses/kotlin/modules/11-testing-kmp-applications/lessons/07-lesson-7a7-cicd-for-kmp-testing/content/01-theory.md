---
type: "THEORY"
title: "Introduction"
---

**Estimated Time**: 55 minutes

Setting up CI/CD for Kotlin Multiplatform projects requires orchestrating tests across multiple platforms. A well-configured pipeline ensures code quality while providing fast feedback to developers.

**KMP CI/CD Overview**

Your pipeline should test:
- **Common tests** (run on all platforms)
- **Platform-specific tests** (JVM, Android, iOS, JS, Native)
- **Linting** (detekt, ktlint)
- **Build verification** (all targets compile)

**GitHub Actions Workflow**

```yaml
# .github/workflows/test.yml
name: KMP Tests

on: [push, pull_request]

jobs:
  common-tests:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - uses: actions/setup-java@v4
        with:
          java-version: '21'
          distribution: 'temurin'
      
      - name: Run common tests
        run: ./gradlew commonTest

  android-tests:
    runs-on: macos-latest
    steps:
      - uses: actions/checkout@v4
      - name: Setup Android SDK
        uses: android-actions/setup-android@v3
      
      - name: Run Android tests
        uses: reactivecircus/android-emulator-runner@v2
        with:
          api-level: 34
          script: ./gradlew connectedCheck

  ios-tests:
    runs-on: macos-latest
    steps:
      - uses: actions/checkout@v4
      - uses: maxim-lobanov/setup-xcode@v1
        with:
          xcode-version: '15.0'
      
      - name: Run iOS tests
        run: ./gradlew iosX64Test

  lint:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - name: Run Detekt
        run: ./gradlew detekt
```

**Gradle Test Tasks**

Understanding KMP test tasks:

```bash
# Run all tests across all platforms
./gradlew allTests

# Run specific platform tests
./gradlew jvmTest          # JVM target
./gradlew jsTest           # JavaScript target
./gradlew iosX64Test       # iOS simulator
./gradlew androidUnitTest  # Android unit tests
./gradlew connectedCheck   # Android instrumented tests
```

**Test Reporting**

Generate consolidated reports:

```kotlin
// build.gradle.kts
tasks.withType<Test> {
    reports {
        junitXml.required.set(true)
        html.required.set(true)
    }
}
```

**Caching for Faster CI**

```yaml
- uses: actions/cache@v3
  with:
    path: |
      ~/.gradle/caches
      ~/.gradle/wrapper
    key: ${{ runner.os }}-gradle-${{ hashFiles('**/*.gradle*', '**/gradle-wrapper.properties') }}
    restore-keys: |
      ${{ runner.os }}-gradle-
```

**Best Practices**

1. Run common tests first - they're fastest and catch most issues
2. Use macOS runners for iOS tests (requires Xcode)
3. Cache Gradle dependencies aggressively
4. Parallelize platform tests when possible
5. Fail fast - cancel other jobs when one fails
6. Store test artifacts for debugging

A robust CI/CD pipeline ensures your KMP code works reliably across all target platforms before it reaches production.