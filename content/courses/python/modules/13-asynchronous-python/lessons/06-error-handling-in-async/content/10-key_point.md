---
type: "KEY_POINT"
title: "Task Exception Handling Summary"
---

**asyncio.Task Exception Methods**

| Method | Behavior | Use Case |
|--------|----------|----------|
| `await task` | Blocks, re-raises exception | When you need the result and want to handle errors |
| `task.result()` | Re-raises stored exception | After confirming task is done |
| `task.exception()` | Returns exception (or None) | Non-blocking error check |

**The Golden Rule:**

> **Every task exception MUST be retrieved** - either by awaiting, calling `result()`, or calling `exception()`. Unretrieved exceptions cause runtime warnings.

**Safe Pattern for Fire-and-Forget Tasks:**

```python
async def safe_fire_and_forget(coro):
    """Safely run a task without waiting for result"""
    async def wrapper():
        try:
            await coro
        except Exception as e:
            logger.error(f"Background task failed: {e}")
    
    asyncio.create_task(wrapper())
```

**When to Use Each Method:**

- **`await task`** - Normal sequential flow, you need the result
- **`task.result()`** - After `await asyncio.wait_for(task, timeout=0)` style checks
- **`task.exception()`** - Polling multiple tasks, collecting errors without blocking

**Checking Task Status:**

```python
if task.done():
    if task.exception() is not None:
        # Handle error
    else:
        # Use task.result()
```
