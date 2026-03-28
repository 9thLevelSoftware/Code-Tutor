---
type: "THEORY"
title: "Session Expiration and Management"
---

**Session Lifecycle Management** is critical for both security and user experience:

**Session Expiration Strategies**

| Strategy | How It Works | Use Case |
|----------|--------------|----------|
| Fixed Expiry | Session dies after set time (e.g., 24h) | Banking, admin panels |
| Sliding Expiry | Session extends on activity | Social media, e-commerce |
| Absolute Expiry | Hard limit regardless of activity | Compliance requirements |
| Idle Timeout | Expires after inactivity period | Corporate applications |

**Best Practices for Session Management:**

1. **Secure Cookie Attributes:**
   - `HttpOnly`: Prevents JavaScript access (XSS protection)
   - `Secure`: Only sent over HTTPS
   - `SameSite`: CSRF protection (Strict or Lax)
   - `Max-Age` or `Expires`: Define session lifetime

2. **Server-Side Session Storage:**
   - Store minimal data in the cookie (just session ID)
   - Keep sensitive session data server-side (Redis, database)
   - Enables instant revocation without waiting for cookie expiry

3. **Session Rotation:**
   - Generate new session ID after login (prevent fixation attacks)
   - Optionally rotate periodically for sensitive operations

4. **Logout Handling:**
   - Clear server-side session data
   - Expire client cookie immediately
   - Invalidate any associated tokens
