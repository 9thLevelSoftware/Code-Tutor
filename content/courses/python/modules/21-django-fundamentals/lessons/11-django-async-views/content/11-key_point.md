---
type: "KEY_POINT"
title: "Key Takeaways"
---

**Django Async Views - Summary**

**Getting Started:**
1. Use `async def` for async views
2. Run with ASGI server: `uvicorn project.asgi:application`
3. Mix sync and async views freely - Django handles it

**Django 5.1 Async ORM:**
- `aall()`, `afirst()`, `alast()` - Async iteration
- `acount()`, `aexists()` - Async aggregation
- `acreate()`, `aupdate()`, `adelete()` - Async mutations
- `aget_or_create()`, `aupdate_or_create()` - Async get-or-create

**sync_to_async:**
- Wrap sync functions: `@sync_to_async`
- Use for legacy code, complex queries, sync libraries
- Use for authentication functions in async views
- `thread_sensitive=True` for Django ORM (default)

**Async Authentication:**
- Use `sync_to_async()` to wrap sync auth functions
- Standard decorators work with async views

**Performance Guidelines:**
- Use async for I/O-bound work (APIs, multiple queries)
- Keep sync for CPU-bound work and simple CRUD
- Use `asyncio.gather()` for concurrent operations
- Batch sync_to_async calls when possible

**Common Pitfalls:**
- Don't use `list()` on querysets in async views
- Don't use `time.sleep()` - use `asyncio.sleep()`
- Don't forget to `await` async calls
- Remember filtering is sync, terminal methods are async