---
type: "EXAMPLE"
title: "Section 5: Quality Gates and Best Practices"
---


### What are Quality Gates?

**Quality gates** are checks that must pass before code is merged or deployed.

### Essential Quality Gates

1. **Linting** - Code must follow style guidelines
2. **Unit Tests** - All tests must pass
3. **Widget Tests** - UI tests must pass
4. **Coverage** - Minimum coverage threshold
5. **Integration Tests** - Critical flows work
6. **Build Success** - App must build without errors

### Implementing Quality Gates


### Branch Protection Rules

Enforce quality gates in GitHub:

1. Go to **Settings** → **Branches**
2. Add rule for `main` branch
3. Enable:
   - ☑️ Require a pull request before merging
   - ☑️ Require status checks to pass before merging
   - ☑️ Require branches to be up to date before merging
4. Select required checks:
   - ✅ Analyze code
   - ✅ Run tests
   - ✅ Check coverage

Now PRs can't be merged until all checks pass!



```yaml
name: Quality Gates

on:
  pull_request:
    branches: [ main ]

jobs:
  quality-check:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4

      - uses: subosito/flutter-action@v2
        with:
          flutter-version: '3.38.0'

      - name: Install dependencies
        run: flutter pub get

      # Gate 1: Formatting
      - name: Check formatting
        run: dart format --set-exit-if-changed .

      # Gate 2: Linting
      - name: Analyze code
        run: flutter analyze --fatal-infos

      # Gate 3: Unit tests
      - name: Run unit tests
        run: flutter test --exclude-tags=integration

      # Gate 4: Coverage threshold
      - name: Check test coverage
        run: |
          flutter test --coverage
          sudo apt-get install -y lcov
          lcov --remove coverage/lcov.info '*.g.dart' -o coverage/lcov.info

          COVERAGE=$(lcov --summary coverage/lcov.info 2>&1 | \
            grep 'lines......:' | \
            grep -oP '\d+\.\d+(?=%)')

          if (( $(echo "$COVERAGE < 70" | bc -l) )); then
            echo "❌ Coverage ${COVERAGE}% below 70%"
            exit 1
          fi

      # Gate 5: Build success
      - name: Build APK
        run: flutter build apk --debug
```
