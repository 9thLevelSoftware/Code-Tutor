---
type: "KEY_POINT"
title: "Key Takeaways: Constructors and Init Blocks"
---

**Primary Constructor — Your First Choice**

Use the primary constructor for the most common initialization path. Declare properties directly in the constructor to minimize boilerplate code. Kotlin's concise syntax makes this the preferred approach.

```kotlin
// Preferred: Single line for class + constructor + properties
class User(val name: String, val age: Int = 0)
```

---

**Init Blocks — For Logic, Not Structure**

Use `init` blocks for validation, logging, calculations, and setup that must run during object creation. They execute in declaration order, immediately after the primary constructor initializes properties.

```kotlin
init {
    require(age >= 0) { "Age must be non-negative" }
    println("User created: $name")
}
```

---

**Secondary Constructors — For Alternatives**

Use secondary constructors when you need multiple ways to create objects with different parameter sets. Always delegate to the primary constructor to ensure consistent initialization.

```kotlin
constructor(name: String) : this(name, 0)
```

---

**Fail Fast with Validation**

Validate inputs in init blocks to ensure objects are always in a valid state. This prevents invalid data from propagating through your application—a core principle of defensive programming.

---

**Execution Order Matters**

Remember the initialization sequence:
1. Primary constructor property initialization
2. Init blocks (in declaration order)
3. Secondary constructor body

Understanding this order helps you avoid accessing uninitialized properties.

---

**Prefer Primary over Secondary**

Avoid overusing secondary constructors. If you need many variations, consider:
- Default parameters in the primary constructor
- Factory methods (companion object functions)
- Builder patterns for complex objects

These alternatives often produce cleaner, more maintainable code.

---

**Real-World Impact**

Mastering constructors and init blocks ensures your Kotlin applications start with valid, properly configured objects. This reduces null pointer exceptions, invalid state bugs, and makes your codebase more robust and predictable.
