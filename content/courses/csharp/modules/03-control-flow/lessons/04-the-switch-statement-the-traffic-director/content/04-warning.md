---
type: "WARNING"
title: "Common Pitfalls with Switch"
---

# Common Mistakes and How to Avoid Them

## ⚠️ Pitfall 1: Missing break Statements

In classic switch statements, every case must end with an exit statement. Forgetting `break` causes a compiler error:

```csharp
switch (day)
{
    case "Monday":
        Console.WriteLine("Monday");
        // Oops! Missing break causes a compiler error
    case "Tuesday":
        Console.WriteLine("Tuesday");
        break;
}
```

**Error:** `Control cannot fall through from one case label to another`

**Fix:** Always include `break`, `return`, `throw`, or `goto case` at the end of each case.

## ⚠️ Pitfall 2: Using Non-Constant Case Values

Case values must be compile-time constants, not variables:

```csharp
int targetScore = 100;

switch (score)
{
    case targetScore:  // ERROR! Not a constant
        Console.WriteLine("Perfect!");
        break;
}
```

**Fix:** Use `const` for case values or switch to an if-statement:

```csharp
const int TargetScore = 100;
switch (score)
{
    case TargetScore:  // OK - constant
        Console.WriteLine("Perfect!");
        break;
}
```

## ⚠️ Pitfall 3: Duplicate Case Values

Each case value must be unique within a switch:

```csharp
switch (score)
{
    case 100:
        Console.WriteLine("Perfect!");
        break;
    case 100:  // ERROR! Duplicate case value
        Console.WriteLine("Top score!");
        break;
}
```

**Fix:** Remove duplicate cases or consolidate them.

## ⚠️ Pitfall 4: Case Sensitivity with Strings

String comparisons in switch are case-sensitive by default:

```csharp
string input = "YES";

switch (input)
{
    case "yes":    // Does NOT match "YES"!
        Console.WriteLine("Confirmed");
        break;
    default:
        Console.WriteLine("Unknown");  // This runs instead!
        break;
}
```

**Fix:** Normalize the input before switching:

```csharp
switch (input.ToLower())
{
    case "yes":  // Now matches "YES", "yes", "Yes"
        Console.WriteLine("Confirmed");
        break;
}
```

## ⚠️ Pitfall 5: Missing Default Case

Always include a `default` case to handle unexpected values:

```csharp
int month = 13;  // Invalid month!

switch (month)
{
    case 1: Console.WriteLine("January"); break;
    case 2: Console.WriteLine("February"); break;
    // ... cases for 3-12
    // No default case!
}

// Nothing happens - no output, no error!
```

**Best Practice:** Always include `default` to catch unexpected values and make your code more robust.

## ⚠️ Pitfall 6: Using Wrong Types in Classic Switch

Classic switch has limited type support:

```csharp
double price = 10.5;

switch (price)  // ERROR! Can't switch on double
{
    case 10.5:
        // ...
}
```

**Allowed in classic switch:** `int`, `long`, `string`, `char`, `bool`, `enum`, and nullable versions

**Not allowed:** `double`, `float`, `object` (without pattern matching)

**Fix:** Use pattern matching in switch expressions for objects:

```csharp
string result = obj switch
{
    double d when d > 10.0 => "Expensive",
    double d => "Affordable",
    _ => "Unknown"
};
```

## ⚠️ Pitfall 7: Non-Exhaustive Enum Switch Expressions

The compiler warns when you miss enum cases in switch expressions:

```csharp
enum Status { Active, Inactive, Pending }

// Compiler warning: not all cases handled
string text = status switch
{
    Status.Active => "Active",
    Status.Inactive => "Inactive"
    // Missing: Pending!
};
```

**Fix:** Either handle all cases or add a discard pattern:

```csharp
string text = status switch
{
    Status.Active => "Active",
    Status.Inactive => "Inactive",
    Status.Pending => "Pending",  // Handle all cases
    _ => "Unknown"  // Or use catch-all
};
```

## ⚠️ Pitfall 8: Complex Conditions in Classic Switch

Classic switch is for exact value matching. For complex conditions, use pattern matching:

```csharp
// WRONG - awkward workaround
switch (true)
{
    case bool b when age >= 18:  // Valid but ugly
        Console.WriteLine("Adult");
        break;
}

// CORRECT - use if-else for complex ranges
if (age >= 18 && age < 65)
    Console.WriteLine("Working age");

// OR use switch expressions with patterns
string category = age switch
{
    >= 0 and < 18 => "Minor",
    >= 18 and < 65 => "Adult",
    >= 65 => "Senior",
    _ => "Invalid"
};
```

## ⚠️ Pitfall 9: Forgetting Null Checks

When switching on nullable types or objects, null values can cause issues:

```csharp
string? input = null;

// Classic switch - null won't match any case
switch (input)
{
    case "yes":  // Won't match null
        break;
    default:
        // Null falls here, which may be unexpected
        break;
}
```

**Fix:** Explicitly handle null in pattern matching:

```csharp
string result = input switch
{
    "yes" => "Confirmed",
    "no" => "Denied",
    null => "No response",
    _ => "Invalid"
};
```

## ⚠️ Pitfall 10: Order Matters in Switch Expressions

Switch expressions evaluate patterns in order. Put specific patterns first:

```csharp
// WRONG ORDER
string result = score switch
{
    >= 60 => "Passing",      // Catches 90 too!
    >= 90 => "Excellent",    // Never reached!
    _ => "Failing"
};

// CORRECT ORDER
string result = score switch
{
    >= 90 => "Excellent",    // Most specific first
    >= 60 => "Passing",
    _ => "Failing"
};
```

**Best Practice:** Order cases from most specific to least specific.
