---
type: "KEY_POINT"
title: "Inheritance Fundamentals"
---

## Key Takeaways

- **The colon `:` means "inherits from"** -- `class Car : Vehicle` gives Car all of Vehicle's members automatically. The derived class (Car) extends the base class (Vehicle).

- **C# supports single inheritance only** -- a class can inherit from one base class but implement multiple interfaces. Chain inheritance for deeper hierarchies: `Vehicle -> Car -> SportsCar`.

- **Use inheritance for "is-a" relationships** -- a Car IS-A Vehicle (correct). A Car IS-A Engine (wrong -- use composition instead). If the relationship does not feel natural, inheritance is the wrong tool.

## See Also

- **Next Steps**: Dive deeper into polymorphism with virtual/override in the next lesson
- **Related Concepts**: Interfaces (M07 L04) provide another way to share capabilities across unrelated classes
- **Advanced OOP**: Learn about exception handling and namespaces in M08 for production-ready code

## See Also

- **Next Steps**: Dive deeper into polymorphism with virtual/override in the next lesson
- **Related Concepts**: Interfaces (M07 L04) provide another way to share capabilities across unrelated classes
- **Advanced OOP**: Learn about exception handling and namespaces in M08 for production-ready code
