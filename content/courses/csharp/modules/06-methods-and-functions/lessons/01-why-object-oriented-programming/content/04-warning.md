---
type: "WARNING"
title: "Common Pitfalls"
---

## Watch Out For These Issues!

**Forgetting 'new'**: `Player p = Player();` is WRONG! You must use `new Player()` to create an instance. Without 'new', C# thinks you're calling a static method or referencing a type, not creating an object. The compiler will complain about invalid syntax. Always remember: creating an object requires the `new` keyword to allocate memory and invoke the constructor.

**Class vs Object confusion**: The class is the BLUEPRINT (the definition in code). Objects are INSTANCES built from that blueprint. You can have many `Player` objects, but only one `Player` class definition exists in your assembly. Think of it this way: "Dog" is the concept (class), but "Rex" and "Buddy" are actual dogs (objects) with their own names, ages, and behaviors.

**Public fields are risky!** While we use `public string Name;` here for simplicity, production code should use properties with getters and setters. Public fields allow external code to assign any value - including null or invalid data - with no validation. Properties let you add validation logic, change notification, or access control. You'll learn about properties in the next lesson.

**Null reference errors**: If you declare `Player p;` without `= new Player()`, then accessing `p.Name` will crash with a `NullReferenceException`! The variable exists but points to nothing. Always initialize your objects before using them, or use null-conditional operators (`p?.Name`) if null is a valid state.

**Shallow vs Deep Copy confusion**: Assigning `Player p2 = p1;` doesn't create a new player - it creates a second reference to the same object. Both variables point to the same memory location. Changing `p2.Name` also changes `p1.Name` because they're the same object! If you need independent copies, you must implement a copy constructor or cloning mechanism.

**Memory management**: Every `new` allocates memory on the heap. Creating thousands of objects in tight loops without understanding garbage collection can impact performance. While C# handles cleanup automatically, being mindful of object lifetime - especially for large objects or disposable resources - is important for scalable applications.