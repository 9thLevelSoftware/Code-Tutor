---
type: "THEORY"
title: "Token-Based Authentication"
---

**How Tokens Work**

Token-based authentication (typically JWT) stores user state in the token itself:

```
┌─────────┐          ┌─────────┐          ┌─────────┐
│  Client │─────────▶│  Server │          │         │
│         │  Login   │ Signs   │          │         │
│         │          │ Token   │          │         │
│         │◀─────────│         │          │         │
│ Stores  │  Token   │         │          │         │
│  Token  │          │         │          │         │
│         │─────────▶│ Validate│          │         │
│         │  Request │ Signature│         │         │
└─────────┘ + Token  └─────────┘          └─────────┘
```

**The Flow:**

1. **Login**: User submits credentials, server validates
2. **Token Creation**: Server creates signed JWT containing user claims (user_id, email, roles)
3. **Client Storage**: Client stores token (localStorage, memory, secure storage)
4. **Subsequent Requests**: Client sends token in Authorization header
5. **Validation**: Server validates signature locally, no database lookup needed
6. **Expiration**: Token expires based on embedded timestamp, cannot be revoked early

**Key Characteristics:**

| Aspect | Behavior |
|--------|----------|
| Storage | Client-side (header, localStorage) |
| Server storage | None (stateless) |
| Validation | Cryptographic signature check |
| Revocation | Difficult - must wait for expiry or use blocklist |
| Scalability | Excellent - no shared state needed |

**Best For:**
- Single Page Applications (SPAs)
- Mobile applications
- Microservices architectures
- Cross-domain authentication (OAuth)
- APIs serving multiple client types