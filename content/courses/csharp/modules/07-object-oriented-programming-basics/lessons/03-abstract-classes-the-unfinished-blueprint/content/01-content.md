---
type: "THEORY"
title: "Abstract Classes - The Unfinished Blueprint"
---

**Estimated Time**: 55 minutes
**Difficulty**: Intermediate

## Introduction

Abstract classes are partially implemented blueprints that cannot be instantiated directly. They define contracts (abstract methods) that derived classes must implement while providing shared functionality (concrete methods) that all subclasses inherit. This balance of constraint and reuse makes abstract classes a cornerstone of object-oriented design.

In this lesson, you'll learn:

1. **Abstract Class Definition**: Creating classes with both abstract and concrete members
2. **Implementation Requirements**: Forcing derived classes to fulfill contracts
3. **Shared Implementation**: Code reuse through inherited concrete methods
4. **When to Choose Abstract vs. Interface**: Design guidance for different scenarios

## Abstract vs. Interface

C# distinguishes between these concepts:

| Feature | Abstract Class | Interface |
|---------|---------------|-----------|
| Instantiation | No | No |
| Multiple inheritance | Single only | Multiple allowed |
| Default implementation | Yes | Yes (C# 8+) |
| Fields | Yes | Only static (C# 8+) |
| Access modifiers | Any | Public by default |
| Constructor | Yes | No |

Use **abstract classes** for "is-a" relationships with shared implementation. Use **interfaces** for "can-do" capabilities that cross inheritance boundaries.

## Real-World Context

The .NET Base Class Library makes extensive use of abstract classes:

- `Stream`: Abstract base for FileStream, MemoryStream, NetworkStream
- `DbConnection`: Shared logic for SQL Server, PostgreSQL, MySQL connections
- `ControllerBase`: Common MVC functionality that specific controllers extend

Game engines like Unity use abstract classes for `MonoBehaviour`—all scripts inherit shared lifecycle methods (`Start`, `Update`) while implementing their own specific behavior.

## Design Example

```csharp
public abstract class PaymentProcessor
{
    // All processors must implement this
    public abstract Task<PaymentResult> ProcessPayment(decimal amount);
    
    // Shared implementation for all processors
    public void LogTransaction(string transactionId)
    {
        Console.WriteLine($"[{DateTime.Now}] Transaction: {transactionId}");
    }
    
    // Template method pattern
    public async Task<PaymentResult> ExecutePayment(decimal amount)
    {
        ValidateAmount(amount);
        var result = await ProcessPayment(amount);
        LogTransaction(result.TransactionId);
        return result;
    }
    
    private void ValidateAmount(decimal amount)
    {
        if (amount <= 0) throw new ArgumentException("Amount must be positive");
    }
}
```

Abstract classes provide the structure of interfaces with the practicality of inheritance—when you need both, they're the right choice.