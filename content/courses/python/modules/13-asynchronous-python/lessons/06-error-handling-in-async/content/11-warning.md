---
type: "WARNING"
title: "Task Exception Handling Pitfalls"
---

### Critical Mistakes with Task Exceptions

**1. Never Retrieving Task Exceptions**

```python
# WRONG - Exception never retrieved!
task = asyncio.create_task(might_fail())
# Task fails but we never check it...
# RuntimeWarning: Task exception was never retrieved

# CORRECT - Always retrieve the exception
task = asyncio.create_task(might_fail())
try:
    await task
except ValueError:
    pass  # Exception is now retrieved (and handled)
```

**2. Calling result()/exception() on Running Task**

```python
# WRONG - Task may not be done!
task = asyncio.create_task(slow_task())
result = task.result()  # InvalidStateError!

# CORRECT - Wait for completion first
task = asyncio.create_task(slow_task())
await task  # or await asyncio.sleep() if non-blocking needed
if task.done():
    result = task.result()
```

**3. Calling result() Without Checking exception() First**

```python
# DANGEROUS - result() re-raises!
task = asyncio.create_task(might_fail())
await asyncio.sleep(0.5)

# Both of these re-raise the exception:
result = task.result()  # Raises ValueError!

# SAFER approach - check first:
if task.exception() is not None:
    print(f"Failed: {task.exception()}")
else:
    result = task.result()
```

**4. Ignoring Exceptions in Background Tasks**

```python
# WRONG - Silent failures
def start_background_work():
    asyncio.create_task(background_task())  # Exception never seen!

# CORRECT - Wrap to catch and log
def start_background_work():
    async def wrapped():
        try:
            await background_task()
        except Exception as e:
            logger.error(f"Background task failed: {e}")
    
    asyncio.create_task(wrapped())
```

**5. Misunderstanding gather() vs Individual Tasks**

```python
# gather() - exceptions raised immediately
try:
    results = await asyncio.gather(task1, task2)  # First exception stops await
except ValueError:
    pass

# Individual tasks - exceptions stored
task = asyncio.create_task(might_fail())
await asyncio.sleep(0.1)  # Let it fail
# No exception raised here - it's stored in task!
```

**6. Not Using return_exceptions=True with gather()**

```python
# WRONG - One failure cancels all
tasks = [asyncio.create_task(t()) for t in task_funcs]
try:
    results = await asyncio.gather(*tasks)  # First fail cancels rest!
except:
    pass  # Some results lost forever

# CORRECT - Get all results including failures
tasks = [asyncio.create_task(t()) for t in task_funcs]
results = await asyncio.gather(*tasks, return_exceptions=True)
for r in results:
    if isinstance(r, Exception):
        # Handle individual error
```
