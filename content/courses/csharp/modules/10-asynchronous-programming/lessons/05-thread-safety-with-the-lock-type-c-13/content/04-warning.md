---
type: "WARNING"
title: "Common Lock Type Pitfalls"
---

## Critical Mistakes to Avoid with C# 13 Lock

### 1. Don't Mix Lock and lock in the Same Codebase

While it might be tempting to incrementally migrate, mixing the two patterns in the same class or related classes can lead to confusion and potential deadlocks.

```csharp
// DANGER: Mixed locking patterns
class MixedLocking
{
    private readonly Lock _modernLock = new();
    private readonly object _legacyLock = new();
    private int _value1;
    private int _value2;
    
    public void MethodA()
    {
        lock (_modernLock)  // Locks _modernLock
        {
            _value1++;
            MethodB();  // DEADLOCK RISK!
        }
    }
    
    public void MethodB()
    {
        lock (_legacyLock)  // Locks DIFFERENT object!
        {
            _value2++;
            // If this tries to lock _modernLock, potential deadlock!
        }
    }
}

// CORRECT: Consistent locking strategy
class ConsistentLocking
{
    private readonly Lock _lock = new();  // One lock for everything
    private int _value1;
    private int _value2;
    
    public void MethodA()
    {
        lock (_lock)
        {
            _value1++;
            MethodB();  // Safe - same lock
        }
    }
    
    public void MethodB()
    {
        lock (_lock)  // Same lock!
        {
            _value2++;
        }
    }
}
```

### 2. Lock Disposal and Resource Management

The C# 13 Lock implements `IDisposable`, but you typically don't need to dispose it directly when using the `lock` statement. However, when using `EnterScope()`, understand the disposal pattern.

```csharp
class DisposalConsiderations
{
    private readonly Lock _lock = new();
    
    // GOOD: lock statement handles everything
    public void SimpleLock()
    {
        lock (_lock)
        {
            // Lock acquired and released automatically
        }
    }
    
    // GOOD: using statement with EnterScope
    public void ScopePattern()
    {
        using (_lock.EnterScope())
        {
            // Lock released at end of using block
        }
    }
    
    // WARNING: Don't dispose the Lock itself while it's in use!
    class BadExample : IDisposable
    {
        private Lock _lock = new();  // Not readonly!
        
        public void Dispose()
        {
            _lock.Dispose();  // Dangerous if other threads are using it!
            _lock = null!;
        }
    }
    
    // CORRECT: Lock should live as long as the object
    class GoodExample : IDisposable
    {
        private readonly Lock _lock = new();
        private bool _disposed;
        
        public void DoWork()
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            
            lock (_lock)
            {
                // Work here
            }
        }
        
        public void Dispose()
        {
            if (!_disposed)
            {
                // Signal disposal, but don't dispose the Lock
                // while other threads might be waiting
                _disposed = true;
            }
        }
    }
}
```

### 3. Deadlock Prevention

Deadlocks occur when two or more threads are waiting for each other to release locks. This is the most dangerous locking pitfall.

```csharp
class DeadlockExample
{
    private readonly Lock _lockA = new();
    private readonly Lock _lockB = new();
    
    // THREAD 1 executes this:
    public void TransferFromAToB()
    {
        lock (_lockA)  // Acquires A
        {
            Thread.Sleep(100);  // Simulate work
            
            lock (_lockB)  // Tries to acquire B...
            {
                // DEADLOCK! If Thread 2 holds B and wants A
            }
        }
    }
    
    // THREAD 2 executes this:
    public void TransferFromBToA()
    {
        lock (_lockB)  // Acquires B
        {
            Thread.Sleep(100);  // Simulate work
            
            lock (_lockA)  // Tries to acquire A...
            {
                // DEADLOCK! Thread 1 holds A, Thread 2 holds B
            }
        }
    }
}

// SOLUTION: Always acquire locks in the same order
class DeadlockSolution
{
    private readonly Lock _lockA = new();
    private readonly Lock _lockB = new();
    
    // Always acquire A before B
    public void TransferFromAToB()
    {
        lock (_lockA)
        {
            lock (_lockB)
            {
                // Safe - consistent ordering
            }
        }
    }
    
    // Same order even when reversing the transfer direction!
    public void TransferFromBToA()
    {
        lock (_lockA)  // Still acquire A first!
        {
            lock (_lockB)  // Then B
            {
                // Safe - same order as above
            }
        }
    }
}
```

