using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

// TODO: Create a thread-safe async cache using C# 13 Lock type
//
// Requirements:
// 1. Use C# 13 Lock: private readonly Lock _lock = new();
// 2. Thread-safe dictionary operations
// 3. Handle expiration
// 4. Support async operations

public class AsyncCache<TKey, TValue> where TKey : notnull
{
    // TODO: Declare C# 13 Lock type
    // private readonly Lock _lock = new();
    
    // TODO: Declare storage dictionaries
    // private readonly Dictionary<TKey, TValue> _items = new();
    // private readonly Dictionary<TKey, DateTime> _expirations = new();
    
    private readonly TimeSpan _defaultExpiration = TimeSpan.FromSeconds(5);
    
    // TODO: Implement GetAsync
    // - Use lock to check if key exists
    // - Check if expired (compare DateTime.UtcNow with expiration)
    // - Return value or default
    public async Task<TValue?> GetAsync(TKey key)
    {
        // Your implementation here
        throw new NotImplementedException();
    }
    
    // TODO: Implement SetAsync
    // - Calculate expiration time
    // - Use lock to safely add/update
    public async Task SetAsync(TKey key, TValue value, TimeSpan? expiration = null)
    {
        // Your implementation here
        throw new NotImplementedException();
    }
    
    // TODO: Implement RemoveAsync
    // - Use lock to safely remove
    // - Return true if removed, false if not found
    public async Task<bool> RemoveAsync(TKey key)
    {
        // Your implementation here
        throw new NotImplementedException();
    }
    
    // TODO: Implement Count property
    // - Use lock to count non-expired items
    // - Clean up expired items while counting
    public int Count
    {
        get
        {
            // Your implementation here
            throw new NotImplementedException();
        }
    }
}

// Test the cache
public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("Cache created successfully");
        
        var cache = new AsyncCache<string, string>();
        
        // Test setting values
        await cache.SetAsync("key1", "value1");
        await cache.SetAsync("key2", "value2", TimeSpan.FromSeconds(10));
        Console.WriteLine("Items added");
        
        // Test concurrent access
        var tasks = new List<Task>();
        for (int i = 0; i < 10; i++)
        {
            int index = i;
            tasks.Add(Task.Run(async () =>
            {
                await cache.SetAsync($"concurrent{index}", $"value{index}");
                var value = await cache.GetAsync($"concurrent{index}");
            }));
        }
        
        await Task.WhenAll(tasks);
        Console.WriteLine("Cache is thread-safe");
        
        // Test retrieval
        var value1 = await cache.GetAsync("key1");
        Console.WriteLine($"Retrieved: {value1}");
        
        // Test expiration
        Console.WriteLine("Waiting for expiration...");
        await Task.Delay(6000);
        
        var expired = await cache.GetAsync("key1");
        if (expired == null)
        {
            Console.WriteLine("SUCCESS: Expired item returned null");
        }
        else
        {
            Console.WriteLine($"FAIL: Expired item returned: {expired}");
        }
        
        // Test Count
        Console.WriteLine($"Cache count: {cache.Count}");
        
        // Test removal
        var removed = await cache.RemoveAsync("key2");
        Console.WriteLine($"Removed key2: {removed}");
        
        Console.WriteLine("SUCCESS: All tests passed!");
    }
}
