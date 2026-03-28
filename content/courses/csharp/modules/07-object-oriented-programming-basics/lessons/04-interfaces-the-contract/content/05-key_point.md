---
type: "KEY_POINT"
title: "Interfaces Define Capabilities"
---

## Key Takeaways

- **Interfaces define WHAT, not HOW** -- `interface IDrawable { void Draw(); }` specifies the contract. The implementing class provides the actual code.

- **A class can implement multiple interfaces** -- `class Button : IDrawable, IClickable` is valid. This is how C# achieves the flexibility that multiple inheritance provides in other languages.

- **Program to interfaces, not implementations** -- use `IList<T>` as parameter types instead of `List<T>`. This makes your code flexible and testable because you can swap implementations.

## See Also

- **Prerequisites**: Understanding classes and inheritance (M06, M07 L01) helps grasp why interfaces are useful
- **Real-World Usage**: LINQ (M09) relies heavily on interfaces like `IEnumerable<T>` and `IQueryable<T>`
- **Testing**: Interfaces are essential for mocking dependencies in unit tests (M15 L02)
- **Design Patterns**: Many patterns in M08 and later modules depend on interface-based design
