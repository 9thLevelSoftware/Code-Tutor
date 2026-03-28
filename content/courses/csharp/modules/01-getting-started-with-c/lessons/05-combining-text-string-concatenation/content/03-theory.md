---
type: "THEORY"
title: "Syntax Breakdown"
---

## Breaking Down the Syntax

**`"Text1" + "Text2"`**: The + operator glues text together. Make sure there's a space between words, or they'll stick together like 'HelloWorld'!

**`"Text" + number`**: When you add text and a number, C# converts the number to text first. "I am " + 25 becomes "I am 25".

**`(2 + 2) in text`**: Parentheses matter! "Result: " + 2 + 2 gives "Result: 22" (text gluing), but "Result: " + (2 + 2) gives "Result: 4" (math first).

## String Interpolation: The Modern Way

In modern C# (.NET 9), **string interpolation** is preferred over concatenation:

```csharp
// Old way (concatenation)
string message = "Hello, " + name + "! You have " + count + " items.";

// Modern way (interpolation) - cleaner!
string message = $"Hello, {name}! You have {count} items.";
```

**Why interpolation is better:**
- More readable - the text flows naturally
- Less error-prone - no forgetting + or spaces
- Supports formatting: `$"{price:C}"` for currency, `$"{date:yyyy-MM-dd}"` for dates
- Better performance in .NET 6+ (compiler optimizes it!)

We teach concatenation first because it helps you understand HOW strings combine. But in real projects, use interpolation!