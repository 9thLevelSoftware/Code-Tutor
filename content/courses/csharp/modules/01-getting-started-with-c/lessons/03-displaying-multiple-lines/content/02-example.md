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

## Practical Example: Creating a Formatted Receipt

Here's a real-world example showing how to create a formatted store receipt:

```csharp
// Store receipt using multiple output techniques
string storeName = "FreshMart";
string date = DateTime.Now.ToShortDateString();
string item1 = "Apples";
decimal price1 = 3.50m;
string item2 = "Bread";
decimal price2 = 2.25m;
decimal total = price1 + price2;

// Header using verbatim string
Console.WriteLine(@"==================");
Console.WriteLine($"  {storeName}");
Console.WriteLine(@"==================");
Console.WriteLine($"Date: {date}\n");

// Items with tabs for alignment
Console.WriteLine("Item\t\tPrice");
Console.WriteLine("------------------------");
Console.WriteLine($"{item1}\t\t${price1:F2}");
Console.WriteLine($"{item2}\t\t${price2:F2}");
Console.WriteLine("------------------------");

// Total with newline for spacing
Console.WriteLine($"\nTOTAL:\t\t${total:F2}");
Console.WriteLine("\nThank you for shopping!");
```

## Practical Example: Multi-Line Welcome Message

A common use case is displaying a welcome message when your program starts:

```csharp
// Using verbatim string for ASCII art welcome
Console.WriteLine(@"
========================================
  WELCOME TO MY APPLICATION
========================================
  Version: 1.0
  .NET Version: 9.0
  
  Press any key to continue...
========================================
");

// Alternative using escape sequences
Console.WriteLine("========================================\n  WELCOME TO MY APPLICATION\n========================================\n  Version: 1.0\n  .NET Version: 9.0\n\n  Press any key to continue...\n========================================");
```

## Practical Example: Using Tabs for Alignment

The `\t` escape sequence creates tabs, useful for aligning columns:

```csharp
// Using tabs to create aligned columns
Console.WriteLine("Product\t\tQty\tPrice\tTotal");
Console.WriteLine("----------------------------------------");
Console.WriteLine($"Apples\t\t5\t$0.50\t$2.50");
Console.WriteLine($"Oranges\t\t3\t$0.75\t$2.25");
Console.WriteLine($"Bananas\t\t4\t$0.40\t$1.60");
Console.WriteLine("----------------------------------------");

// Using string interpolation with newlines for a summary
int totalItems = 12;
decimal grandTotal = 6.35m;
Console.WriteLine($"\nTotal Items: {totalItems}\nGrand Total: ${grandTotal:F2}");
```

## Practical Example: Environment.NewLine for Cross-Platform Files

When writing text that might be saved to a file, use Environment.NewLine:

```csharp
// Creating a multi-line string that works on any OS
string report = "Sales Report" + Environment.NewLine +
                "============" + Environment.NewLine +
                Environment.NewLine +
                "Q1: $10,000" + Environment.NewLine +
                "Q2: $15,000" + Environment.NewLine +
                "Q3: $12,500" + Environment.NewLine +
                "Q4: $18,000" + Environment.NewLine +
                Environment.NewLine +
                "Total: $55,500";

Console.WriteLine(report);

// Alternative using string.Join with Environment.NewLine
string[] lines = {
    "Sales Report",
    "============",
    "",
    "Q1: $10,000",
    "Q2: $15,000",
    "Q3: $12,500",
    "Q4: $18,000",
    "",
    "Total: $55,500"
};

string report2 = string.Join(Environment.NewLine, lines);
Console.WriteLine(report2);
```
