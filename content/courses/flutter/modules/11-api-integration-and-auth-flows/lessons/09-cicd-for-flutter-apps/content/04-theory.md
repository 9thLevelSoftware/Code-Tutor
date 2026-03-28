---
type: "THEORY"
title: "Section 2: Setting Up GitHub Actions"
---


### Step 1: Create Workflow File

GitHub Actions workflows live in `.github/workflows/`.


### Step 2: Basic Flutter CI Workflow

Create `.github/workflows/flutter_ci.yml`:


### Step 3: Commit and Push


### Step 4: View Results

1. Go to your GitHub repository
2. Click "Actions" tab
3. See your workflow running!
4. ✅ Green checkmark = all passed
5. ❌ Red X = something failed

### Advanced: Multi-Platform CI

Test on Linux, macOS, and Windows:




```yaml
name: Flutter CI (Multi-Platform)

on:
  push:
    branches: [ main ]
  pull_request:
    branches: [ main ]

jobs:
  test:
    name: Test on ${{ matrix.os }}
    runs-on: ${{ matrix.os }}
    strategy:
      matrix:
        os: [ubuntu-latest, macos-latest, windows-latest]

    steps:
      - uses: actions/checkout@v4

      - uses: subosito/flutter-action@v2
        with:
          flutter-version: '3.38.0'
          channel: 'stable'

      - name: Install dependencies
        run: flutter pub get

      - name: Run tests
        run: flutter test

      - name: Build
        run: |
          if [ "$RUNNER_OS" == "Linux" ]; then
            flutter build apk --debug
          elif [ "$RUNNER_OS" == "macOS" ]; then
            flutter build ios --no-codesign
          elif [ "$RUNNER_OS" == "Windows" ]; then
            flutter build windows
          fi
        shell: bash
```