### 4. Thread Pool and Lock Contention

Holding locks while doing thread pool operations can cause serious performance issues.

```csharp
class ThreadPoolProblems
{
    private readonly Lock _lock = new();
    private List<int> _data = new();
    
    // BAD: Holding lock while using thread pool
    public async Task BadPatternAsync()
    {
        lock (_lock)
        {
            // DON'T: This blocks the thread pool thread
            // while holding the lock!
            Task.Run(() => 
            {
                // Other threads can't enter the lock
                // because we're still holding it!
            }).Wait();  // NEVER use .Wait() in lock!
        }
    }
    
    // BAD: Async-over-sync in lock
    public void BadAsyncOverSync()
    {
        lock (_lock)
        {
            // DON'T: This can cause thread pool starvation!
            var result = GetDataAsync().Result;  // .Result blocks!
        }
    }
    
    // GOOD: Release lock before async operations
    public async Task GoodPatternAsync()
    {
        List<int> snapshot;
        
        lock (_lock)
        {
            // Take a snapshot while holding lock
            snapshot = new List<int>(_data);
        }  // Release lock immediately
        
        // Do async work without holding lock
        await Task.WhenAll(
            snapshot.Select(item => ProcessAsync(item))
        );
        
        // Re-acquire only when updating
        lock (_lock)
        {
            _data.AddRange(results);
        }
    }
    
    private async Task<int> ProcessAsync(int item)
    {
        await Task.Delay(100);
        return item * 2;
    }
}
```

### 5. Don't Lock on 'this', Public Objects, or Strings

This applies to both traditional locks and C# 13 Lock, but is worth repeating:

```csharp
class DangerousLockingTargets
{
    // DANGER: Locking on 'this'
    public void BadMethod1()
    {
        lock (this)  // External code can lock on your object too!
        {
            // Deadlock risk if external code also locks on this instance
        }
    }
    
    // DANGER: Locking on strings
    public void BadMethod2()
    {
        lock ("myLock")  // String interning - same lock across app!
        {
            // Unrelated code might lock on the same string!
        }
    }
    
    // DANGER: Locking on mutable fields
    private object _badLock = new();
    
    public void BadMethod3()
    {
        lock (_badLock)
        {
            // What if someone does: _badLock = new object(); ?
            // The lock is now on the OLD object!
        }
    }
    
    // DANGER: Locking on public/protected fields
    protected readonly object _protectedLock = new();
    
    // Subclass could use it incorrectly, causing deadlocks
}

// CORRECT: Private, readonly, dedicated Lock instance
class SafeLocking
{
    // The ONLY correct pattern for class-level locks
    private readonly Lock _lock = new();
    
    public void SafeMethod()
    {
        lock (_lock)
        {
            // Safe: private, readonly, dedicated
        }
    }
}
```

### 6. Lock Recursion and Reentrancy

The C# 13 Lock type does NOT support recursion by default. The same thread trying to acquire the lock twice will block.

```csharp
class RecursionTrap
{
    private readonly Lock _lock = new();
    
    // DANGER: Recursive lock attempt
    public void OuterMethod()
    {
        lock (_lock)
        {
            InnerMethod();  // DEADLOCK - tries to re-acquire same lock!
        }
    }
    
    public void InnerMethod()
    {
        lock (_lock)  // Thread already holds this lock!
        {
            // Never reached - deadlock!
        }
    }
    
    // SOLUTION 1: Extract the non-locking logic
    public void FixedOuterMethod()
    {
        lock (_lock)
        {
            DoWork();  // Call non-locking version
        }
    }
    
    public void FixedInnerMethod()
    {
        lock (_lock)
        {
            DoWork();
        }
    }
    
    private void DoWork()  // No locking here!
    {
        // The actual work
    }
    
    // SOLUTION 2: Use ReaderWriterLockSlim with recursion
    private readonly ReaderWriterLockSlim _recursiveLock = new(
        LockRecursionPolicy.SupportsRecursion);
    
    public void RecursiveOuterMethod()
    {
        _recursiveLock.EnterWriteLock();
        try
        {
            RecursiveInnerMethod();  // Works with recursion support
        }
        finally
        {
            _recursiveLock.ExitWriteLock();
        }
    }
    
    public void RecursiveInnerMethod()
    {
        _recursiveLock.EnterWriteLock();
        try
        {
            // This works because recursion is enabled
        }
        finally
        {
            _recursiveLock.ExitWriteLock();
        }
    }
}
```

