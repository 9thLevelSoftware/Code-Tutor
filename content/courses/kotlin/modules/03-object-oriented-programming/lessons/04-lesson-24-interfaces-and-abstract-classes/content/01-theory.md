---
type: "THEORY"
title: "Interfaces and Abstract Classes in Kotlin"
---

**Estimated Time**: 55 minutes
**Difficulty**: Intermediate

## Introduction

Interfaces and abstract classes define contracts that concrete classes must fulfill. Understanding when to use each—and how Kotlin's approach differs from Java—is essential for designing clean, testable architectures.

**Learning Objectives**:
- Define interfaces with `interface` and implement them with `:`
- Create abstract classes with `abstract` methods and properties
- Use interface delegation to avoid boilerplate
- Choose between interfaces, abstract classes, and concrete classes

## Interfaces vs. Abstract Classes

| Feature | Interface | Abstract Class |
|---------|-----------|----------------|
| Multiple inheritance | ✅ Yes | ❌ No |
| State (fields) | ❌ No | ✅ Yes |
| Constructor | ❌ No | ✅ Yes |
| Method implementations | ✅ Default methods | ✅ Concrete methods |

## Interface Delegation

Kotlin's `by` keyword eliminates delegation boilerplate:

```kotlin
interface Repository {
    fun get(id: String): User
    fun save(user: User)
}

class CachedRepository(
    private val delegate: Repository
) : Repository by delegate {
    private val cache = mutableMapOf<String, User>()
    
    override fun get(id: String): User {
        return cache.getOrPut(id) { delegate.get(id) }
    }
}
```

## When to Use What

**Use interfaces when**:
- Defining capabilities (Comparable, Serializable)
- Multiple inheritance is needed
- Creating testable, mockable contracts

**Use abstract classes when**:
- Sharing implementation code
- Defining a template method pattern
- Providing common state/constructor logic

## Real-World Context

The Repository pattern in Android Architecture Components uses interfaces for testability. Spring Data JPA generates implementations from interfaces. The Strategy pattern relies on interchangeable interface implementations.

Choosing the right abstraction leads to code that's easier to test, extend, and maintain. It has proper frontmatter so the loader will not fail to parse it.
