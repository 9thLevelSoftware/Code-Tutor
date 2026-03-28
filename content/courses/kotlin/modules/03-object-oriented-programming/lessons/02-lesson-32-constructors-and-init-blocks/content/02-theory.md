---
type: "THEORY"
title: "Primary Constructors: Declaring and Using"
---

**What is a Primary Constructor?**

In Kotlin, the **primary constructor** is the most concise way to define how objects are created. Unlike Java where constructors are separate methods, Kotlin lets you declare the primary constructor directly in the class header, reducing boilerplate code significantly.

**Syntax and Benefits**

The primary constructor appears right after the class name in parentheses. You can declare properties directly in the constructor, combining parameter declaration with property initialization in a single line:

```kotlin
class User(val name: String, var age: Int)
```

This single line declares:
- A read-only property `name` (val)
- A mutable property `age` (var)
- A constructor that takes both values

**Default Parameters**

Kotlin's primary constructor supports default values, eliminating the need for multiple constructor overloads:

```kotlin
class User(val name: String, var age: Int = 0, val email: String = "")
```

You can now create users flexibly:
- `User("Alice")` — uses defaults for age and email
- `User("Bob", 25)` — uses default for email
- `User("Charlie", 30, "charlie@example.com")` — full specification

**Validation in Primary Constructors**

While the primary constructor is concise, you can still validate inputs using the `init` block, which we'll explore next. The primary constructor sets the foundation, and init blocks handle the logic.

**Real-World Relevance**

Primary constructors are the standard in modern Kotlin code. They appear everywhere—from data classes representing API responses to domain models in business applications. Mastering this syntax makes your code more readable and reduces unnecessary ceremony.
