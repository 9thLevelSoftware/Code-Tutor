---
type: "THEORY"
title: "Kotlin Properties and Initialization"
---

**Estimated Time**: 55 minutes
**Difficulty**: Beginner

## Introduction

Properties are the foundation of Kotlin's object-oriented design. Unlike Java fields, Kotlin properties combine data storage with accessor logic through getters and setters—providing encapsulation without boilerplate.

**Learning Objectives**:
- Declare mutable (`var`) and read-only (`val`) properties
- Customize property accessors with custom getters/setters
- Use `lateinit` and lazy initialization for deferred setup
- Understand backing fields and property delegation

## Why Properties Matter

In Java, you write getters and setters manually:
```java
private String name;
public String getName() { return name; }
public void setName(String name) { this.name = name; }
```

In Kotlin, properties handle this automatically:
```kotlin
var name: String = ""
    get() = field.uppercase()  // Custom getter
    set(value) {               // Custom setter
        field = value.trim()
    }
```

## Initialization Patterns

**Lazy initialization** (computed on first access):
```kotlin
val database: Database by lazy {
    println("Creating database connection...")
    Database.connect()
}
```

**Late initialization** (for dependency injection):
```kotlin
lateinit var repository: UserRepository

fun setup() {
    repository = UserRepositoryImpl()  // Must initialize before use
}
```

## Real-World Context

Properties appear in every Kotlin class. Android development uses `lateinit` for views before they're inflated. Backend applications use lazy initialization for expensive resources like database connections. Custom getters enable computed properties without storing redundant data.

Mastering properties means writing concise, encapsulated code that Java developers envy!