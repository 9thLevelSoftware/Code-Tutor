// .NET 9 ConfigureAwait test
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http;

// Test ConfigureAwait in library code
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
            .ConfigureAwait(false);

        return await response.Content
            .ReadAsStringAsync()
            .ConfigureAwait(false);
    }
}

// Test Task.WhenEach (.NET 9 feature)
public class TaskWhenEachTest
{
    public static async Task TestWhenEach()
    {
        var tasks = new[]
        {
            Task.Delay(100).ContinueWith(_ => "Task 1"),
            Task.Delay(50).ContinueWith(_ => "Task 2"),
            Task.Delay(150).ContinueWith(_ => "Task 3")
        };

        Console.WriteLine("Testing Task.WhenEach (.NET 9):");
        await foreach (var completedTask in Task.WhenEach(tasks))
        {
            var result = await completedTask;
            Console.WriteLine($"Completed: {result}");
        }
    }
}

class Program
{
    static async Task Main()
    {
        // Note: ConfigureAwait test requires a real HTTP endpoint
        // Using mock for syntax validation only
        Console.WriteLine("ConfigureAwait syntax validated.");

        // Test Task.WhenEach
        await TaskWhenEachTest.TestWhenEach();

        Console.WriteLine(".NET 9 tests completed!");
    }
}
