---
type: "THEORY"
title: "Introduction"
---

**Estimated Time**: 45 minutes

GitHub Actions is a powerful CI/CD platform that integrates seamlessly with your repository. For KMP projects, it enables automated testing, building, and deployment across all target platforms.

**Understanding GitHub Actions for KMP**

Key considerations for KMP:
- **Multi-platform runners**: Ubuntu for Android/JVM, macOS for iOS builds
- **Matrix builds**: Test multiple OS versions simultaneously
- **Caching**: Essential for Gradle and Kotlin/Native compilation speed

**Complete KMP Workflow**

```yaml
# .github/workflows/kmp-ci.yml
name: KMP CI

on:
  push:
    branches: [main]
  pull_request:
    branches: [main]

jobs:
  # Fast feedback - common tests
  common-tests:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - uses: actions/setup-java@v4
        with:
          java-version: '21'
          distribution: 'temurin'
      
      - name: Cache Gradle
        uses: actions/cache@v3
        with:
          path: |
            ~/.gradle/caches
            ~/.gradle/wrapper
          key: ${{ runner.os }}-gradle-${{ hashFiles('**/*.gradle*') }}
      
      - name: Run common tests
        run: ./gradlew :shared:commonTest

  # Android tests
  android-tests:
    runs-on: ubuntu-latest
    needs: common-tests
    steps:
      - uses: actions/checkout@v4
      - uses: actions/setup-java@v4
        with:
          java-version: '21'
          distribution: 'temurin'
      
      - name: Run Android unit tests
        run: ./gradlew :shared:androidUnitTest
      
      - name: Build Android app
        run: ./gradlew :androidApp:assembleDebug

  # iOS tests (requires macOS)
  ios-tests:
    runs-on: macos-latest
    needs: common-tests
    steps:
      - uses: actions/checkout@v4
      - uses: actions/setup-java@v4
        with:
          java-version: '21'
          distribution: 'temurin'
      
      - uses: maxim-lobanov/setup-xcode@v1
        with:
          xcode-version: '15.0'
      
      - name: Run iOS tests
        run: ./gradlew :shared:iosX64Test
      
      - name: Build iOS framework
        run: ./gradlew :shared:linkDebugFrameworkIosX64

  # Desktop tests
  desktop-tests:
    strategy:
      matrix:
        os: [ubuntu-latest, macos-latest, windows-latest]
    runs-on: ${{ matrix.os }}
    needs: common-tests
    steps:
      - uses: actions/checkout@v4
      - uses: actions/setup-java@v4
        with:
          java-version: '21'
          distribution: 'temurin'
      
      - name: Run JVM tests
        run: ./gradlew :shared:jvmTest
```

**Advanced Patterns**

```yaml
# Conditional jobs
- name: Deploy to TestFlight
  if: github.ref == 'refs/heads/main' && github.event_name == 'push'
  env:
    APP_STORE_CONNECT_API_KEY: ${{ secrets.APP_STORE_KEY }}
  run: fastlane beta

# Artifact upload
- name: Upload artifacts
  uses: actions/upload-artifact@v4
  with:
    name: ios-build
    path: iosApp/build/
```

**Best Practices**

1. Use macOS runners sparingly - they're more expensive
2. Cache aggressively: Gradle, CocoaPods, Konan
3. Run common tests first for fast feedback
4. Parallelize platform-specific tests
5. Use concurrency groups to prevent redundant builds

GitHub Actions provides a complete CI/CD solution for KMP projects, from commit to deployment.
