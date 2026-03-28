---
type: "EXAMPLE"
title: "Code Example"
---

This example demonstrates the concepts in action.

```csharp
// Method 1: Multiple WriteLine statements
Console.WriteLine("First line");
Console.WriteLine("Second line");
Console.WriteLine("Third line");

// Method 2: Using \n for newlines
Console.WriteLine("First line\nSecond line\nThird line");

// Method 3: String concatenation with newlines
string message = "Line 1" + "\n" + "Line 2" + "\n" + "Line 3";
Console.WriteLine(message);

// Method 4: Environment.NewLine (cross-platform compatible)
string multiLine = "Line A" + Environment.NewLine + "Line B" + Environment.NewLine + "Line C";
Console.WriteLine(multiLine);

// Method 5: Verbatim string (@) for multi-line output
// Using @ preserves line breaks in the code itself
Console.WriteLine(@"First line of verbatim
Second line of verbatim
Third line of verbatim");

// Method 6: Interpolated string with newlines
string name = "Alice";
int age = 25;
Console.WriteLine($"Name: {name}\nAge: {age}\nCity: New York");
```
