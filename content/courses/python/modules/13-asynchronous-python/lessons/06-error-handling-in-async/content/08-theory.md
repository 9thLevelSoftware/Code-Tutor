---
type: "THEORY"
title: "Task Exception Handling - The Critical Missing Piece"
---

**Understanding asyncio.Task Exception Handling**

When you create an `asyncio.Task`, exceptions behave differently than with `await`:

```python
task = asyncio.create_task(might_fail())
# Task is running in the background...
```

**The Problem: Stored Exceptions**

When a task raises an exception, it's **stored** inside the Task object:

```python
async def failing_task():
    await asyncio.sleep(0.1)
    raise ValueError("Task failed!")

task = asyncio.create_task(failing_task())
await asyncio.sleep(0.2)  # Let task complete

# The exception is stored, NOT raised!
print("Task done:", task.done())  # True
print("Task failed:", task.exception())  # ValueError("Task failed!")
```

**The Danger: Unretrieved Exceptions**

If you never retrieve the exception, asyncio logs a **warning** and may crash:

```python
import asyncio

async def main():
    task = asyncio.create_task(failing_task())
    # Never await or check the task!
    await asyncio.sleep(0.5)
    # Program exits with warning:
    # "Task exception was never retrieved"

asyncio.run(main())
```

**Three Ways to Handle Task Exceptions:**

**1. Using `await` (re-raises the exception):**
```python
task = asyncio.create_task(failing_task())
try:
    result = await task  # Re-raises the stored exception
except ValueError as e:
    print(f"Caught: {e}")
```

**2. Using `task.result()` (re-raises the exception):**
```python
task = asyncio.create_task(failing_task())
await asyncio.sleep(0.2)  # Let it finish

try:
    result = task.result()  # Re-raises if task failed
except ValueError as e:
    print(f"Caught: {e}")
```

**3. Using `task.exception()` (returns exception without raising):**
```python
task = asyncio.create_task(failing_task())
await asyncio.sleep(0.2)

exc = task.exception()  # Returns None if successful
if exc is not None:
    print(f"Task failed with: {exc}")
else:
    result = task.result()
    print(f"Task succeeded: {result}")
```

**Important: Check `task.done()` First**

```python
task = asyncio.create_task(some_task())

# WRONG - may raise InvalidStateError if not done!
exc = task.exception()

# CORRECT - wait for completion first
await task  # or await asyncio.sleep() etc.
if task.done():
    exc = task.exception()
```

**Best Practices:**

1. **Always await or check background tasks** - Unretrieved exceptions cause warnings
2. **Use `task.exception()` for non-blocking error checks** - When you need to check status without blocking
3. **Re-raise or handle** - Don't let exceptions silently disappear in background tasks
