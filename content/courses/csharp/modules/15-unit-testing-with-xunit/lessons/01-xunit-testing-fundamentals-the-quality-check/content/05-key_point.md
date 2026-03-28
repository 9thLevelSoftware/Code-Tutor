---
type: "KEY_POINT"
title: "xUnit Testing Fundamentals"
---

## Key Takeaways

- **`[Fact]` marks a test, `[Theory]` marks parameterized tests** -- use `[Fact]` for single-scenario tests. Use `[Theory]` with `[InlineData]` to run the same test with multiple inputs efficiently.

- **`Assert.Equal(expected, actual)` -- expected comes first** -- wrong parameter order produces confusing failure messages. Read as: "I expect 5, the actual result was 3."

- **Name tests with `Method_Scenario_Expected` convention** -- `Add_TwoPositiveNumbers_ReturnsSum` documents the behavior. Clear names make test failures immediately understandable without reading the test code.

## See Also

- **Prerequisites**: Methods and return values (M06) - tests are just methods that verify other methods
- **Next Step**: Mocking dependencies (M15 L02) for testing classes that rely on external services
- **Related**: Interfaces (M07 L04) make testing easier by allowing mock implementations
- **Integration**: Integration testing (M15 L03) builds on these fundamentals for testing complete workflows

## See Also

- **Prerequisites**: Methods and return values (M06) - tests are just methods that verify other methods
- **Next Step**: Mocking dependencies (M15 L02) for testing classes that rely on external services
- **Related**: Interfaces (M07 L04) make testing easier by allowing mock implementations
- **Integration**: Integration testing (M15 L03) builds on these fundamentals for testing complete workflows
