---
type: "THEORY"
title: "Introduction"
---

**Estimated Time**: 50 minutes

Continuous Integration and Deployment (CI/CD) for KMP projects requires orchestrating builds across multiple platforms. A robust pipeline automates testing, building, and distribution to ensure reliable releases.

**KMP CI/CD Pipeline Overview**

```
┌─────────────┐     ┌──────────────┐     ┌─────────────┐
│   Commit    │ --> │  Build & Test  │ --> │   Deploy    │
│             │     │              │     │             │
│ • Lint      │     │ • Common     │     │ • Android   │
│ • Format    │     │ • Android    │     │ • iOS       │
│ • Security  │     │ • iOS        │     │ • Web       │
└─────────────┘     └──────────────┘     └─────────────┘
```

**Complete GitHub Actions Workflow**

```yaml
# .github/workflows/ci.yml
name: CI/CD

on:
  push:
    branches: [main, develop]
  pull_request:
    branches: [main]

jobs:
  validate:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - uses: actions/setup-java@v4
        with:
          java-version: '21'
          distribution: 'temurin'
      
      - name: Setup Gradle
        uses: gradle/gradle-build-action@v3
      
      - name: Lint
        run: ./gradlew detekt ktlintCheck
      
      - name: Security scan
        run: ./gradlew dependencyCheckAnalyze

  test:
    strategy:
      matrix:
        os: [ubuntu-latest, macos-latest]
    runs-on: ${{ matrix.os }}
    steps:
      - uses: actions/checkout@v4
      - name: Run tests
        run: ./gradlew allTests
      
      - name: Upload results
        uses: actions/upload-artifact@v4
        with:
          name: test-results-${{ matrix.os }}
          path: '**/build/reports/tests/'

  build-android:
    needs: [validate, test]
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - name: Build Android
        run: ./gradlew :androidApp:assembleRelease
      - name: Upload APK
        uses: actions/upload-artifact@v4
        with:
          name: android-release
          path: androidApp/build/outputs/apk/release/*.apk

  build-ios:
    needs: [validate, test]
    runs-on: macos-latest
    steps:
      - uses: actions/checkout@v4
      - uses: maxim-lobanov/setup-xcode@v1
        with:
          xcode-version: '15.0'
      - name: Build iOS
        run: ./gradlew :iosApp:assembleRelease
```

**Gradle Configuration for CI**

```kotlin
// build.gradle.kts
plugins {
    id("org.jetbrains.kotlinx.kover") version "0.7.0"
}

tasks.withType<Test> {
    useJUnitPlatform()
    testLogging {
        events("passed", "skipped", "failed")
    }
}

// Coverage aggregation
kover {
    filters {
        classes {
            excludes += listOf("*.di.*", "*Test", "*Mock*")
        }
    }
}
```

**Environment Management**

```yaml
# Staging deployment
- name: Deploy to Play Console Internal
  if: github.ref == 'refs/heads/develop'
  env:
    SERVICE_ACCOUNT_JSON: ${{ secrets.PLAY_STORE_JSON }}
  run: ./gradlew publishToPlayStoreInternal

# Production deployment
- name: Deploy to Production
  if: github.ref == 'refs/heads/main'
  needs: [staging-test]
  run: ./gradlew publishToPlayStoreProduction
```

**Best Practices**

1. Fail fast - lint and quick tests first
2. Parallelize platform builds when possible
3. Cache Gradle and Xcode dependencies aggressively
4. Store secrets in GitHub Secrets or similar vaults
5. Tag releases automatically from main branch
6. Maintain separate pipelines for PRs and releases

A well-designed CI/CD pipeline turns deployment from a scary manual process into a boring, reliable routine.
