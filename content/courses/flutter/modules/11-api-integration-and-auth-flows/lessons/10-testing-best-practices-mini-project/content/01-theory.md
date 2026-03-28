---
type: "THEORY"
title: "Testing Best Practices for Production Apps"
---

## The Testing Pyramid in Flutter

Effective testing follows a pyramid structure: many fast unit tests at the base, fewer widget tests in the middle, and a small number of integration tests at the top. This balance provides maximum confidence with minimal execution time.

**Unit tests** verify individual functions, classes, and business logic. They're fast (milliseconds), isolated, and catch logic errors early. Use the `test` package and mock dependencies with `mockito` or `mocktail`.

**Widget tests** verify UI components render correctly and respond to interactions. They use `tester.pumpWidget()` and `tester.tap()` to simulate user behavior. These are slower than unit tests but faster than integration tests.

**Integration tests** (end-to-end) verify complete user flows across screens. They run on real devices or emulators, ensuring everything works together. Reserve these for critical paths like checkout flows or authentication.

## Test-Driven Development (TDD)

TDD follows a red-green-refactor cycle: write a failing test first, implement code to make it pass, then refactor while keeping tests green. This approach produces testable code, clarifies requirements, and prevents over-engineering. For Flutter apps, start with unit tests for business logic, then add widget tests for UI components.

## Key Testing Best Practices

1. **Test behavior, not implementation**: Assert on outcomes, not internal state. This prevents brittle tests that break during refactoring.

2. **Use descriptive test names**: `test('throws error when email is invalid', ...)` explains intent better than `test('email test', ...)`.

3. **Arrange-Act-Assert pattern**: Structure tests with clear setup, execution, and verification phases for readability.

4. **Keep tests fast and deterministic**: Avoid real network calls or timers. Mock external dependencies and use `pump()` with durations sparingly.

5. **Maintain coverage goals**: Aim for 80%+ coverage, but prioritize testing critical paths over hitting arbitrary percentages.

6. **Run tests in CI/CD**: Execute the full test suite on every pull request to catch regressions before they reach production.

## Example: Testing a Login ViewModel

```dart
group('LoginViewModel', () {
  late LoginViewModel viewModel;
  late MockAuthRepository mockAuth;

  setUp(() {
    mockAuth = MockAuthRepository();
    viewModel = LoginViewModel(mockAuth);
  });

  test('emits error when email is empty', () async {
    // Act
    await viewModel.login('', 'password');

    // Assert
    expect(viewModel.error, equals('Email cannot be empty'));
    verifyNever(mockAuth.login(any, any));
  });
});
```

Following these practices creates a maintainable test suite that catches bugs early, documents behavior, and enables confident refactoring.

