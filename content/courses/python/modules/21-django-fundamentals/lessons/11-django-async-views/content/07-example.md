---
type: "EXAMPLE"
title: "Async Authentication in Django 5.1"
---

**Async Auth in Django 5.1**

Django 5.1 requires using sync_to_async to wrap authentication functions in async views:

| Sync Function | Async Pattern (Django 5.1) |
|-------------|---------------------------|
| `authenticate()` | `await sync_to_async(authenticate)(...)` |
| `login()` | `await sync_to_async(login)(request, user)` |
| `logout()` | `await sync_to_async(logout)(request)` |
| `get_user()` | `request.user` (works in async views) |

**Async Decorators:**
- Use `@login_required` with async views - it handles both
- For legacy auth code, wrap with `@sync_to_async`

**Request User in Async Views:**
```python
async def my_view(request):
    # request.user works fine in async views
    user = request.user
```

```python
from django.http import JsonResponse
from django.contrib.auth import authenticate, login, logout
from django.views.decorators.http import require_http_methods
from asgiref.sync import sync_to_async
import json


@require_http_methods(["POST"])
async def async_login_view(request):
    """Async login endpoint using sync_to_async."""
    data = json.loads(request.body)
    
    # Wrap sync authenticate with sync_to_async
    auth_fn = sync_to_async(authenticate)
    user = await auth_fn(
        request=request,
        username=data.get("username"),
        password=data.get("password")
    )
    
    if user is not None:
        # Wrap sync login with sync_to_async
        login_fn = sync_to_async(login)
        await login_fn(request, user)
        return JsonResponse({
            "status": "success",
            "user": user.username
        })
    
    return JsonResponse(
        {"error": "Invalid credentials"},
        status=401
    )


@require_http_methods(["POST"])
async def async_logout_view(request):
    """Async logout endpoint."""
    logout_fn = sync_to_async(logout)
    await logout_fn(request)
    return JsonResponse({"status": "logged_out"})


# Using with login_required decorator
from django.contrib.auth.decorators import login_required

@login_required
async def protected_async_view(request):
    """Protected async view - login_required works with async."""
    return JsonResponse({
        "message": f"Hello, {request.user.username}!",
        "authenticated": True
    })


print("=== Django 5.1 Async Authentication ===")

print("\nUsing sync_to_async with Auth Functions:")
print("  auth_fn = sync_to_async(authenticate)")
print("  user = await auth_fn(request, username, password)")
print("  login_fn = sync_to_async(login)")
print("  await login_fn(request, user)")

print("\nDecorators Work with Async:")
print("  @login_required")
print("  async def my_view(request):")
print("      ...")

print("\nMigration Tip:")
print("  Use sync_to_async() to wrap sync auth functions")
print("  Add 'await' before the wrapped function calls")
```
