---
type: "KEY_POINT"
title: "Session vs Token Decision Guide"
---

**Use This Decision Framework When Choosing Authentication:**

```
                    ┌─────────────────────────────────────┐
                    │    What type of client?             │
                    └─────────────┬───────────────────────┘
                                  │
              ┌───────────────────┴───────────────────┐
              ▼                                       ▼
    ┌─────────────────┐                     ┌─────────────────┐
    │  Web Browser    │                     │  Mobile/API     │
    │  (Server-       │                     │  Client         │
    │  Rendered)      │                     │                 │
    └────────┬────────┘                     └────────┬────────┘
             │                                        │
             ▼                                        ▼
    ┌─────────────────┐                     ┌─────────────────┐
    │  USE SESSIONS   │                     │  USE JWT TOKENS │
    │                 │                     │                 │
    │  ✓ httpOnly     │                     │  ✓ Stateless    │
    │    cookies      │                     │  ✓ Cross-domain │
    │  ✓ Easy revoke  │                     │  ✓ Mobile-      │
    │  ✓ Familiar     │                     │    friendly     │
    │    security     │                     │  ✓ Offline      │
    │                 │                     │    validation   │
    └─────────────────┘                     └─────────────────┘
```

**Quick Reference Table:**

| Factor | Choose Sessions | Choose Tokens |
|--------|----------------|---------------|
| **Primary client** | Web browser | Mobile app |
| **Server scaling** | Sticky sessions/Redis OK | Horizontal scaling needed |
| **Revocation speed** | Instant | Delayed (or needs blocklist) |
| **Cross-domain** | Same-origin only | Any domain |
| **Offline use** | Not applicable | Validate without server |
| **Complexity** | Simpler mental model | More moving parts |

**Red Flags - When to Reconsider:**

🚩 **Using Sessions when:**
- You have millions of users (Redis memory cost)
- You need to authenticate across 10+ microservices
- Your mobile team is complaining about cookie handling

🚩 **Using Tokens when:**
- You need to instantly ban users (token revocation is hard)
- Your tokens are growing huge with claims
- You're storing sensitive data in the JWT payload

**The Golden Rule:**
> Start with sessions for traditional web apps. Add tokens only when you have a specific requirement they solve (mobile apps, third-party APIs, cross-domain). Hybrid approaches are perfectly valid!
