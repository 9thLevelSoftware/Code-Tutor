---
type: "EXAMPLE"
title: "Comparison, Logical, and Pattern Matching Operators in Action"
---

## Example 1: All Comparison Operators

Comparison operators form the foundation of decision-making. Here's how each one works:

```csharp
int score = 85;
int perfect = 100;

// Equality checks
if (score == 100)
    Console.WriteLine("Perfect score!");

if (score != 100)
    Console.WriteLine("Not perfect, but still good!");

// Greater/Less than
if (score > 80)
    Console.WriteLine("Above 80!");

if (score < 90)
    Console.WriteLine("Below 90!");

// Greater/Less than or equal
if (score >= 85)
    Console.WriteLine("85 or higher!");

if (score <= 100)
    Console.WriteLine("Within valid range!");
```

**Output:**
```
Not perfect, but still good!
Above 80!
Below 90!
85 or higher!
Within valid range!
```

## Example 2: Logical AND (&&) — All Must Be True

The AND operator requires every condition to pass—like multi-factor authentication:

```csharp
int age = 25;
bool hasTicket = true;
bool isBanned = false;

// All three conditions must be true
if (age >= 18 && hasTicket && !isBanned)
{
    Console.WriteLine("Welcome to the concert!");
}

// User registration validation
string username = "john_doe";
int passwordLength = 12;
bool emailVerified = true;

if (username.Length >= 3 && username.Length <= 20 && passwordLength >= 8 && emailVerified)
{
    Console.WriteLine("Registration successful!");
}
```

**Output:**
```
Welcome to the concert!
Registration successful!
```

## Example 3: Logical OR (||) — At Least One Must Be True

The OR operator provides alternative paths—multiple valid ways to achieve the same outcome:

```csharp
bool isWeekend = false;
bool isHoliday = true;
bool isRemoteWorker = false;

// Any condition being true is enough
if (isWeekend || isHoliday || isRemoteWorker)
{
    Console.WriteLine("No commute today!");
}

// Multiple string comparisons
string day = "Saturday";
if (day == "Saturday" || day == "Sunday")
{
    Console.WriteLine("It's the weekend!");
}

// Feature access with multiple valid roles
string userRole = "moderator";
if (userRole == "admin" || userRole == "moderator" || userRole == "editor")
{
    Console.WriteLine("Content management access granted");
}
```

**Output:**
```
No commute today!
It's the weekend!
Content management access granted
```

## Example 4: Logical NOT (!) — Flipping Conditions

The NOT operator inverts boolean values—essential for exclusion checks:

```csharp
bool isRaining = false;
bool isClosed = false;
bool isMaintenanceMode = false;

// NOT flips the value
if (!isRaining)
{
    Console.WriteLine("Perfect weather for a walk!");
}

// Combining NOT with AND for safe operation checks
if (!isClosed && !isMaintenanceMode)
{
    Console.WriteLine("The park is open for visitors!");
}

// Double NOT cancels out (avoid this in practice!)
bool hasPermission = true;
if (!!hasPermission)  // Same as hasPermission
{
    Console.WriteLine("Access confirmed");
}
```

**Output:**
```
Perfect weather for a walk!
The park is open for visitors!
Access confirmed
```

## Example 5: Combining Everything — Complex Conditions

Real-world scenarios often require mixing operators. Use parentheses to control evaluation order:

```csharp
int temperature = 75;
bool isSunny = true;
bool hasUmbrella = false;
bool isIndoorVenue = true;

// Complex condition: good weather OR prepared for rain, AND venue is accessible
if (((temperature >= 70 && temperature <= 85 && isSunny) || hasUmbrella) && isIndoorVenue)
{
    Console.WriteLine("Great day for an outdoor/indoor event!");
}

// Banking transaction validation
decimal balance = 500.00m;
decimal withdrawalAmount = 100.00m;
bool isBusinessHours = true;
bool isEmergencyAccount = false;

if ((balance >= withdrawalAmount && isBusinessHours) || isEmergencyAccount)
{
    Console.WriteLine("Transaction approved");
}
```

**Output:**
```
Great day for an outdoor/indoor event!
Transaction approved
```

## Example 6: The `is` Operator — Type Checking (C# 7+)

The `is` operator checks if an object is of a specific type or pattern. Modern C# makes type checking elegant:

