---
type: "KEY_POINT"
title: "Mocking with Moq"
---

## Key Takeaways

- **`new Mock<IService>()` creates a fake implementation** -- configure return values with `.Setup(x => x.Method()).Returns(value)`. Pass `mock.Object` to the class under test.

- **`mock.Verify()` checks method calls** -- `mock.Verify(x => x.Save(), Times.Once)` asserts that `Save` was called exactly once. Use `Times.Never` to assert a method was not called.

- **Mock interfaces, not concrete classes** -- design your code with interfaces so dependencies can be replaced with mocks. This is why dependency injection and programming to interfaces matter.

## See Also

- **Prerequisites**: Understanding interfaces (M07 L04) is essential for mocking - you can only mock interfaces
- **Foundation**: Unit testing basics (M15 L01) - mocking builds on the `[Fact]` and `[Theory]` patterns
- **Integration**: Integration testing (M15 L03) for when you want to test with real (not mocked) dependencies
- **Related**: Dependency injection patterns in later modules show how to design code for testability
- **Real World**: Entity Framework Core (M12) provides in-memory database options for integration testing

## See Also

- **Prerequisites**: Understanding interfaces (M07 L04) is essential for mocking - you can only mock interfaces
- **Foundation**: Unit testing basics (M15 L01) - mocking builds on the `[Fact]` and `[Theory]` patterns
- **Integration**: Integration testing (M15 L03) for when you want to test with real (not mocked) dependencies
- **Related**: Dependency injection patterns in later modules show how to design code for testability
- **Real World**: Entity Framework Core (M12) provides in-memory database options for integration testing
