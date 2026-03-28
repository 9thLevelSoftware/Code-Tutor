---
type: "EXAMPLE"
title: "Secondary Constructors in Action"
---

Let's explore practical examples demonstrating when and how to use secondary constructors effectively.

**Example 1: Multiple Ways to Create a Rectangle**

```kotlin
class Rectangle(val width: Double, val height: Double) {
    val area: Double
    
    init {
        require(width > 0 && height > 0) { "Dimensions must be positive" }
        area = width * height
    }
    
    // Create a square
    constructor(side: Double) : this(side, side) {
        println("Created square with side $side")
    }
    
    // Create from another rectangle (copy)
    constructor(other: Rectangle) : this(other.width, other.height) {
        println("Copied rectangle")
    }
}

// Usage
val rect1 = Rectangle(10.0, 20.0)     // Regular rectangle
val rect2 = Rectangle(5.0)            // Square (calls primary with 5, 5)
val rect3 = Rectangle(rect1)          // Copy of rect1
```

**Example 2: Timestamped Event Creation**

```kotlin
class Event(val name: String, val timestamp: Long) {
    
    init {
        require(name.isNotBlank()) { "Event name required" }
    }
    
    // Create event with current time
    constructor(name: String) : this(name, System.currentTimeMillis()) {
        println("Event '$name' created at current time")
    }
    
    // Create event from date string
    constructor(name: String, dateString: String) : this(
        name, 
        java.time.Instant.parse(dateString).toEpochMilli()
    ) {
        println("Event '$name' created from date string")
    }
}

val event1 = Event("User Login", 1704067200000L)
val event2 = Event("System Boot")  // Uses current time
val event3 = Event("Meeting", "2024-01-01T10:00:00Z")
```

**Example 3: Student with Multiple Entry Points**

```kotlin
class Student(val id: String, val name: String, val enrolledCourses: List<String>) {
    
    init {
        require(id.isNotBlank()) { "Student ID required" }
        require(name.isNotBlank()) { "Name required" }
    }
    
    // New student with no courses
    constructor(id: String, name: String) : this(id, name, emptyList()) {
        println("New student enrolled: $name")
    }
    
    // Student from comma-separated data
    constructor(data: String) : this(
        data.split(",")[0].trim(),
        data.split(",")[1].trim(),
        data.split(",").drop(2).map { it.trim() }
    ) {
        println("Student created from CSV data")
    }
}

val student1 = Student("S001", "Alice", listOf("Math", "Physics"))
val student2 = Student("S002", "Bob")  // No courses yet
val student3 = Student("S003,Charlie,Chemistry,Biology,History")
```

**Example 4: Database Entity with Default Values**

```kotlin
class Product(
    val id: String,
    val name: String,
    val price: Double,
    val inStock: Boolean
) {
    
    init {
        require(price >= 0) { "Price cannot be negative" }
    }
    
    // Create product that is always in stock
    constructor(id: String, name: String, price: Double) : 
        this(id, name, price, true)
    
    // Create from map (common in database operations)
    constructor(data: Map<String, Any>) : this(
        id = data["id"] as String,
        name = data["name"] as String,
        price = (data["price"] as Number).toDouble(),
        inStock = data["in_stock"] as? Boolean ?: true
    )
}
```
