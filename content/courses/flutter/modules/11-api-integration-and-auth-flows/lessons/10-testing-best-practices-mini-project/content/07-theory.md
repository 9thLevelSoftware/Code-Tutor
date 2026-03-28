---
type: "THEORY"
title: "Section 7: CI/CD Setup"
---


### GitHub Actions Workflow

Create `.github/workflows/ci.yml`:




```yaml
name: TaskMaster Pro CI

on:
  pull_request:
    branches: [ main ]
  push:
    branches: [ main ]

jobs:
  test:
    name: Test and Coverage
    runs-on: ubuntu-latest
    timeout-minutes: 20

    steps:
      - name: Checkout code
        uses: actions/checkout@v4

      - name: Setup Flutter
        uses: subosito/flutter-action@v2
        with:
          flutter-version: '3.38.0'
          channel: 'stable'
          cache: true

      - name: Install dependencies
        run: flutter pub get

      - name: Generate Hive adapters
        run: flutter pub run build_runner build --delete-conflicting-outputs

      - name: Verify code formatting
        run: dart format --set-exit-if-changed .

      - name: Analyze code
        run: flutter analyze --fatal-infos

      - name: Run unit and widget tests with coverage
        run: flutter test --coverage --no-test-assets

      - name: Install lcov
        run: sudo apt-get install -y lcov

      - name: Clean coverage data
        run: |
          lcov --remove coverage/lcov.info \
            '*.g.dart' \
            '*.freezed.dart' \
            -o coverage/lcov_cleaned.info

      - name: Check coverage threshold (80%)
        run: |
          COVERAGE=$(lcov --summary coverage/lcov_cleaned.info 2>&1 | \
            grep 'lines......:' | \
            grep -oP '\d+\.\d+(?=%)')

          echo "Coverage: ${COVERAGE}%"

          if (( $(echo "$COVERAGE < 80" | bc -l) )); then
            echo "❌ Coverage ${COVERAGE}% is below 80% threshold"
            exit 1
          else
            echo "✅ Coverage meets 80% threshold"
          fi

      - name: Upload coverage to Codecov
        uses: codecov/codecov-action@v4
        with:
          files: ./coverage/lcov_cleaned.info
          fail_ci_if_error: false

      - name: Generate HTML coverage report
        run: |
          genhtml coverage/lcov_cleaned.info -o coverage/html

      - name: Upload coverage HTML as artifact
        uses: actions/upload-artifact@v4
        with:
          name: coverage-report
          path: coverage/html/

      - name: Build APK
        run: flutter build apk --debug
```
