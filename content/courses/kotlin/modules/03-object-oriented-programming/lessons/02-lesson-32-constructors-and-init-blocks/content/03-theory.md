---
type: "THEORY"
title: "Understanding Init Blocks"
---

**What is an Init Block?**

The **init block** (short for "initialization block") is where you place logic that runs when an object is created. While the primary constructor handles parameter declaration, init blocks handle setup logic—validation, calculations, logging, or any initialization code that needs to execute.

**Syntax and Execution**

An init block uses the `init` keyword followed by curly braces:

```kotlin
class User(val name: String, var age: Int) {
    init {
        require(name.isNotBlank()) { "Name cannot be blank" }
        require(age >= 0) { "Age cannot be negative" }
        println("User created: $name, age $age")
    }
}
```

Every time you create a `User`, the init block runs automatically, validating the data and printing a message.

**Multiple Init Blocks**

You can have multiple init blocks in a class—they execute in the order they appear:

```kotlin
class Example {
    init {
        println("First init block")
    }
    
    init {
        println("Second init block")
    }
}
```

**When to Use Init Blocks**

Use init blocks when you need to:
- Validate constructor parameters
- Perform calculations to initialize properties
- Set up logging or instrumentation
- Register the object with other components
- Execute any logic that must run during construction

**Execution Order**

Init blocks run after the primary constructor initializes properties but before secondary constructors execute. This predictable order ensures your object's state is valid before any additional construction logic runs.

**Real-World Relevance**

Init blocks are crucial for **defensive programming**. They ensure objects are always in a valid state, preventing invalid data from propagating through your system—a common source of bugs in production applications.
