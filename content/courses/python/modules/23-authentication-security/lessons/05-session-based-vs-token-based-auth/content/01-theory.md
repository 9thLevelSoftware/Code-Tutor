---
type: "THEORY"
title: "Session-Based Authentication"
---

**How Sessions Work**

Session-based authentication maintains user state on the server:

```
┌─────────┐          ┌─────────┐          ┌──────────┐
│  Client │─────────▶│  Server │─────────▶│  Session │
│         │  Login   │         │ Create   │  Store   │
│         │          │         │          │ (Redis)  │
│         │◀─────────│         │◀─────────│          │
│         │ Session  │         │ Validate │          │
│ Cookie  │─────────▶│         │─────────▶│          │
└─────────┘ Request  └─────────┘          └──────────┘
```

**The Flow:**

1. **Login**: User submits credentials, server validates
2. **Session Creation**: Server generates unique session ID, stores user data in Redis/database
3. **Cookie Response**: Server sends session ID in httpOnly cookie
4. **Subsequent Requests**: Browser automatically sends cookie, server looks up session data
5. **Logout**: Server deletes session from store, cookie becomes worthless

**Key Characteristics:**

| Aspect | Behavior |
|--------|----------|
| Storage | Server-side (Redis, database, memory) |
| Client storage | Small cookie with session ID only |
| Validation | Requires server lookup |
| Revocation | Instant - delete from store |
| Scalability | Needs shared session store across servers |

**Best For:**
- Traditional server-rendered web applications
- When you need instant session invalidation
- When session data is large or sensitive
- Applications where server-side control is preferred