---
type: "KEY_POINT"
title: "While Loop Essentials"
---

## Key Takeaways

- **Use `while` when the iteration count is unknown** -- keep looping until a condition changes: user input validation, reading a file until end, waiting for a network response.

- **Something inside the loop must change the condition** -- if the condition never becomes false, you get an infinite loop. Always ensure the loop body moves toward termination.

- **Sentinel values signal when to stop** -- a special value like `-1`, `null`, or `"quit"` tells the loop to exit. This is a common pattern for user-driven input loops.

## When to Choose While Loops

**Perfect for while loops:**
- Reading data until EOF (end of file)
- Validating user input (retry until valid)
- Game loops (run until quit signal)
- Polling/waiting for external conditions
- Processing until a sentinel value appears

**Not suitable for while loops:**
- Known iteration count (use for)
- Iterating over collections (use foreach)

## Mastering break and continue

| Statement | Use When | Example Use Case |
|-----------|----------|------------------|
| `break` | Found what you need | Search loop - exit when target found |
| `break` | Error/abort condition | Cancel operation on user request |
| `continue` | Skip specific cases | Skip invalid records, keep processing |
| `continue` | Filter results | Only process odd numbers |

## Infinite Loop Safety Checklist

Before running your while loop, verify:
- [ ] Loop variable is initialized before the while
- [ ] Loop variable is modified inside the while body
- [ ] The modification moves toward making the condition false
- [ ] Edge cases are tested (what if condition starts false?)
- [ ] Maximum iteration safety net added (optional but recommended)

## While vs For Decision Matrix

| Situation | Use While | Use For |
|-----------|-----------|---------|
| Count from 1 to 10 | | ✓ |
| Read until EOF | ✓ | |
| User input validation | ✓ | |
| Iterate array by index | | ✓ |
| Game/animation loop | ✓ | |
| Retry network request | ✓ | |

## Common While Loop Patterns

**Pattern 1: Input Validation**
```csharp
bool isValid = false;
while (!isValid) {
    // Get input
    // Validate
    // Set isValid = true if good
}
```

**Pattern 2: Process Until Sentinel**
```csharp
while (value != -1) {
    // Process value
    // Get next value
}
```

**Pattern 3: Infinite Loop with Break**
```csharp
while (true) {
    // Do work
    if (shouldStop) break;
}
```

## Debugging While Loops

When your while loop misbehaves:
1. **Check the initial condition** - is it what you expect?
2. **Verify the loop variable changes** - add Console.WriteLine to track it
3. **Test boundary conditions** - what happens at first and last iterations?
4. **Use a debugger** - step through to see the actual execution flow
5. **Add iteration limits** - prevent infinite loops during testing:
```csharp
int maxIterations = 1000;
int iterations = 0;
while (condition && iterations < maxIterations) {
    iterations++;
    // loop body
}
```
