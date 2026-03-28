---
type: "THEORY"
title: "Introduction to Authentication Architectures"
---

**Understanding How Users Prove Their Identity**

Authentication is the process of verifying who a user claims to be. In backend development, two dominant architectural patterns have emerged:

**Session-Based Authentication (Stateful)**
- Server creates and stores session data
- Client receives only a session identifier (cookie)
- Server validates every request by looking up session storage
- Traditional approach used by frameworks like Django, Rails, Express

**Token-Based Authentication (Stateless)**
- Server generates a signed token containing user data
- Client stores and sends token with every request
- Server validates token signature without database lookup
- Modern approach popularized by JWT, OAuth2

**Why This Matters for Finance Tracker:**

Your finance application handles sensitive financial data. The authentication choice affects:
- How quickly you can revoke access when suspicious activity is detected
- Whether users stay logged in across devices
- How your API scales with more users
- Security trade-offs between instant revocation and token tampering

This lesson compares both approaches with working Python implementations so you can make informed decisions for your application's security architecture.