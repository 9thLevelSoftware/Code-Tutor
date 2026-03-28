---
type: "WARNING"
title: "Async Pattern Pitfalls - .NET 9 Updated"
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

**4. ConfigureAwait(false) guidance for .NET 9:**

| Context | Recommendation |
|---------|---------------|
| **Library/Reusable code** | ✅ Use `ConfigureAwait(false)` on every await |
| **ASP.NET Core apps** | 🤷 Either way works - no sync context |
| **Desktop/UI apps** | ❌ Don't use it - you need the UI thread |
| **Legacy ASP.NET** | ✅ Use `ConfigureAwait(false)` |

```csharp
// Library - CORRECT
public async Task<string> GetDataAsync()
{
    var response = await _client.GetAsync(url).ConfigureAwait(false);
    return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
}

// ASP.NET Core - Optional (no-op either way)
[HttpGet]
public async Task<IActionResult> Get()
{
    var data = await _service.GetDataAsync();  // OK without ConfigureAwait
    return Ok(data);
}

// UI app - CORRECT (no ConfigureAwait)
private async void Button_Click(object sender, EventArgs e)
{
    var data = await _service.GetDataAsync();
    label.Text = data;  // Needs UI thread
}
```

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

**8. .NET 9: Leverage Task.WhenEach instead of WhenAny loops**
```csharp
// OLD - Complex WhenAny loop
var pending = new List<Task>(tasks);
while (pending.Count > 0)
{
    var completed = await Task.WhenAny(pending);
    pending.Remove(completed);
    // process...
}

// NEW - Clean Task.WhenEach (NET 9+)
await foreach (var task in Task.WhenEach(tasks))
{
    // process immediately as each completes
}
```