---
type: "EXAMPLE"
title: "Traditional lock vs C# 13 Lock Type"
---

## Code Comparison: Old vs New

### Example 1: Race Condition Without Locking

First, let's see what happens **without** thread safety:

```csharp
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

// DANGER: Race condition without synchronization!
class UnsafeBankAccount
{
    private decimal _balance = 1000m;
    
    public decimal Balance => _balance;
    
    public void Deposit(decimal amount)
    {
        // This is NOT atomic - multiple threads can interfere!
        var newBalance = _balance + amount;
        // Thread could switch here, losing updates!
        _balance = newBalance;
    }
    
    public void Withdraw(decimal amount)
    {
        if (_balance >= amount)
        {
            var newBalance = _balance - amount;
            // Race condition: another thread might withdraw before this writes!
            _balance = newBalance;
        }
    }
}

// Demonstrate the problem
var unsafeAccount = new UnsafeBankAccount();
var tasks = new List<Task>();

// 100 threads, each depositing $10
for (int i = 0; i < 100; i++)
{
    tasks.Add(Task.Run(() => unsafeAccount.Deposit(10)));
}

await Task.WhenAll(tasks);
Console.WriteLine($"Expected balance: $2000");
Console.WriteLine($"Actual balance: ${unsafeAccount.Balance}");
// Result will likely be LESS than $2000 due to lost updates!
```

### Example 2: Traditional lock Statement (C# 12 and earlier)

The classic approach using an object as a lock:

```csharp
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

class TraditionalBankAccount
{
    private decimal _balance;
    // Traditional: Use any object as a lock
    private readonly object _lockObject = new();
    
    public TraditionalBankAccount(decimal initialBalance)
    {
        _balance = initialBalance;
    }
    
    public decimal Balance
    {
        get
        {
            lock (_lockObject)  // Acquire lock
            {
                return _balance;
            }  // Release lock automatically
        }
    }
    
    public void Deposit(decimal amount)
    {
        lock (_lockObject)
        {
            _balance += amount;
            Console.WriteLine($"Deposited ${amount}, Balance: ${_balance}");
        }
    }
    
    public bool Withdraw(decimal amount)
    {
        lock (_lockObject)
        {
            if (_balance >= amount)
            {
                _balance -= amount;
                Console.WriteLine($"Withdrew ${amount}, Balance: ${_balance}");
                return true;
            }
            Console.WriteLine($"Insufficient funds for ${amount}");
            return false;
        }
    }
}

// Test the traditional lock
var account = new TraditionalBankAccount(1000m);
var tasks = new List<Task>();

// Concurrent deposits
for (int i = 0; i < 10; i++)
{
    tasks.Add(Task.Run(() => account.Deposit(100)));
}

// Concurrent withdrawals
for (int i = 0; i < 10; i++)
{
    tasks.Add(Task.Run(() => account.Withdraw(50)));
}

await Task.WhenAll(tasks);
Console.WriteLine($"\nFinal balance: ${account.Balance}");
Console.WriteLine($"Expected: $1500");
Console.WriteLine(account.Balance == 1500m ? "SUCCESS! Lock prevented race conditions!" : "ERROR: Data corruption!");
```

### Example 3: Modern C# 13 Lock Type (NEW!)

The new C# 13 way with `System.Threading.Lock`:

