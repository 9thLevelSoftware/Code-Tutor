---
type: "THEORY"
title: "CI/CD for Flutter Apps"
---

## What is CI/CD?

CI/CD stands for **Continuous Integration** and **Continuous Deployment**—automated pipelines that build, test, and deploy your code. CI merges code changes frequently into a shared repository, running automated tests to catch bugs early. CD automates the release process, delivering working builds to testers or app stores automatically.

## Why CI/CD Matters for Flutter Apps

Flutter apps target multiple platforms (iOS, Android, Web, Desktop). Manually building and testing for each platform is time-consuming and error-prone. CI/CD ensures:

- **Consistent builds** across all platforms
- **Early bug detection** through automated testing
- **Faster releases** without manual intervention
- **Quality gates** that prevent broken code from reaching production

## Key CI/CD Tools for Flutter

**GitHub Actions** is popular for open-source projects—free for public repos and integrates seamlessly with GitHub workflows. You define workflows in YAML files stored in `.github/workflows/`.

**Codemagic** is built specifically for Flutter, requiring minimal configuration. It handles code signing, Flutter versioning, and deployment to both app stores with simple UI configuration.

**Bitrise** offers visual workflow editors and hundreds of integrations, making it ideal for teams preferring low-code pipeline setup.

## Best Practices for Flutter CI/CD

1. **Run tests on every PR**: Unit, widget, and integration tests should all pass before merging.
2. **Enforce code quality**: Include static analysis (`flutter analyze`) and formatting checks (`dart format --set-exit-if-changed`).
3. **Generate coverage reports**: Track test coverage trends and fail builds if coverage drops below thresholds.
4. **Cache dependencies**: Store `pub` dependencies between runs to speed up builds.
5. **Build for all targets**: iOS (if macOS runner available), Android APK/AAB, and Web.
6. **Automate deployments**: Upload beta builds to TestFlight and Google Play Internal Testing automatically.

## Example GitHub Actions Workflow

```yaml
name: Flutter CI

on: [push, pull_request]

jobs:
  test:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - uses: subosito/flutter-action@v2
        with:
          flutter-version: '3.24.0'
      - run: flutter pub get
      - run: flutter analyze
      - run: flutter test --coverage
      - run: flutter build apk --release
```

This workflow runs on every push and PR, ensuring code quality and producing a release APK. By automating these checks, you maintain confidence that your app works correctly across all changes.

