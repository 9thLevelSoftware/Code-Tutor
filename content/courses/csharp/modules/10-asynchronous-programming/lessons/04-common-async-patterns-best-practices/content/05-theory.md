---
type: "THEORY"
title: "ConfigureAwait Deep Dive"
---

## ConfigureAwait(false) - When and Why?

**What does it do?**
By default, after an await, execution resumes on the original context (UI thread, ASP.NET request context). `ConfigureAwait(false)` says 'I don't need to resume on the original context.'

**When to use ConfigureAwait(false):**
- **Library code**: Always! You don't know if your code will be called from UI apps
- **Internal helper methods**: When you don't need UI thread access
- **Performance-critical code**: Avoids context-switch overhead

**When NOT to use it:**
- **UI code**: You NEED to be on UI thread to update controls
- **ASP.NET Core**: No synchronization context, so it's a no-op anyway
- **Code that accesses HttpContext**: Needs the request context

```csharp
// Library code - use ConfigureAwait(false)
public async Task<string> FetchDataAsync()
{
    var response = await httpClient
        .GetAsync(url)
        .ConfigureAwait(false);  // Don't capture context!
        
    return await response.Content
        .ReadAsStringAsync()
        .ConfigureAwait(false);  // On EVERY await!
}

// UI code - DON'T use it
private async void Button_Click(object sender, EventArgs e)
{
    string data = await FetchDataAsync();
    label.Text = data;  // Needs UI thread!
}
```

**Modern guidance (.NET 9 / 2025):** Use ConfigureAwait(false) in library code. For app code in ASP.NET Core (no sync context), it doesn't matter. This guidance remains valid in .NET 9.

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
- More efficient than Task.WhenAny loops
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