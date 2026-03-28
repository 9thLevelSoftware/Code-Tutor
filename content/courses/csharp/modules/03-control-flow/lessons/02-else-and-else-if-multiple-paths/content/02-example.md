---
type: "EXAMPLE"
title: "if-else if-else Chains in Action"
---

## Example 1: Grade Calculator

```csharp
int score = 85;

if (score >= 90)
{
    Console.WriteLine("Grade: A");
}
else if (score >= 80)
{
    Console.WriteLine("Grade: B");
}
else if (score >= 70)
{
    Console.WriteLine("Grade: C");
}
else if (score >= 60)
{
    Console.WriteLine("Grade: D");
}
else
{
    Console.WriteLine("Grade: F");
}
```

**Output:** `Grade: B`

Notice: Only the second condition (`score >= 80`) runs because it was the first match. The conditions are checked top-to-bottom!

## Example 2: Time of Day Greeter

```csharp
int hour = 14; // 2 PM

if (hour >= 5 && hour < 12)
{
    Console.WriteLine("Good morning!");
}
else if (hour >= 12 && hour < 17)
{
    Console.WriteLine("Good afternoon!");
}
else if (hour >= 17 && hour < 21)
{
    Console.WriteLine("Good evening!");
}
else
{
    Console.WriteLine("Good night!");
}
```

**Output:** `Good afternoon!`

## Example 3: Simple Login System

```csharp
string username = "admin";
string password = "secret123";

if (username != "admin")
{
    Console.WriteLine("Username not found");
}
else if (password != "secret123")
{
    Console.WriteLine("Password is incorrect");
}
else
{
    Console.WriteLine("Welcome, admin!");
}
```

**Output:** `Welcome, admin!`

The program checks username first. Only if it matches does it check the password. This creates a logical flow that users expect.
