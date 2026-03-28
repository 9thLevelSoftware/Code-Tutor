---
type: "THEORY"
title: "Syntax Breakdown"
---

## Breaking Down the Syntax

**`while (condition)`**: The loop checks the condition BEFORE each iteration. If true, the loop body runs. If false, the loop stops and continues after the braces.

**`Condition checked FIRST`**: Unlike do-while (coming soon!), while checks BEFORE running. If the condition starts as false, the loop body NEVER runs, not even once!

**`Must change something!`**: CRITICAL: Something inside the loop MUST eventually make the condition false! Otherwise you get an INFINITE LOOP and your program hangs forever.

**`while vs for`**: Use FOR when you know the count (repeat 10 times). Use WHILE when you're checking a condition (repeat until user enters valid input).

**`Sentinel values`**: A common pattern uses a special value (like -1 or null) to signal when to stop. The while loop keeps processing until it encounters this sentinel.

## How the While Loop Condition Evaluation Works

The while loop follows a predictable cycle:

1. **Evaluate the condition** - The condition expression is checked BEFORE entering the loop body
2. **If the condition is TRUE** - Execute all statements inside the loop body, then go back to step 1
3. **If the condition is FALSE** - Skip the loop body entirely and continue with the code after the closing brace

```csharp
int number = 3;

// Step 1: Check if number > 0 (3 > 0 is TRUE)
while (number > 0)
{
    // Step 2: Execute body
    Console.WriteLine(number);
    number--;  // number becomes 2
}
// Step 3: When number becomes 0, condition is FALSE, loop exits
```

## When the Condition is Checked

**Before EACH iteration** - This means:

- The condition is evaluated before the first iteration
- The condition is re-evaluated after every iteration
- The loop may run zero times if the condition starts false

```csharp
int counter = 10;

// This loop runs ZERO times because condition is false immediately
while (counter < 5)
{
    Console.WriteLine("This will never print!");
    counter--;
}
```

## Importance of Updating the Loop Variable

The most common mistake is forgetting to update the variable used in the condition:

```csharp
// DANGER: Infinite loop!
int x = 1;
while (x <= 5)
{
    Console.WriteLine(x);
    // Oops! Forgot to update x, so x stays 1 forever
    // The condition (1 <= 5) will ALWAYS be true
}
```

**Always ensure** that something inside the loop will eventually make the condition false.

## Common Infinite Loop Scenarios

Here are the most common ways to accidentally create infinite loops:

**1. Forgot to increment/decrement**
```csharp
int i = 0;
while (i < 10)
{
    Console.WriteLine(i);
    // Missing: i++;
}
```

**2. Wrong comparison direction**
```csharp
int i = 0;
while (i > 10)  // Should be < 10
{
    Console.WriteLine(i);
    i++;
}
```

**3. Condition never changes**
```csharp
bool running = true;
while (running)
{
    Console.WriteLine("Looping...");
    // Missing: running = false; or a break condition
}
```

**4. Variable shadowing (accidentally creating a new variable)**
```csharp
int count = 0;
while (count < 5)
{
    int count = count + 1;  // Error: creates new 'count' instead of updating outer one
    Console.WriteLine(count);
}
```

**5. Complex conditions with logical errors**
```csharp
int a = 5, b = 10;
while (a < 10 || b < 5)  // b starts at 10, so b < 5 is always false, but a < 10 is true
{
    Console.WriteLine(a);
    a++;  // a eventually reaches 10
    // But b never changes, so loop exits when a >= 10
}
```

**Tip**: Always trace through your loop mentally or with a debugger to ensure the condition will eventually become false.

## Break and Continue in While Loops

While loops support two special statements that give you more control: `break` and `continue`.

### The `break` Statement

**`break`** immediately exits the loop entirely, regardless of the condition. It's like hitting the emergency stop button.

```csharp
// Find the first number divisible by 7
int number = 1;
while (number <= 100)
{
    if (number % 7 == 0)
    {
        Console.WriteLine($"Found it: {number}");
        break;  // Exit the loop immediately!
    }
    number++;
}
// Program continues here after break
Console.WriteLine("Done searching");
```

### The `continue` Statement

**`continue`** skips the rest of the current iteration and jumps back to check the condition. It's like saying "skip this one, move to the next."

```csharp
// Print all odd numbers from 1 to 10
int i = 0;
while (i < 10)
{
    i++;
    if (i % 2 == 0)  // If even number
    {
        continue;  // Skip the rest, go back to condition check
    }
    Console.WriteLine(i);  // Only prints odd numbers: 1, 3, 5, 7, 9
}
```

**Important**: When using `continue`, make sure you update your loop variable BEFORE the continue statement, or you might create an infinite loop:

```csharp
// DANGER: Infinite loop!
int x = 0;
while (x < 5)
{
    if (x == 2)
    {
        continue;  // Oops! x never increments past 2!
    }
    Console.WriteLine(x);
    x++;  // This line is skipped when x == 2
}
```

**Correct version**:
```csharp
int x = 0;
while (x < 5)
{
    if (x == 2)
    {
        x++;  // Increment BEFORE continuing
        continue;
    }
    Console.WriteLine(x);
    x++;
}
```

## When to Use break vs continue

| Statement | Use When... |
|-----------|-------------|
| `break` | You found what you're looking for and want to stop searching |
| `break` | An error occurred and you need to abort the loop |
| `continue` | You want to skip specific values (skip even numbers) |
| `continue` | You want to skip invalid data and keep processing |

**Real-world pattern**: User input validation with `break`:

```csharp
string? input;
while (true)  // Infinite loop, we'll break out when valid
{
    Console.Write("Enter your age (18-65): ");
    input = Console.ReadLine();
    
    if (int.TryParse(input, out int age) && age >= 18 && age <= 65)
    {
        Console.WriteLine($"Valid age: {age}");
        break;  // Valid input, exit loop
    }
    
    Console.WriteLine("Invalid input. Try again.");
    // Loop continues automatically
}
```

This pattern of `while (true)` with a `break` is very common when you want the loop body to run at least once and the exit condition is complex.