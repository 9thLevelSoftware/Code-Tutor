---
type: "WARNING"
title: "Async Pattern Pitfalls"
---

## Critical Best Practices!

**1. Always support CancellationToken in long operations!**
Users expect to cancel slow operations. APIs expect it. Not supporting cancellation is a usability bug.

**2. Dispose CancellationTokenSource!**
```csharp
using var cts = new CancellationTokenSource();
// CTS implements IDisposable - always dispose!
```

**3. Don't swallow OperationCanceledException!**
Cancellation is not an error - it's expected behavior. Catch it to clean up, but don't treat it as a failure.

**4. ConfigureAwait(false) on EVERY await in libraries!**
One missed ConfigureAwait can still cause deadlock. Be consistent!

**5. Progress callbacks happen on captured context!**
Progress<T> marshals to the original thread. If UI thread is blocked, progress updates queue up. Never block on async code!

**6. Fire-and-forget needs error handling!**
```csharp
// DANGEROUS - exceptions are lost!
_ = DoWorkAsync();

// BETTER - log errors
_ = DoWorkAsync().ContinueWith(t => 
    Log(t.Exception), TaskContinuationOptions.OnlyOnFaulted);

// BEST in .NET 6+ - use async void only for event handlers with try/catch
private async void Button_Click(object sender, EventArgs e)
{
    try
    {
        await DoWorkAsync();
    }
    catch (Exception ex)
    {
        Log(ex);
    }
}
```

**7. NEVER use `.Result` or `.Wait()` - causes deadlocks!**
```csharp
// DANGEROUS - can deadlock!
var result = GetDataAsync().Result;

// SAFE - use await all the way up
var result = await GetDataAsync();

// Or use .GetAwaiter().GetResult() in Main (last resort)
static void Main() => MainAsync().GetAwaiter().GetResult();
```