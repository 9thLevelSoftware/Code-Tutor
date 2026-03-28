---
type: "KEY_POINT"
title: "Classes as Blueprints"
---

## Key Takeaways

- **A class is a blueprint, an object is an instance** — `class Player { }` defines the template containing fields, properties, and methods. `new Player()` creates a concrete object in memory with its own data storage. You can instantiate hundreds of objects from a single class definition, each with independent state.

- **Each object has independent data** — `alice.Name` and `bob.Name` are separate memory locations. Changing one object's properties does not affect other objects created from the same class. This encapsulation isolates state and prevents unintended side effects between object instances.

- **Use PascalCase for class names** — `Player`, `ShoppingCart`, `OrderProcessor`. This C# convention distinguishes types from variables (which use camelCase). Consistent naming improves code readability and follows established .NET conventions that other developers expect.

- **The `new` keyword triggers constructor execution** — when you write `new Player("Alice")`, C# allocates heap memory, initializes fields to default values, then invokes the constructor to set up the object's initial state. Understanding this two-phase construction helps debug initialization issues.

- **Reference types vs value types matter** — classes are reference types: assignment copies the reference, not the object. Two variables can point to the same object. structs are value types: assignment copies the entire value. This distinction affects equality comparisons, method parameter passing, and memory layout.

- **Design classes around behavior, not just data** — a well-designed class encapsulates both state (what it knows) and behavior (what it does). Avoid anemic data-only classes that expose all fields publicly. Rich domain models with methods that enforce invariants create more maintainable applications.
