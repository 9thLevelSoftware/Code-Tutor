using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

// Solution: Thread-safe async cache using C# 13 Lock type
public class AsyncCache<TKey, TValue> where TKey : notnull
{
    // C# 13 Lock type - purpose-built for thread synchronization
    private readonly Lock _lock = new();
    
    // Storage for cached values and their expiration times
    private readonly Dictionary<TKey, TValue> _items = new();
    private readonly Dictionary<TKey, DateTime> _expirations = new();
    
    private readonly TimeSpan _defaultExpiration = TimeSpan.FromSeconds(5);
    
    /// <summary>
    /// Gets a value from the cache if it exists and hasn't expired.
    /// Thread-safe using C# 13 Lock.
    /// </summary>
    public async Task<TValue?> GetAsync(TKey key)
    {
        // Simulate potential async operation (e.g., fetching from remote cache)
        await Task.Yield();
        
        lock (_lock)
        {
            // Check if key exists
            if (!_expirations.TryGetValue(key, out var expiration))
            {
                return default;
            }
            
            // Check if expired
            if (DateTime.UtcNow > expiration)
            {
                // Clean up expired item
                _items.Remove(key);
                _expirations.Remove(key);
                return default;
            }
            
            // Return the value
            return _items.TryGetValue(key, out var value) ? value : default;
        }
    }
    
    /// <summary>
    /// Sets a value in the cache with optional expiration.
    /// Thread-safe using C# 13 Lock.
    /// </summary>
    public async Task SetAsync(TKey key, TValue value, TimeSpan? expiration = null)
    {
        // Simulate potential async validation or serialization
        await Task.Yield();
        
        var expirationTime = DateTime.UtcNow.Add(expiration ?? _defaultExpiration);
        
        lock (_lock)
        {
            _items[key] = value;
            _expirations[key] = expirationTime;
        }
    }
    
    /// <summary>
    /// Removes a value from the cache.
    /// Thread-safe using C# 13 Lock.
    /// </summary>
    public async Task<bool> RemoveAsync(TKey key)
    {
        // Simulate potential async cleanup
        await Task.Yield();
        
        lock (_lock)
        {
            var found = _items.ContainsKey(key);
            
            if (found)
            {
                _items.Remove(key);
                _expirations.Remove(key);
            }
            
            return found;
        }
    }
    
    /// <summary>
    /// Gets the count of non-expired items.
    /// Cleans up expired items while counting.
    /// Thread-safe using C# 13 Lock.
    /// </summary>
    public int Count
    {
        get
        {
            lock (_lock)
            {
                var now = DateTime.UtcNow;
                var expiredKeys = new List<TKey>();
                
                // Find expired keys
                foreach (var kvp in _expirations)
                {
                    if (now > kvp.Value)
                    {
                        expiredKeys.Add(kvp.Key);
                    }
                }
                
                // Clean up expired items
                foreach (var key in expiredKeys)
                {
                    _items.Remove(key);
                    _expirations.Remove(key);
                }
                
                return _items.Count;
            }
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
        
        // Test concurrent access - demonstrates thread safety
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
        
        // Test Count (should show key2 and concurrent items, not key1)
        Console.WriteLine($"Cache count: {cache.Count}");
        
        // Test removal
        var removed = await cache.RemoveAsync("key2");
        Console.WriteLine($"Removed key2: {removed}");
        
        // Verify removal
        var afterRemove = await cache.GetAsync("key2");
        Console.WriteLine($"key2 after removal: {afterRemove ?? "null"}");
        
        Console.WriteLine("SUCCESS: All tests passed!");
    }
}
