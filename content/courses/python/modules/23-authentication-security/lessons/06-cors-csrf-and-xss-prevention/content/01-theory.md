---
type: "THEORY"
title: "Understanding CORS - Cross-Origin Resource Sharing"
---

**Estimated Time**: 45 minutes
**Difficulty**: Intermediate

## Introduction

CORS (Cross-Origin Resource Sharing) is a security mechanism that controls how web pages can request resources from a different domain than the one serving the page. Without CORS, browsers would block all cross-origin requests, preventing legitimate interactions between trusted domains.

**Real-World Context**: When your frontend at `app.example.com` needs to call your API at `api.example.com`, CORS determines whether that request succeeds or fails. Misconfigured CORS is a common source of "blocked by CORS policy" errors in browser consoles.

## Why CORS Exists

The Same-Origin Policy (SOP) prevents malicious websites from reading data from other domains. However, modern applications need legitimate cross-origin access:
- **APIs**: Frontend and backend on different subdomains
- **CDNs**: Loading assets from distributed servers
- **Third-party services**: Authentication, payments, analytics

CORS provides a controlled way to relax SOP restrictions.

## How CORS Works

1. **Browser sends preflight request** (OPTIONS) for non-simple requests
2. **Server responds with allowed origins, methods, headers**
3. **Browser checks if actual request is permitted**
4. **Request proceeds or is blocked** based on CORS headers

## Key CORS Headers

| Header | Purpose |
|--------|---------|
| `Access-Control-Allow-Origin` | Which origins can access resources |
| `Access-Control-Allow-Methods` | Allowed HTTP methods |
| `Access-Control-Allow-Headers` | Allowed request headers |
| `Access-Control-Allow-Credentials` | Whether cookies/auth headers allowed |
| `Access-Control-Max-Age` | Preflight result cache duration |

## Python Implementation

```python
from fastapi import FastAPI
from fastapi.middleware.cors import CORSMiddleware

app = FastAPI()

# Configure CORS
app.add_middleware(
    CORSMiddleware,
    allow_origins=["https://app.example.com"],  # Specific origin, not "*"
    allow_credentials=True,
    allow_methods=["GET", "POST", "PUT", "DELETE"],
    allow_headers=["Authorization", "Content-Type"],
    max_age=600,  # Cache preflight for 10 minutes
)
```

## Security Best Practices

- **Never use `allow_origins=["*"]` with credentials**—it's a security vulnerability
- **Specify exact origins** rather than pattern matching
- **Limit allowed methods** to only those your API actually uses
- **Validate preflight caching** balances performance with policy updates

Proper CORS configuration protects your API while enabling legitimate cross-origin use cases. It has proper frontmatter so the loader will not fail to parse it.