### 7. Exception Handling in Locks

Exceptions don't break the lock - it's automatically released. But be careful with cleanup.

```csharp
class ExceptionHandling
{
    private readonly Lock _lock = new();
    private bool _operationInProgress;
    
    // GOOD: Lock automatically released on exception
    public void SafeExceptionHandling()
    {
        lock (_lock)
        {
            _operationInProgress = true;
            
            // If this throws, lock is still released!
            RiskyOperation();
            
            _operationInProgress = false;
        }
    }
    
    // BETTER: Use try/finally for state cleanup
    public void BetterExceptionHandling()
    {
        bool acquiredState = false;
        
        lock (_lock)
        {
            try
            {
                _operationInProgress = true;
                acquiredState = true;
                
                RiskyOperation();
            }
            finally
            {
                if (acquiredState)
                {
                    _operationInProgress = false;
                }
            }
        }  // Lock released here
    }
    
    // WARNING: Don't catch and swallow exceptions blindly
    public void DangerousSwallowing()
    {
        lock (_lock)
        {
            try
            {
                RiskyOperation();
            }
            catch
            {
                // DANGER: Lock released, but state might be corrupt!
                // Consider if partial updates left data inconsistent
            }
        }
    }
    
    private void RiskyOperation()
    {
        if (Random.Shared.Next(2) == 0)
            throw new InvalidOperationException("Random failure");
    }
}
```

### 8. Lock Timeouts and Deadlock Detection

Unlike `Monitor.TryEnter`, the standard `lock` statement doesn't support timeouts. For complex scenarios, you might need additional tools.

```csharp
class TimeoutStrategies
{
    private readonly Lock _lock = new();
    private readonly object _monitorLock = new();  // For TryEnter
    
    // Standard lock - no timeout possible
    public void StandardLock()
    {
        lock (_lock)
        {
            // Blocks indefinitely if deadlock occurs
        }
    }
    
    // If you need timeouts, use Monitor with object
    public bool TimedLock(int millisecondsTimeout)
    {
        if (Monitor.TryEnter(_monitorLock, millisecondsTimeout))
        {
            try
            {
                // Got the lock within timeout
                return true;
            }
            finally
            {
                Monitor.Exit(_monitorLock);
            }
        }
        
        // Could not acquire lock in time
        return false;
    }
    
    // For production, consider Polly library for resilience
    // Or use CancellationToken with SemaphoreSlim
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    
    public async Task<bool> AsyncTimeoutAsync(
        CancellationToken cancellationToken)
    {
        if (await _semaphore.WaitAsync(TimeSpan.FromSeconds(5), cancellationToken))
        {
            try
            {
                return true;
            }
            finally
            {
                _semaphore.Release();
            }
        }
        
        return false;
    }
}
```

### 9. Value Types and Boxing

Although you should always use `Lock` type in C# 13, if you ever encounter legacy code, remember: don't lock on value types!

```csharp
class BoxingTrap
{
    // BAD: Value type lock (if you were using int for some reason)
    // private readonly int _lock = 0;  // NEVER do this!
    
    // The lock statement boxes value types, creating a NEW box each time!
    // lock (_intLock)  // Boxes to different object each time!
    // {
    //     // No synchronization actually happening!
    // }
    
    // ALWAYS use the C# 13 Lock type
    private readonly Lock _lock = new();
    
    public void SafeMethod()
    {
        lock (_lock)  // Proper synchronization
        {
            // Thread-safe code
        }
    }
}
```

### Summary: Lock Safety Checklist

Before using the C# 13 Lock type, verify:

- [ ] Lock field is `private readonly Lock`
- [ ] Not mixing Lock types with object locks in same hierarchy
- [ ] Lock sections are as short as possible
- [ ] No async/await inside lock statements
- [ ] Multiple locks are always acquired in the same order
- [ ] No recursive lock attempts on same thread
- [ ] No thread pool blocking while holding locks
- [ ] Proper exception handling for state consistency

Remember: A deadlock is often worse than a race condition because it stops all affected threads completely. Design your locking strategy carefully!
