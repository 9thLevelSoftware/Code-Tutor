---
type: "KEY_POINT"
title: "JWT Authentication Takeaways"
---

**Core Concepts:**

1. **JWT Structure** - Three parts: header.payload.signature
   - Header: Algorithm info
   - Payload: User claims (sub, exp, etc.)
   - Signature: Verifies integrity

2. **Use PyJWT** - Modern, maintained library
   ```python
   import jwt
   token = jwt.encode(payload, secret, algorithm="HS256")
   data = jwt.decode(token, secret, algorithms=["HS256"])
   ```

3. **Token Flow:**
   - Login: Verify credentials, return token
   - Requests: Include token in Authorization header
   - Server: Validate token, extract user

4. **Security Best Practices:**
   - Short expiration (15-60 minutes)
   - Store secrets in environment variables
   - Use HTTPS in production
   - Handle expiration gracefully

**Quick Reference:**

```python
# Installation
uv add pyjwt

# Create token
token = jwt.encode(
    {"sub": email, "exp": datetime.now(timezone.utc) + timedelta(minutes=30)},
    SECRET_KEY,
    algorithm="HS256"
)

# Verify token
try:
    payload = jwt.decode(token, SECRET_KEY, algorithms=["HS256"])
except jwt.ExpiredSignatureError:
    # Handle expired
except jwt.InvalidTokenError:
    # Handle invalid
```

**Security Checklist:**
- [ ] Use pyjwt (not python-jose)
- [ ] Set appropriate expiration times
- [ ] Store SECRET_KEY in environment
- [ ] Always validate tokens on protected routes
- [ ] Return generic error messages
- [ ] Use HTTPS in production