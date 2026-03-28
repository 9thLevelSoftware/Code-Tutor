---
type: "THEORY"
title: "Understanding Delegation Patterns"
---

**Estimated Time**: 65 minutes
**Difficulty**: Advanced
**Prerequisites**: Object-oriented programming fundamentals, interfaces

---

Delegation is one of the most powerful design patterns in software engineering, and Kotlin provides first-class language support for it. At its core, **delegation** means handing off responsibility for certain operations to another object—the *delegate*—rather than handling them yourself.

**Why Delegation Matters**

In traditional OOP, you typically have two ways to reuse code: inheritance (extending a base class) or composition (containing an instance). Inheritance is rigid—Kotlin classes can only extend one parent. Composition is flexible but verbose—you must manually forward every method call to the contained object.

Delegation gives you the best of both worlds: the flexibility of composition with the convenience of inheritance.

**Class Delegation with `by`**

Kotlin's class delegation allows you to implement an interface by delegating all its methods to another object:

```kotlin
interface Printer {
    fun print(message: String)
    fun println(message: String)
}

class ConsolePrinter : Printer {
    override fun print(message: String) = kotlin.io.print(message)
    override fun println(message: String) = kotlin.io.println(message)
}

class LoggingPrinter(printer: Printer) : Printer by printer {
    override fun print(message: String) {
        // Custom behavior
        kotlin.io.print("[LOG] ")
        // Delegated behavior happens automatically
    }
}
```

The `by printer` clause tells Kotlin: "Forward all Printer interface methods to the `printer` object, unless I override them."

**Property Delegation**

Beyond class delegation, Kotlin supports property delegation—controlling how properties are stored, accessed, and modified:

```kotlin
val lazyValue: String by lazy {
    println("Computed!")
    "Hello"
}

var observed: String by Delegates.observable("") { prop, old, new ->
    println("$old -> $new")
}
```

Common property delegates include:
- `lazy` — Initialize on first access
- `observable` — React to changes
- `vetoable` — Validate changes
- `notNull` — Fail if accessed before assignment

**Real-World Relevance**

Delegation patterns appear everywhere in professional Kotlin code:
- **Android**: View binding delegates, ViewModel delegation
- **Ktor**: Request/response pipeline delegation
- **Architecture**: Decorator patterns, proxy patterns, logging wrappers
- **Testing**: Mock objects using delegation

Mastering delegation lets you write cleaner, more maintainable code with less boilerplate and more flexibility.
