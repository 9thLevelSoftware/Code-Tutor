---
type: "EXAMPLE"
title: "Comparison & Logical Operators in Action"
---

## Example 1: All Comparison Operators

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

## Example 2: Logical AND (&&) - Both Must Be True

```csharp
int age = 25;
bool hasTicket = true;

// Both conditions must be true
if (age >= 18 && hasTicket)
{
    Console.WriteLine("Welcome to the concert!");
}

// Check if someone can vote
bool isCitizen = true;
bool isRegistered = true;

if (age >= 18 && isCitizen && isRegistered)
{
    Console.WriteLine("You can vote!");
}
```

## Example 3: Logical OR (||) - At Least One Must Be True

```csharp
bool isWeekend = true;
bool isHoliday = false;

// Either condition being true is enough
if (isWeekend || isHoliday)
{
    Console.WriteLine("No work today!");
}

// Multiple options
string day = "Saturday";
if (day == "Saturday" || day == "Sunday")
{
    Console.WriteLine("It's the weekend!");
}
```

## Example 4: Logical NOT (!) - Flipping Conditions

```csharp
bool isRaining = false;
bool isClosed = false;

// NOT flips the value
if (!isRaining)
{
    Console.WriteLine("Let's go for a walk!");
}

// Combining NOT with other checks
if (!isClosed && !isRaining)
{
    Console.WriteLine("The park is open and dry!");
}
```

## Example 5: Combining Everything

```csharp
int temperature = 75;
bool isSunny = true;
bool hasUmbrella = false;

// Complex condition for going outside
if ((temperature >= 70 && temperature <= 85 && isSunny) || hasUmbrella)
{
    Console.WriteLine("Great weather for a picnic!");
}

// Validating user input
string username = "john_doe";
int passwordLength = 12;

if (username.Length >= 3 && username.Length <= 20 && passwordLength >= 8)
{
    Console.WriteLine("Valid credentials!");
}
```
