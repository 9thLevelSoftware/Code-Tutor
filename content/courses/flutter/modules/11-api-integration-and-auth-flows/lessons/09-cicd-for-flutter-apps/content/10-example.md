---
type: "EXAMPLE"
title: "Complete Example: Production-Ready CI/CD"
---


### Project Structure


### ci.yml (Runs on every PR)




```yaml
name: CI

on:
  pull_request:
    branches: [ main ]
  push:
    branches: [ main ]

jobs:
  ci:
    runs-on: ubuntu-latest
    timeout-minutes: 20
    steps:
      - uses: actions/checkout@v4

      - uses: subosito/flutter-action@v2
        with:
          flutter-version: '3.38.0'
          cache: true

      - name: Install dependencies
        run: flutter pub get

      - name: Check formatting
        run: dart format --set-exit-if-changed .

      - name: Analyze code
        run: flutter analyze --fatal-infos

      - name: Run tests with coverage
        run: flutter test --coverage --no-test-assets

      - name: Check coverage
        run: |
          sudo apt-get install -y lcov
          lcov --remove coverage/lcov.info '*.g.dart' -o coverage/lcov.info
          COVERAGE=$(lcov --summary coverage/lcov.info 2>&1 | grep 'lines' | grep -oP '\d+\.\d+(?=%)')
          echo "Coverage: ${COVERAGE}%"
          if (( $(echo "$COVERAGE < 70" | bc -l) )); then exit 1; fi

      - name: Build APK
        run: flutter build apk --debug
```