```csharp
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

class ModernBankAccount
{
    private decimal _balance;
    // C# 13: Dedicated Lock type instead of object!
    private readonly Lock _lock = new();
    
    public ModernBankAccount(decimal initialBalance)
    {
        _balance = initialBalance;
    }
    
    public decimal Balance
    {
        get
        {
            lock (_lock)  // Same syntax, better performance!
            {
                return _balance;
            }
        }
    }
    
    public void Deposit(decimal amount)
    {
        lock (_lock)
        {
            _balance += amount;
            Console.WriteLine($"Deposited ${amount}, Balance: ${_balance}");
        }
    }
    
    public bool Withdraw(decimal amount)
    {
        lock (_lock)
        {
            if (_balance >= amount)
            {
                _balance -= amount;
                Console.WriteLine($"Withdrew ${amount}, Balance: ${_balance}");
                return true;
            }
            Console.WriteLine($"Insufficient funds for ${amount}");
            return false;
        }
    }
}

// Same test, better performance
var modernAccount = new ModernBankAccount(1000m);
var tasks = new List<Task>();

for (int i = 0; i < 10; i++)
{
    tasks.Add(Task.Run(() => modernAccount.Deposit(100)));
}

for (int i = 0; i < 10; i++)
{
    tasks.Add(Task.Run(() => modernAccount.Withdraw(50)));
}

await Task.WhenAll(tasks);
Console.WriteLine($"\nFinal balance: ${modernAccount.Balance}");
// Result is always correct, and it's ~30% faster!
```

### Example 4: EnterScope Pattern (Alternative Syntax)

The C# 13 Lock also supports a scope-based pattern:

```csharp
using System.Threading;

class ScopedLockExample
{
    private int _counter;
    private readonly Lock _lock = new();
    
    // Traditional lock syntax
    public void IncrementTraditional()
    {
        lock (_lock)
        {
            _counter++;
        }
    }
    
    // C# 13 scope-based syntax
    public void IncrementScoped()
    {
        using (_lock.EnterScope())  // Auto-dispose pattern
        {
            _counter++;
        }  // Lock automatically released here
    }
    
    // Async-compatible pattern
    public async Task SafeIncrementAsync()
    {
        // For async methods, don't use lock - use other primitives
        // But you CAN use Lock with proper patterns
        await Task.Run(() =>
        {
            using var scope = _lock.EnterScope();
            _counter++;
        });
    }
    
    public int Counter => _counter;
}
```

### Example 5: Async-Await with Lock (Safe Patterns)

While you can't use `lock` directly in async methods, you can use the C# 13 Lock type with proper patterns:

```csharp
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

class AsyncSafeDataProcessor
{
    private readonly List<string> _data = new();
    private readonly Lock _lock = new();
    
    // CORRECT: Lock only the synchronous work
    public async Task ProcessDataAsync(string item)
    {
        // Step 1: Do async work OUTSIDE the lock
        var processedData = await FetchAndProcessAsync(item);
        
        // Step 2: Lock only when accessing shared state
        lock (_lock)
        {
            _data.Add(processedData);
        }
        
        // Step 3: More async work outside the lock
        await SaveToCacheAsync(processedData);
    }
    
    // CORRECT: Use Task.Run for async methods that need locking
    public async Task<int> GetCountAsync()
    {
        // Run on thread pool to avoid blocking async context
        return await Task.Run(() =>
        {
            lock (_lock)
            {
                return _data.Count;
            }
        });
    }
    
    // WRONG: Don't do this!
    // public async Task BadMethodAsync()
    // {
    //     lock (_lock)  // COMPILER ERROR in async method!
    //     {
    //         await Task.Delay(100);  // Can't await inside lock!
    //     }
    // }
    
    private async Task<string> FetchAndProcessAsync(string item)
    {
        await Task.Delay(100);  // Simulate API call
        return $"Processed: {item}";
    }
    
    private async Task SaveToCacheAsync(string data)
    {
        await Task.Delay(50);  // Simulate cache write
    }
    
    public int Count
    {
        get
        {
            lock (_lock)
            {
                return _data.Count;
            }
        }
    }
}
```

### Key Differences Summary

| Feature | Traditional `object` Lock | C# 13 `Lock` Type |
|---------|---------------------------|-------------------|
| Declaration | `private readonly object _lock = new();` | `private readonly Lock _lock = new();` |
| Usage | `lock (_lock) { }` | `lock (_lock) { }` (same!) |
| Performance | Good | ~30% faster |
| Intent | Generic object reused for locking | Purpose-built for synchronization |
| Disposal | Not disposable | Implements `IDisposable` with `EnterScope()` |
| Async-friendly | Same limitations | Same limitations, but cleaner patterns |
| Compiler optimization | Standard | Optimized code generation |
