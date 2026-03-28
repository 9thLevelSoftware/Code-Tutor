---
type: "KEY_POINT"
title: "Primary Constructors for Concise Classes"
---

## Key Takeaways

- **Primary constructor parameters go after the class name** -- `class Person(string name, int age)` captures parameters for use throughout the class. No separate constructor body needed.

- **Parameters are captured, not automatically properties** -- to expose them publicly, declare a property: `public string Name { get; } = name;`. Without this, they are only accessible internally.

- **Validate with field initializers** -- `private readonly string _name = name ?? throw new ArgumentNullException(nameof(name));` ensures parameters are validated at construction time.

- **C# 13 `field` keyword eliminates backing field declarations** -- use `get => field; set => field = value;` in properties to access the compiler-generated field directly, reducing boilerplate when combining with primary constructors.

- **C# 13 is stable with .NET 9** -- the `field` keyword and primary constructor enhancements are production-ready features in C# 13, no preview flags needed.