```csharp
object value = "Hello, C#!";

// Classic type checking (still works)
if (value is string)
{
    Console.WriteLine("It's a string!");
}

// Modern pattern matching with declaration (C# 7+)
if (value is string text)
{
    Console.WriteLine($"String content: {text}");
}

// Checking for null with 'is not' (C# 9+)
object? mightBeNull = null;
if (mightBeNull is not null)
{
    Console.WriteLine("Object exists");
}
else
{
    Console.WriteLine("Object is null");
}

// Multiple type checks with OR pattern (C# 9+)
object number = 42;
if (number is int or long or double)
{
    Console.WriteLine("It's a numeric type!");
}
```

**Output:**
```
It's a string!
String content: Hello, C#!
Object is null
It's a numeric type!
```

## Example 7: Pattern Matching with Relational Patterns (C# 9+)

Modern C# introduces relational patterns for cleaner range checking:

```csharp
int temperature = 85;

// Using 'is' with relational patterns (C# 9+)
if (temperature is > 90)
{
    Console.WriteLine("Heat warning! Stay hydrated.");
}
else if (temperature is >= 75 and <= 90)
{
    Console.WriteLine("Perfect weather!");
}
else if (temperature is < 32)
{
    Console.WriteLine("Freezing! Bundle up.");
}

// Range check with 'and' pattern
int age = 25;
if (age is >= 13 and <= 19)
{
    Console.WriteLine("Teenager detected");
}
else if (age is >= 20 and <= 35)
{
    Console.WriteLine("Young adult");
}
```

**Output:**
```
Perfect weather!
Young adult
```

## Example 8: Advanced Pattern Matching with 'or' and 'and' (C# 9+)

Combine patterns for sophisticated matching logic:

```csharp
char grade = 'B';

// Using 'or' pattern for multiple valid values
if (grade is 'A' or 'B' or 'C')
{
    Console.WriteLine("Passing grade");
}

// Combining 'and' and 'or' in switch expressions
int score = 85;
string performance = score switch
{
    >= 90 and <= 100 => "Excellent",
    >= 80 and < 90 => "Good",
    >= 70 and < 80 => "Satisfactory",
    >= 60 and < 70 => "Needs Improvement",
    >= 0 and < 60 => "Failing",
    _ => "Invalid score"
};

Console.WriteLine($"Performance: {performance}");

// Type and value pattern matching
object input = 42;
if (input is int i and > 0)
{
    Console.WriteLine($"Positive integer: {i}");
}
```

**Output:**
```
Passing grade
Performance: Good
Positive integer: 42
```

## Example 9: Null Checking Patterns (C# 9+)

Modern null checking is more readable and less error-prone:

```csharp
string? userInput = null;

// Traditional null check
if (userInput != null)
{
    Console.WriteLine($"Input: {userInput}");
}

// Modern 'is not null' pattern (C# 9+)
if (userInput is not null)
{
    Console.WriteLine($"Input: {userInput}");
}
else
{
    Console.WriteLine("No input provided");
}

// Null-coalescing with pattern matching
string? displayName = null;
string actualName = displayName is not null ? displayName : "Anonymous";
// Or simply: displayName ?? "Anonymous"
Console.WriteLine($"User: {actualName}");
```

**Output:**
```
No input provided
User: Anonymous
```

## Example 10: Real-World Login System

A practical example combining all concepts for user authentication:

```csharp
// Simulating a login scenario
string? username = "alice_dev";
string? password = "YOUR_PASSWORD_HERE";
int failedAttempts = 2;
bool isLockedOut = false;
DateTime lastLogin = DateTime.Now.AddDays(-1);

// Complex login validation with modern C# features
if (username is not null && password is not null && 
    username.Length >= 3 && password.Length >= 8 &&
    !isLockedOut && failedAttempts < 3)
{
    Console.WriteLine("Login successful!");
    
    // Check if first login today
    if (lastLogin.Date < DateTime.Today)
    {
        Console.WriteLine("Welcome back! Here's your daily tip.");
    }
}
else if (isLockedOut || failedAttempts >= 3)
{
    Console.WriteLine("Account locked. Please contact support.");
}
else
{
    Console.WriteLine("Invalid credentials. Please try again.");
}
```

**Output:**
```
Login successful!
Welcome back! Here's your daily tip.
```
