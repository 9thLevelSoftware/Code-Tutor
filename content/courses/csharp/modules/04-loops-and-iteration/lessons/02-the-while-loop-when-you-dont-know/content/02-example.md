---
type: "EXAMPLE"
title: "Code Example"
---

The while loop checks its condition BEFORE each iteration. If the condition is true, the loop body executes. Something inside the loop must eventually make the condition false, or you get an infinite loop!

```csharp
// Simple while loop
int count = 1;

while (count <= 5)
{
    Console.WriteLine($"Count is: {count}");
    count++;  // CRITICAL! Must change count or loop never ends!
}

// User input validation (keep asking until valid)
bool validInput = false;
int userAge = 0;

// Simulating user input
userAge = 15; // Pretend user entered this

while (!validInput)
{
    if (userAge >= 18)
    {
        Console.WriteLine("Valid age!");
        validInput = true;
    }
    else
    {
        Console.WriteLine("You must be 18 or older.");
        validInput = true; // For this example, we stop after one check
    }
}

// Countdown with while
int countdown = 5;
while (countdown > 0)
{
    Console.WriteLine(countdown);
    countdown--;
}
Console.WriteLine("Done!");

// Real-world: Processing until a sentinel value
int[] data = { 5, 10, 15, -1 };  // -1 is sentinel (end marker)
int index = 0;

while (data[index] != -1)
{
    Console.WriteLine($"Processing: {data[index]}");
    index++;
}
```

## Real-World: User Input Validation

A classic use case for while loops is validating user input. You need to keep asking until the user provides valid data:

```csharp
// Real input validation - keep asking until valid
Console.WriteLine("Enter a number between 1 and 10:");
int number = 0;
bool isValid = false;

while (!isValid)
{
    string? input = Console.ReadLine();
    
    if (int.TryParse(input, out number) && number >= 1 && number <= 10)
    {
        isValid = true;
        Console.WriteLine($"You entered: {number}");
    }
    else
    {
        Console.WriteLine("Invalid input. Please enter a number between 1 and 10:");
    }
}
```

## Real-World: Simple Game Loop

Game loops are a perfect fit for while loops - you keep running the game until the player chooses to quit:

```csharp
bool playing = true;
int score = 0;
int health = 100;

while (playing && health > 0)
{
    Console.WriteLine($"\nHealth: {health}, Score: {score}");
    Console.WriteLine("What do you want to do? (fight/run/quit)");
    string? action = Console.ReadLine();
    
    switch (action?.ToLower())
    {
        case "fight":
            // Simulate combat
            health -= 10;
            score += 50;
            Console.WriteLine("You fought a monster! -10 health, +50 score");
            break;
        case "run":
            health += 5;
            Console.WriteLine("You rested. +5 health");
            break;
        case "quit":
            playing = false;
            Console.WriteLine("Thanks for playing!");
            break;
        default:
            Console.WriteLine("Invalid command. Try: fight, run, or quit");
            break;
    }
    
    if (health <= 0)
    {
        Console.WriteLine("Game Over! You ran out of health.");
    }
}

Console.WriteLine($"Final Score: {score}");
```

## Using break and continue in Real Programs

Here's a practical password validation example using break and continue:

```csharp
// Password validation with break and continue
string? password;
while (true)  // Infinite loop - we'll break out when valid
{
    Console.Write("Enter a password (min 8 chars, must contain a number): ");
    password = Console.ReadLine();
    
    if (password is null || password.Length < 8)
    {
        Console.WriteLine("Password must be at least 8 characters.");
        continue;  // Skip to next iteration, ask again
    }
    
    bool hasNumber = false;
    foreach (char c in password)
    {
        if (char.IsDigit(c))
        {
            hasNumber = true;
            break;  // Found a digit, no need to check rest
        }
    }
    
    if (!hasNumber)
    {
        Console.WriteLine("Password must contain at least one number.");
        continue;  // Skip to next iteration
    }
    
    // If we get here, password is valid
    break;  // Exit the while loop
}

Console.WriteLine("Password accepted!");
```
