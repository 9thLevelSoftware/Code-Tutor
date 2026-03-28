---
type: "THEORY"
title: "Inheritance and Polymorphism in Kotlin"
---

**Estimated Time**: 60 minutes
**Difficulty**: Intermediate

## Introduction

Inheritance allows classes to derive properties and behavior from parent classes, while polymorphism enables objects of different classes to be treated uniformly through a common interface. These concepts are fundamental to building extensible, maintainable object-oriented systems.

**Learning Objectives**:
- Extend classes using the `open` and `override` keywords
- Understand `super` calls and constructor chaining
- Apply polymorphism for flexible, decoupled designs
- Recognize when composition is preferable to inheritance

## Kotlin's Inheritance Model

Unlike Java, Kotlin classes are `final` by default—you must explicitly mark them `open` to allow extension:

```kotlin
open class Animal(val name: String) {
    open fun makeSound() = "Some sound"
}

class Dog(name: String) : Animal(name) {
    override fun makeSound() = "Woof!"
}
```

## Polymorphism in Action

Polymorphism lets you write flexible code:

```kotlin
fun animalConcert(animals: List<Animal>) {
    for (animal in animals) {
        println("${animal.name} says: ${animal.makeSound()}")
    }
}

// Works with any Animal subtype
animalConcert(listOf(Dog("Buddy"), Cat("Whiskers"), Animal("Unknown")))
```

## Composition vs. Inheritance

Favor composition when:
- The relationship isn't truly "is-a"
- You need multiple sources of behavior
- You want to change behavior at runtime

```kotlin
// Composition: Has-a relationship
class Car(val engine: Engine, val wheels: List<Wheel>)

// Inheritance: Is-a relationship
class ElectricCar : Car  // Only if ElectricCar truly IS a Car
```

## Real-World Context

Inheritance hierarchies appear in Android (`View` → `TextView` → `Button`), backend frameworks (`Exception` → `IOException` → `FileNotFoundException`), and game engines. Polymorphism enables plugin architectures, testing with mocks, and extensible frameworks.

Understanding when to extend—and when to compose—is a hallmark of experienced Kotlin developers. It has proper frontmatter so the loader will not fail to parse it.
