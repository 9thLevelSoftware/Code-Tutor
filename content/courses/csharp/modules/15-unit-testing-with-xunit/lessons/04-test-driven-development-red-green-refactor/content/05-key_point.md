---
type: "KEY_POINT"
title: "TDD: Red-Green-Refactor"
---

## Key Takeaways

- **Red: Write a failing test first** -- the test describes the behavior you want. It must fail before you write any implementation code. If it passes immediately, the test is wrong.

- **Green: Write the minimum code to pass** -- resist the urge to gold-plate. Get the test passing with the simplest possible implementation. Correctness first, elegance later.

- **Refactor: Improve with confidence** -- with passing tests as your safety net, clean up duplication, improve naming, and simplify logic. If tests still pass after refactoring, you have not broken anything.

## See Also

- **Prerequisites**: Unit testing (M15 L01) and mocking (M15 L02) provide the foundation for TDD
- **Philosophy**: TDD changes how you design code - interfaces and testability become first-class concerns (M07 L04)
- **Workflow**: The Red-Green-Refactor cycle pairs well with CI/CD automation (M23)
- **Real World**: Many teams combine TDD with integration tests (M15 L03) for comprehensive coverage
- **Advanced**: Clean architecture (M18) principles make TDD easier by naturally isolating testable units
