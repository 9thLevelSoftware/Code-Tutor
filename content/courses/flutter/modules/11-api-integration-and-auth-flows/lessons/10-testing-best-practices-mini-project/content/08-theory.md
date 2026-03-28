---
type: "THEORY"
title: "Section 8: Running and Verifying Tests"
---


### Script: scripts/run_all_tests.sh

Create a comprehensive test runner script that runs all tests in your project:

```bash
#!/bin/bash

# scripts/run_all_tests.sh
# Comprehensive test runner for Flutter projects

set -e  # Exit on any error

echo "========================================"
echo "       Running All Tests"
echo "========================================"
echo ""

# Colors for output
GREEN='\033[0;32m'
RED='\033[0;31m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Track overall status
TESTS_PASSED=0
TESTS_FAILED=0

# Function to run tests with coverage
run_tests() {
    local test_type=$1
    local test_path=$2

    echo -e "${YELLOW}Running $test_type tests...${NC}"

    if flutter test $test_path --coverage; then
        echo -e "${GREEN}✓ $test_type tests passed${NC}"
        ((TESTS_PASSED++))
    else
        echo -e "${RED}✗ $test_type tests failed${NC}"
        ((TESTS_FAILED++))
    fi
    echo ""
}

# Run unit tests
run_tests "Unit" "test/unit/"

# Run widget tests
run_tests "Widget" "test/widget/"

# Run integration tests (if available)
if [ -d "integration_test" ]; then
    echo -e "${YELLOW}Running integration tests...${NC}"
    flutter test integration_test/
    echo ""
fi

# Generate coverage report
echo -e "${YELLOW}Generating coverage report...${NC}"
if command -v lcov &> /dev/null; then
    genhtml coverage/lcov.info -o coverage/html
    echo -e "${GREEN}Coverage report: coverage/html/index.html${NC}"
fi

# Summary
echo "========================================"
echo "           Test Summary"
echo "========================================"
echo -e "Passed: ${GREEN}$TESTS_PASSED${NC}"
echo -e "Failed: ${RED}$TESTS_FAILED${NC}"
echo ""

if [ $TESTS_FAILED -eq 0 ]; then
    echo -e "${GREEN}All tests passed!${NC}"
    exit 0
else
    echo -e "${RED}Some tests failed. Check output above.${NC}"
    exit 1
fi
```

Make it executable:

```bash
chmod +x scripts/run_all_tests.sh
```

Run it:

```bash
./scripts/run_all_tests.sh
```

### Expected Output

When all tests pass, you'll see:

```
========================================
       Running All Tests
========================================

Running Unit tests...
00:05 +45: All tests passed!
✓ Unit tests passed

Running Widget tests...
00:12 +28: All tests passed!
✓ Widget tests passed

Generating coverage report...
Coverage report: coverage/html/index.html

========================================
           Test Summary
========================================
Passed: 2
Failed: 0

All tests passed!
```

### Integrating with CI/CD

Add this to your GitHub Actions workflow:

```yaml
# .github/workflows/test.yml
name: Tests

on: [push, pull_request]

jobs:
  test:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - uses: subosito/flutter-action@v2
        with:
          flutter-version: '3.38.0'

      - name: Install dependencies
        run: flutter pub get

      - name: Run all tests
        run: ./scripts/run_all_tests.sh

      - name: Upload coverage
        uses: codecov/codecov-action@v4
        with:
          files: coverage/lcov.info
```
