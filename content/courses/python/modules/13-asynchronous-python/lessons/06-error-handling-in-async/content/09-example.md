---
type: "EXAMPLE"
title: "Task Exception Handling Patterns"
---

**Pattern 1: Basic task.exception() Usage**

```python
import asyncio

async def successful_task():
    await asyncio.sleep(0.1)
    return "Success!"

async def failing_task():
    await asyncio.sleep(0.1)
    raise ValueError("Something went wrong!")

async def demo_basic_exception_handling():
    """Basic task.exception() usage"""
    print("=== Basic task.exception() ===")
    
    # Create both tasks
    good_task = asyncio.create_task(successful_task())
    bad_task = asyncio.create_task(failing_task())
    
    # Let them complete
    await asyncio.sleep(0.2)
    
    # Check the successful task
    if good_task.done():
        exc = good_task.exception()
        if exc is None:
            result = good_task.result()
            print(f"  Good task: {result}")
        else:
            print(f"  Good task failed: {exc}")
    
    # Check the failing task
    if bad_task.done():
        exc = bad_task.exception()
        if exc is None:
            result = bad_task.result()
            print(f"  Bad task: {result}")
        else:
            print(f"  Bad task failed with: {type(exc).__name__}: {exc}")
```

**Pattern 2: Using await vs task.result()**

```python
async def demo_await_vs_result():
    """Difference between await and task.result()"""
    print("\n=== await vs task.result() ===")
    
    # Method 1: await (blocks and re-raises)
    task = asyncio.create_task(failing_task())
    try:
        result = await task  # Blocks and re-raises exception
        print(f"  Result: {result}")
    except ValueError as e:
        print(f"  await caught: {e}")
    
    # Method 2: task.result() (re-raises stored exception)
    task = asyncio.create_task(failing_task())
    await asyncio.sleep(0.2)  # Wait for completion
    try:
        result = task.result()  # Re-raises the stored exception
        print(f"  Result: {result}")
    except ValueError as e:
        print(f"  task.result() caught: {e}")
```

**Pattern 3: Non-blocking Error Checking**

```python
async def demo_non_blocking_check():
    """Check multiple tasks without blocking on each"""
    print("\n=== Non-blocking Error Checking ===")
    
    tasks = [
        asyncio.create_task(successful_task()),
        asyncio.create_task(failing_task()),
        asyncio.create_task(successful_task()),
    ]
    
    # Wait a bit for tasks to complete
    await asyncio.sleep(0.2)
    
    # Check all tasks without blocking
    results = []
    errors = []
    
    for i, task in enumerate(tasks):
        if not task.done():
            print(f"  Task {i}: still running")
            continue
            
        exc = task.exception()
        if exc is not None:
            errors.append((i, exc))
            print(f"  Task {i}: FAILED - {exc}")
        else:
            result = task.result()
            results.append((i, result))
            print(f"  Task {i}: OK - {result}")
    
    print(f"  Summary: {len(results)} succeeded, {len(errors)} failed")
```

**Pattern 4: The Unretrieved Exception Warning**

```python
async def demo_unretrieved_warning():
    """Demonstrate the unretrieved exception warning"""
    print("\n=== Unretrieved Exception Warning ===")
    
    # Create a failing task but NEVER check it
    task = asyncio.create_task(failing_task())
    
    # Don't await or check the task!
    # This will cause: "Task exception was never retrieved"
    
    await asyncio.sleep(0.2)  # Let task fail
    
    print("  Task completed but exception was NOT retrieved")
    print("  (Check console for warning message)")
    print("  Warning: 'Task exception was never retrieved'")
    
    # To fix, we should have done:
    # await task  # or task.exception() or task.result()
```

**Pattern 5: Safe Task Wrapper**

```python
async def safe_task_wrapper(coro, task_name="unnamed"):
    """Wrapper that safely handles exceptions"""
    try:
        result = await coro
        return {"status": "success", "result": result}
    except Exception as e:
        return {"status": "error", "task": task_name, "error": str(e)}

async def demo_safe_wrapper():
    """Using safe wrapper pattern"""
    print("\n=== Safe Task Wrapper ===")
    
    # Instead of raw tasks, wrap them
    task1 = asyncio.create_task(safe_task_wrapper(successful_task(), "task1"))
    task2 = asyncio.create_task(safe_task_wrapper(failing_task(), "task2"))
    
    # Now we can safely await without try/except
    result1 = await task1
    result2 = await task2
    
    print(f"  Task1: {result1}")
    print(f"  Task2: {result2}")
```

**Pattern 6: Multiple Tasks with Exception Aggregation**

```python
async def fetch_with_timeout(url, timeout=1.0):
    """Simulated fetch that might fail"""
    await asyncio.sleep(0.05)
    if "error" in url:
        raise ConnectionError(f"Failed to connect to {url}")
    return f"Data from {url}"

async def demo_exception_aggregation():
    """Aggregate exceptions from multiple tasks"""
    print("\n=== Exception Aggregation ===")
    
    urls = ["good.com", "error.com", "also-good.com", "timeout.com/error"]
    
    # Create tasks
    tasks = {url: asyncio.create_task(fetch_with_timeout(url)) for url in urls}
    
    # Wait for all with individual handling
    results = {}
    errors = {}
    
    for url, task in tasks.items():
        try:
            results[url] = await task
        except Exception as e:
            errors[url] = e
            print(f"  {url}: FAILED - {type(e).__name__}: {e}")
    
    print(f"\n  Successful: {list(results.keys())}")
    print(f"  Failed: {list(errors.keys())}")
```

```python
async def main():
    """Run all demos"""
    await demo_basic_exception_handling()
    await demo_await_vs_result()
    await demo_non_blocking_check()
    await demo_unretrieved_warning()
    await demo_safe_wrapper()
    await demo_exception_aggregation()
    
    print("\n=== Key Takeaways ===")
    print("1. task.exception() returns the exception without raising")
    print("2. task.result() re-raises the stored exception")
    print("3. await task also re-raises the stored exception")
    print("4. Always retrieve exceptions to avoid warnings!")

asyncio.run(main())
```
