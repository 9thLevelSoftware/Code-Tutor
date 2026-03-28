---
type: "WARNING"
title: "Common Constructor Mistakes and How to Avoid Them"
---

**Mistake 1: Calling Secondary Constructor Before Primary Init**

Remember: init blocks always run before secondary constructor bodies. Don't rely on values set in secondary constructors within init blocks.

```kotlin
class Wrong {
    val value: String
    
    init {
        // ❌ This will fail - 'extra' isn't set yet!
        // value = extra.uppercase()  // DON'T DO THIS
    }
    
    constructor(extra: String) {
        // This runs AFTER init block
    }
}
```

**Correct approach** — Set all necessary values in the primary constructor or compute them within the init block using available parameters.

---

**Mistake 2: Infinite Delegation Loops**

Secondary constructors must delegate to the primary constructor (or another secondary constructor that eventually does). Don't create circular delegation:

```kotlin
class Bad {
    // ❌ WRONG: This creates an infinite loop
    constructor() : this(1)  // Calls constructor(Int)
    constructor(x: Int) : this()  // Calls constructor() - INFINITE LOOP!
}
```

**Correct approach** — Always ensure the delegation chain ends at the primary constructor:

```kotlin
class Good {
    constructor(x: Int, y: String)  // Primary
    constructor(x: Int) : this(x, "default")  // Delegates to primary
    constructor() : this(0)  // Delegates to secondary that reaches primary
}
```

---

**Mistake 3: Accessing Uninitialized Lateinit Properties in Init**

`lateinit` properties cannot be accessed in init blocks—they're not initialized yet:

```kotlin
class Dangerous {
    lateinit var config: String
    
    init {
        // ❌ CRASH: lateinit property not initialized
        // println(config.length)  // DON'T DO THIS
    }
}
```

**Correct approach** — Use nullable types or initialize in the constructor:

```kotlin
class Safe(val config: String)  // Initialized in constructor
// OR
class AlsoSafe {
    var config: String? = null  // Nullable with default
}
```

---

**Mistake 4: Forgetting to Validate in Init Blocks**

Don't skip validation in init blocks. Invalid objects should fail fast at construction time, not later when used:

```kotlin
// ❌ WRONG: Allows invalid state
class BankAccount(val balance: Double)  // Negative balance allowed!

// ✅ CORRECT: Validates immediately
class BankAccount(val balance: Double) {
    init {
        require(balance >= 0) { "Balance cannot be negative" }
    }
}
```

---

**Mistake 5: Heavy Operations in Primary Constructor**

Avoid expensive operations (database calls, network requests) in constructors:

```kotlin
// ❌ WRONG: Blocks during construction
class User(val id: String) {
    val profile: Profile = fetchFromNetwork(id)  // DON'T!
}

// ✅ CORRECT: Use lazy initialization or factory methods
class User(val id: String) {
    val profile: Profile by lazy { fetchFromNetwork(id) }
}
```
