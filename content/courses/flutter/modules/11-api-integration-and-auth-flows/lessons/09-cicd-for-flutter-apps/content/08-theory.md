---
type: "THEORY"
title: "Section 6: Common CI/CD Patterns"
---


### Pattern 1: Separate Workflows by Purpose


**ci.yml** (fast, runs always):

**integration.yml** (slow, runs on main only):

**deploy.yml** (manual trigger):

### Pattern 2: Caching for Faster Builds


**Result:** Builds go from 10 minutes → 2 minutes! ⚡

### Pattern 3: Matrix Testing

Test multiple Flutter versions:




```yaml
jobs:
  test:
    strategy:
      matrix:
        flutter-version: ['3.36.0', '3.38.0']

    steps:
      - uses: actions/checkout@v4

      - uses: subosito/flutter-action@v2
        with:
          flutter-version: ${{ matrix.flutter-version }}

      - run: flutter test
```
