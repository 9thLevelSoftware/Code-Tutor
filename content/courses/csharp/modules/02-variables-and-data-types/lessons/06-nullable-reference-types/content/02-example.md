---
type: "EXAMPLE"
title: "Code Example"
---

Nullable reference types help you avoid the dreaded NullReferenceException - one of the most common bugs in C# programs! By marking variables with `?`, you're telling both the compiler and other developers that this value might be null and needs to be handled carefully. Modern C# (.NET 9) enables this feature by default in new projects.

```csharp
// Non-nullable - MUST have a value!
string firstName = "Alice";  // OK
// string lastName = null;   // WARNING! Non-nullable can't be null!

// Nullable - CAN be null (use ? after type)
string? middleName = null;    // OK - ? means nullable
string? nickname = "Ali";     // Also OK - can have value

// Simulating a value that might be null
string? maybeName = GetUserInput();  // Could return null!

// WRONG: Using nullable without checking
// Console.WriteLine(maybeName.Length);  // WARNING! Might be null!

// Safe ways to handle nullable:

// 1. Null check with if
if (maybeName != null)
{
    Console.WriteLine(maybeName.Length);  // Safe now!
}

// 2. Null-conditional operator ?.
int? length = maybeName?.Length;  // Returns null if maybeName is null
Console.WriteLine($"Length: {length}");

// 3. Null-coalescing operator ??
string displayName = maybeName ?? "Guest";  // Use "Guest" if null
Console.WriteLine($"Hello, {displayName}!");

// 4. Pattern matching (modern approach - recommended!)
if (maybeName is string actualName)
{
    Console.WriteLine($"Welcome, {actualName}!");
}
else
{
    Console.WriteLine("No name provided");
}

// 5. Null-coalescing assignment ??=
string? message = null;
message ??= "Default message";  // Only assigns if message is null
Console.WriteLine(message);

// Helper method that might return null
static string? GetUserInput()
{
    // Simulating: sometimes we get input, sometimes not
    return Random.Shared.Next(2) == 0 ? null : "User123";
}
```
