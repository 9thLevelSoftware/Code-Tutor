---
type: "EXAMPLE"
title: "switch Statement in Action"
---

## Example 1: Classic Switch Statement

The traditional switch statement is perfect for menu selections:

```csharp
string day = "Wednesday";

switch (day)
{
    case "Monday":
        Console.WriteLine("Start of the work week!");
        break;
    case "Tuesday":
        Console.WriteLine("Taco Tuesday!");
        break;
    case "Wednesday":
        Console.WriteLine("Hump day!");
        break;
    case "Thursday":
        Console.WriteLine("Almost Friday!");
        break;
    case "Friday":
        Console.WriteLine("Weekend is coming!");
        break;
    case "Saturday":
    case "Sunday":
        Console.WriteLine("It's the weekend!");
        break;
    default:
        Console.WriteLine("Invalid day");
        break;
}
```

**Output:** `Hump day!`

Notice: **Multiple cases can share the same code** (Saturday and Sunday both go to weekend message).

## Example 2: Switch Expression (Modern C# 8+)

Switch expressions are perfect for simple value mappings:

```csharp
int score = 85;

string grade = score switch
{
    >= 90 => "A",
    >= 80 => "B",
    >= 70 => "C",
    >= 60 => "D",
    _ => "F"  // _ is the default case
};

Console.WriteLine($"Grade: {grade}");
```

**Output:** `Grade: B`

## Example 3: Using Switch with Numbers

```csharp
int month = 3;

switch (month)
{
    case 1:
        Console.WriteLine("January");
        break;
    case 2:
        Console.WriteLine("February");
        break;
    case 3:
        Console.WriteLine("March");
        break;
    // ... more cases
    default:
        Console.WriteLine("Invalid month");
        break;
}
```

## Example 4: Switch Expression with String Result

```csharp
string command = "save";

string result = command.ToLower() switch
{
    "start" => "Starting game...",
    "save" => "Saving progress...",
    "load" => "Loading save file...",
    "quit" => "Quitting game...",
    _ => "Unknown command"
};

Console.WriteLine(result);
```

**Output:** `Saving progress...`

Switch expressions are **concise** and **return values directly**—great for assignments!
