---
type: "THEORY"
title: "ConfigureAwait Deep Dive - .NET 9 Guidance"
---

## ConfigureAwait(false) - When and Why?

**What does it do?**
By default, after an `await`, execution resumes on the original "synchronization context" (UI thread, ASP.NET request context). `ConfigureAwait(false)` tells the runtime: 'I don't need to resume on the original context - resume anywhere.'

---

## 🆕 .NET 9 / Modern Guidance (2025)

### When to use `ConfigureAwait(false)`:
- **Library code**: Still recommended! Libraries don't know their callers
- **Reusable components**: Middleware, utilities, shared code
- **Performance-critical paths**: Avoids context-switch overhead

### When NOT to use it:
- **UI code (WPF, WinForms, MAUI, Avalonia)**: You NEED the UI thread to update controls
- **ASP.NET Core**: No synchronization context since .NET Core 3.0 - `ConfigureAwait(false)` is harmless but unnecessary
- **Legacy ASP.NET (.NET Framework)**: Still needs `ConfigureAwait(false)` for libraries

### .NET 9 Best Practices Summary

```csharp
// LIBRARY CODE (.NET 9) - Still use ConfigureAwait(false)
public class MyLibrary
{
    public async Task<string> FetchAsync(string url)
    {
        using var client = new HttpClient();
        var response = await client.GetAsync(url).ConfigureAwait(false);
        return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
    }
}

// ASP.NET CORE (.NET 9) - No sync context, your choice
public class MyController : ControllerBase
{
    [HttpGet("data")]
    public async Task<IActionResult> GetData()
    {
        // Both work identically in ASP.NET Core:
        var data = await _service.FetchAsync("url");        // OK
        // var data = await _service.FetchAsync("url").ConfigureAwait(false);  // Also OK
        return Ok(data);
    }
}

// DESKTOP/WPF APP - Keep context for UI
public async Task LoadDataAsync()
{
    // DON'T use ConfigureAwait(false) - need UI thread
    var data = await _service.FetchAsync("url");
    StatusLabel.Text = "Loaded";  // UI update requires UI thread
}
```

---

## 🆕 .NET 9: Task.WhenEach for Processing Tasks As They Complete

**The Problem:** `Task.WhenAll` waits for ALL tasks to complete. What if you want to process results as they become available?

**The Solution:** .NET 9 introduces `Task.WhenEach` - an async enumerable that yields tasks as they complete!

```csharp
// .NET 9 - Process tasks as they complete!
async Task ProcessAsTheyCompleteAsync()
{
    var tasks = new[]
    {
        FetchDataAsync("https://api1.example.com"),
        FetchDataAsync("https://api2.example.com"),
        FetchDataAsync("https://api3.example.com")
    };

    // Process each result as it arrives, without waiting for all!
    await foreach (var completedTask in Task.WhenEach(tasks))
    {
        var result = await completedTask;  // Already complete!
        Console.WriteLine($"Got result: {result}");
    }
}
```

**Benefits of Task.WhenEach:**
- Start processing results immediately (no waiting for slowest task)
- Great for UIs - show data as it streams in
- More efficient than `Task.WhenAny` loops
- Cleaner syntax with `await foreach`

**Compare with old approach:**
```csharp
// OLD WAY - manually tracking with WhenAny
var pending = new List<Task<string>>(tasks);
while (pending.Count > 0)
{
    var completed = await Task.WhenAny(pending);
    pending.Remove(completed);
    var result = await completed;
    // Process result...
}
```

**Note:** `Task.WhenEach` is a .NET 9 runtime feature, not a C# language feature. It works with any C# version targeting .NET 9+.