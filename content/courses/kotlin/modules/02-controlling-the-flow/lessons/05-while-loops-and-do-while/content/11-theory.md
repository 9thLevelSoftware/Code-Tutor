---
type: "THEORY"
title: "Summary"
---


Congratulations! You've mastered condition-based loops. Let's recap:

**Key Concepts:**
- **While loops** repeat based on conditions, not counts
- **Do-while loops** execute at least once before checking
- **Break** exits the loop immediately
- **Continue** skips to the next iteration
- **Infinite loops** can be intentional with proper guards

**Loop Decision Guide:**

**Control Flow:**

**Best Practices:**
- Always ensure loops can exit
- Validate user input
- Initialize variables before loops
- Use meaningful variable names
- Guard against infinite loops

**Common Patterns:**

---



```kotlin
// Input validation
do {
    // Get input
} while (invalid)

// Menu systems
while (choice != "quit") {
    // Show menu
}

// Search until found
while (!found && index < size) {
    // Search logic
}
```
