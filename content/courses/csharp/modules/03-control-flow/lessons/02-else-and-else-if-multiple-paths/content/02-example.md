---
type: "EXAMPLE"
title: "if-else if-else Chains in Action"
---

## Example 1: Grade Calculator with C# 13 Pattern Matching

```csharp
// Using C# 13 pattern matching enhancements for cleaner conditions
int score = 85;

if (score is >= 90)
{
    Console.WriteLine("Grade: A - Excellent work!");
}
else if (score is >= 80)
{
    Console.WriteLine("Grade: B - Good job!");
}
else if (score is >= 70)
{
    Console.WriteLine("Grade: C - Satisfactory");
}
else if (score is >= 60)
{
    Console.WriteLine("Grade: D - Needs improvement");
}
else
{
    Console.WriteLine("Grade: F - See me after class");
}
```

**Output:** `Grade: B - Good job!`

Notice: Only the second condition (`score >= 80`) runs because it was the first match. The conditions are checked top-to-bottom!

---

## Example 2: Time of Day Greeter (Modern C#)

```csharp
int hour = 14; // 2 PM

// Using C# 13 pattern matching with relational patterns
if (hour is >= 5 and < 12)
{
    Console.WriteLine("Good morning! ☀️");
}
else if (hour is >= 12 and < 17)
{
    Console.WriteLine("Good afternoon! 🌤️");
}
else if (hour is >= 17 and < 21)
{
    Console.WriteLine("Good evening! 🌅");
}
else
{
    Console.WriteLine("Good night! 🌙");
}
```

**Output:** `Good afternoon! 🌤️`

---

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

---

## Example 4: Weather Decision Helper (Complete Program)

```csharp
using System;

class WeatherAdvisor
{
    static void Main()
    {
        // Simulate weather data
        int temperature = 72;
        bool isRaining = false;
        bool isSnowing = false;
        int windSpeed = 15;
        
        Console.WriteLine("=== Weather Decision Helper ===");
        Console.WriteLine($"Temperature: {temperature}°F");
        Console.WriteLine($"Wind: {windSpeed} mph");
        Console.WriteLine();
        
        // Determine clothing recommendation
        string clothingAdvice;
        
        if (isSnowing)
        {
            clothingAdvice = "Wear a heavy winter coat, boots, and gloves.";
        }
        else if (isRaining && temperature < 50)
        {
            clothingAdvice = "Waterproof jacket and warm layers needed.";
        }
        else if (isRaining)
        {
            clothingAdvice = "Bring an umbrella or light rain jacket.";
        }
        else if (temperature is >= 85)
        {
            clothingAdvice = "Shorts and t-shirt weather! Stay hydrated.";
        }
        else if (temperature is >= 70 and < 85)
        {
            clothingAdvice = "Perfect weather! Light clothing recommended.";
        }
        else if (temperature is >= 50 and < 70)
        {
            clothingAdvice = "Bring a light jacket or sweater.";
        }
        else if (temperature is >= 32 and < 50)
        {
            clothingAdvice = "Wear a warm coat and layers.";
        }
        else
        {
            clothingAdvice = "Freezing! Heavy winter gear essential.";
        }
        
        Console.WriteLine($"Clothing: {clothingAdvice}");
        
        // Determine activity recommendation
        string activityAdvice;
        
        if (isRaining || isSnowing)
        {
            if (windSpeed > 25)
            {
                activityAdvice = "Stay indoors! Stormy conditions.";
            }
            else
            {
                activityAdvice = "Good day for indoor activities.";
            }
        }
        else if (temperature is >= 60 and < 85 && windSpeed < 20)
        {
            activityAdvice = "Perfect day for outdoor activities!";
        }
        else if (temperature is >= 85)
        {
            activityAdvice = "Great for swimming or water activities.";
        }
        else
        {
            activityAdvice = "Consider indoor activities or dress warmly.";
        }
        
        Console.WriteLine($"Activity: {activityAdvice}");
        
        // UV and sun protection advice
        if (!isRaining && !isSnowing && temperature > 60)
        {
            Console.WriteLine("Don't forget sunscreen! ☀️");
        }
    }
}
```

**Output:**
```
=== Weather Decision Helper ===
Temperature: 72°F
Wind: 15 mph

Clothing: Perfect weather! Light clothing recommended.
Activity: Perfect day for outdoor activities!
Don't forget sunscreen! ☀️
```

---

## Example 5: Package Shipping Calculator (Business Logic)

```csharp
using System;

class ShippingCalculator
{
    static void Main()
    {
        // Package details
        double weight = 2.5;      // pounds
        double dimensions = 12.0;  // cubic inches
        string destination = "international"; // "local", "domestic", "international"
        bool isExpress = true;
        
        decimal baseRate;
        string shippingClass;
        
        // Determine shipping class based on weight and dimensions
        if (weight is <= 1.0 && dimensions is <= 100.0)
        {
            shippingClass = "Letter/Large Envelope";
            baseRate = 1.50m;
        }
        else if (weight is <= 5.0 && dimensions is <= 500.0)
        {
            shippingClass = "Small Package";
            baseRate = 5.99m;
        }
        else if (weight is <= 20.0 && dimensions is <= 2000.0)
        {
            shippingClass = "Medium Package";
            baseRate = 12.99m;
        }
        else if (weight is <= 50.0)
        {
            shippingClass = "Large Package";
            baseRate = 25.99m;
        }
        else
        {
            shippingClass = "Freight";
            baseRate = 49.99m;
        }
        
        // Calculate destination multiplier
        decimal destinationMultiplier = destination.ToLower() switch
        {
            "local" => 1.0m,
            "domestic" => 1.5m,
            "international" => 3.0m,
            _ => 1.0m
        };
        
        // Calculate express surcharge
        decimal expressSurcharge = isExpress ? 5.00m : 0.00m;
        
        // Calculate total
        decimal totalCost = (baseRate * destinationMultiplier) + expressSurcharge;
        
        Console.WriteLine("=== Shipping Quote ===");
        Console.WriteLine($"Class: {shippingClass}");
        Console.WriteLine($"Base Rate: ${baseRate:F2}");
        Console.WriteLine($"Destination: {destination} (x{destinationMultiplier})");
        Console.WriteLine($"Express Delivery: {(isExpress ? "Yes" : "No")}");
        Console.WriteLine($"-----------------------");
        Console.WriteLine($"Total Cost: ${totalCost:F2}");
    }
}
```

**Output:**
```
=== Shipping Quote ===
Class: Small Package
Base Rate: $5.99
Destination: international (x3)
Express Delivery: Yes
-----------------------
Total Cost: $22.97
```

This example demonstrates how else-if chains work together with other C# features like switch expressions to build real business logic!
