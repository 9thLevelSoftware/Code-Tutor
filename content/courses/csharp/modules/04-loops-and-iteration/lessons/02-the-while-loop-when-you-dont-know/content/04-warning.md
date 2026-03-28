---
type: "WARNING"
title: "Common Pitfalls"
---

## Watch Out For These Mistakes!

**Infinite loops**: The #1 mistake! Forgetting to change the condition variable inside the loop: `while (x < 10) { /* oops, x never changes! */ }` hangs forever.

**Semicolon after while**: `while (condition);` is a silent bug! The semicolon creates an empty loop body, and your actual code runs only once after the (infinite) loop ends.

**Condition never true**: If the condition starts false, the loop body NEVER runs: `int x = 100; while (x < 50) { /* never executes */ }`

**Changing the wrong variable**: If your condition checks `count`, make sure you're modifying `count` inside the loop, not some unrelated variable!

**Not using braces**: Without braces, only the FIRST statement is part of the loop. Always use braces to avoid subtle bugs!

## Infinite Loop Pitfalls Explained

Infinite loops are the most dangerous and common while loop mistake. Here's a deeper look at the traps:

**Off-by-One Errors Leading to Infinite Loops**
```csharp
// WRONG: Will run forever
int i = 0;
while (i <= 10)  // Condition is true when i is 10
{
    Console.WriteLine(i);
    // Forgot to increment! i stays at 0 forever
}
```

**Loop Variable Shadowing**
```csharp
int count = 0;
while (count < 5)
{
    int count = count + 1;  // Error! Creates NEW variable 'count'
    Console.WriteLine(count);
    // Outer 'count' never changes - infinite loop!
}
```

**Modifying the Wrong Variable**
```csharp
int x = 0;
int y = 0;
while (x < 10)  // Checking x
{
    Console.WriteLine(y);
    y++;  // But changing y! x never changes - infinite loop
}
```

**Floating-Point Precision Issues**
```csharp
// Dangerous: Floating point comparison
double value = 0.1;
while (value != 1.0)  // May never be exactly 1.0 due to precision!
{
    value += 0.1;
    Console.WriteLine(value);
}
// Safer alternative:
// while (value < 1.0) or use a counter for iterations
```

## break and continue Mistakes

**Forgetting to Update Before continue**
```csharp
int i = 0;
while (i < 5)
{
    if (i == 2)
    {
        continue;  // DANGER! i never increments past 2!
    }
    Console.WriteLine(i);
    i++;
}
// Fix: Increment BEFORE continue
```

**break in the Wrong Place**
```csharp
// Accidentally breaking out of the wrong loop
while (condition1)
{
    while (condition2)
    {
        if (shouldExit)
        {
            break;  // Only exits INNER loop, not outer!
        }
    }
    // Code here still runs
}
```

## Condition Evaluation Gotchas

**Short-Circuit Evaluation Surprises**
```csharp
int x = 5;
while (x > 0 && SomeExpensiveOperation())
{
    // SomeExpensiveOperation() is only called if x > 0
    // But if it has side effects, order matters!
    x--;
}
```

**Null Reference in Condition**
```csharp
string? input = null;
while (input.Length > 0)  // CRASH! NullReferenceException
{
    // Process input
}
// Always check for null first:
// while (input != null && input.Length > 0)
```

## Best Practices to Avoid Infinite Loops

1. **Always initialize your loop variable before the while**
2. **Always ensure the loop variable is modified inside the loop**
3. **Use a maximum iteration counter as a safety net**:

```csharp
int attempts = 0;
int maxAttempts = 1000;
while (!conditionMet && attempts < maxAttempts)
{
    // Do work
    attempts++;
}

if (attempts >= maxAttempts)
{
    Console.WriteLine("Warning: Maximum attempts reached!");
}
```

4. **Test with edge cases** - what happens if the condition is initially false?
5. **Use a debugger** - step through your loop to verify it terminates