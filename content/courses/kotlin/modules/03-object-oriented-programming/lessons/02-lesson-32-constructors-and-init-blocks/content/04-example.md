---
type: "EXAMPLE"
title: "Primary Constructor and Init Block in Practice"
---

Let's explore how primary constructors and init blocks work together through practical examples.

**Example 1: Validating User Registration**

```kotlin
class User(val username: String, val email: String, val age: Int) {
    init {
        require(username.length >= 3) { 
            "Username must be at least 3 characters" 
        }
        require("@" in email) { 
            "Email must contain @" 
        }
        require(age >= 13) { 
            "Must be at least 13 years old" 
        }
    }
}

// Creating users
val user1 = User("alice", "alice@example.com", 25)  // ✅ Valid
val user2 = User("ab", "bob@example.com", 20)       // ❌ Fails: username too short
val user3 = User("carol", "carol-at-example.com", 20) // ❌ Fails: invalid email
```

**Example 2: Calculating Derived Properties**

```kotlin
class Circle(val radius: Double) {
    val area: Double
    val circumference: Double
    
    init {
        require(radius > 0) { "Radius must be positive" }
        area = Math.PI * radius * radius
        circumference = 2 * Math.PI * radius
    }
}

val circle = Circle(5.0)
println("Area: ${circle.area}")               // 78.54...
println("Circumference: ${circle.circumference}") // 31.41...
```

**Example 3: Setting Up Collections**

```kotlin
class ShoppingCart(val userId: String) {
    val items: MutableList<String>
    val createdAt: Long
    
    init {
        require(userId.isNotBlank()) { "User ID required" }
        items = mutableListOf()
        createdAt = System.currentTimeMillis()
        println("Cart created for user $userId at $createdAt")
    }
}

val cart = ShoppingCart("user_123")
cart.items.add("Laptop")
cart.items.add("Mouse")
```

**Key Observations**

1. **Validation happens immediately** — Invalid objects cannot be created
2. **Properties are initialized** — Either in the primary constructor or init blocks
3. **Code is clean and focused** — Constructor for structure, init for logic
