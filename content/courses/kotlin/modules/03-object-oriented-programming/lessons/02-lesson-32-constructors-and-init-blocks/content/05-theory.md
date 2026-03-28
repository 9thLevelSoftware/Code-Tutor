---
type: "THEORY"
title: "Secondary Constructors: When You Need Alternatives"
---

**What are Secondary Constructors?**

While the primary constructor handles most scenarios, **secondary constructors** provide alternative ways to create objects. They're defined in the class body using the `constructor` keyword and are useful when:

- You need multiple initialization patterns
- Converting from one type to another
- Providing convenience constructors with fewer parameters
- Calling super class constructors with different arguments

**Syntax and Delegation**

Every secondary constructor must eventually delegate to the primary constructor (or another secondary constructor that does). This ensures the primary initialization path is always executed:

```kotlin
class Person(val name: String, val age: Int) {
    // Primary constructor handles the main properties
    
    // Secondary constructor for name-only initialization
    constructor(name: String) : this(name, 0) {
        println("Created person with default age")
    }
    
    // Secondary constructor for copying from another person
    constructor(other: Person) : this(other.name, other.age) {
        println("Copied from another person")
    }
}
```

**Execution Order**

When using a secondary constructor:
1. The primary constructor's init block runs first
2. Then the secondary constructor's body executes
3. This guarantees consistent initialization

**Real-World Use Cases**

Secondary constructors shine in scenarios like:
- **Deserialization**: Creating objects from JSON or database records
- **Convenience constructors**: Simplified object creation for common cases
- **Interoperability**: Working with Java frameworks that expect specific constructors
- **Inheritance**: Calling parent class constructors with different arguments

**Best Practices**

Prefer the primary constructor when possible—it keeps your class definition cleaner. Use secondary constructors sparingly for genuinely different initialization patterns. If you find yourself writing many secondary constructors, consider using factory methods or the Builder pattern instead.

**Real-World Relevance**

In Android development, secondary constructors are common when extending View classes. In backend development, they help when deserializing data from multiple sources (JSON, XML, database rows). Understanding when to use them keeps your APIs flexible yet maintainable.
