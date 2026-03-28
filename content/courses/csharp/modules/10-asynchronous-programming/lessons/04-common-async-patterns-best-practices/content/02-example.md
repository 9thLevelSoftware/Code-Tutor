---
type: "EXAMPLE"
title: "Code Example"
---

This example demonstrates the concepts in action.

```csharp
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http;

// PATTERN 1: Error handling in async
async Task<string> FetchDataAsync()
{
    try
    {
        using HttpClient client = new HttpClient();  // Modern using declaration
        return await client.GetStringAsync("https://api.example.com/data");
    }
    catch (HttpRequestException ex)
    {
        Console.WriteLine("Network error: " + ex.Message);
        return "Error occurred";
    }
}

// PATTERN 2: Cancellation with CancellationToken
async Task LongRunningTaskAsync(CancellationToken cancellationToken)
{
    for (int i = 0; i < 10; i++)
    {
        // Check if cancellation requested
        cancellationToken.ThrowIfCancellationRequested();
        
        Console.WriteLine("Working... " + i);
        await Task.Delay(500, cancellationToken);
    }
    Console.WriteLine("Task completed!");
}

// Using cancellation
CancellationTokenSource cts = new CancellationTokenSource();
Task task = LongRunningTaskAsync(cts.Token);

cts.CancelAfter(2000);  // Cancel after 2 seconds

try
{
    await task;
}
catch (OperationCanceledException)
{
    Console.WriteLine("Task was cancelled!");
}

// PATTERN 3: Timeout pattern
async Task<string> GetDataWithTimeoutAsync(int timeoutMs)
{
    using CancellationTokenSource cts = new CancellationTokenSource();  // Modern using declaration
    cts.CancelAfter(timeoutMs);
    
    try
    {
        return await FetchDataAsync();  // Your async operation
    }
    catch (OperationCanceledException)
    {
        return "Operation timed out!";  // cts disposed at end of method scope
    }
}

// PATTERN 4: Progress reporting
async Task ProcessWithProgressAsync(IProgress<int> progress)
{
    for (int i = 0; i <= 100; i += 10)
    {
        await Task.Delay(200);
        progress?.Report(i);  // Report progress
    }
}

// Using progress
var progress = new Progress<int>(percent => 
{
    Console.WriteLine("Progress: " + percent + "%");
});

await ProcessWithProgressAsync(progress);

// PATTERN 5: Retry logic
async Task<string> RetryAsync(Func<Task<string>> operation, int maxRetries)
{
    for (int i = 0; i < maxRetries; i++)
    {
        try
        {
            return await operation();
        }
        catch (Exception ex)
        {
            if (i == maxRetries - 1) throw;  // Last attempt, rethrow
            Console.WriteLine("Attempt " + (i + 1) + " failed, retrying...");
            await Task.Delay(1000 * (i + 1));  // Exponential backoff
        }
    }
    throw new Exception("All retries failed");
}
```

---

## ConfigureAwait Guidance (.NET 9)

```csharp
// .NET 9 GUIDANCE: ConfigureAwait(false)

// LIBRARY CODE - Still recommended in .NET 9
public class DataService
{
    private readonly HttpClient _httpClient;
    
    public DataService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    
    // Library code should use ConfigureAwait(false)
    public async Task<string> GetDataAsync(string url)
    {
        var response = await _httpClient
            .GetAsync(url)
            .ConfigureAwait(false);  // Prevent deadlock risk
            
        return await response.Content
            .ReadAsStringAsync()
            .ConfigureAwait(false);  // On EVERY await!
    }
}

// ASP.NET CORE - No sync context, ConfigureAwait is a no-op
public class ApiController : ControllerBase
{
    private readonly DataService _dataService;
    
    public ApiController(DataService dataService)
    {
        _dataService = dataService;
    }
    
    [HttpGet("data")]
    public async Task<IActionResult> GetData()
    {
        // ASP.NET Core has no SynchronizationContext
        // ConfigureAwait(false) is harmless but unnecessary
        var data = await _dataService.GetDataAsync("/api/data");
        return Ok(data);
    }
}

// DESKTOP/WPF/WinForms - Keep context for UI updates
public partial class MainWindow : Window
{
    private async void LoadButton_Click(object sender, RoutedEventArgs e)
    {
        LoadButton.IsEnabled = false;
        
        // DON'T use ConfigureAwait(false) - need UI thread
        var data = await _dataService.GetDataAsync("/api/data");
        
        // This must run on UI thread
        DataTextBox.Text = data;
        LoadButton.IsEnabled = true;
    }
}
```
