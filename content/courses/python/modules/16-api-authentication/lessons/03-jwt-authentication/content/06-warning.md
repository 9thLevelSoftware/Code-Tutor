---
type: "WARNING"
title: "JWT Security Critical Mistakes"
---

**JWT Vulnerabilities to Avoid:**

❌ **NEVER store secrets in JWT payload**

```python
# WRONG: Sensitive data in payload (visible to anyone!)
payload = {
    "user_id": 123,
    "credit_card": "4111-1111-1111-1111",  # EXPOSED!
    "ssn": "123-45-6789"  # EXPOSED!
}

# JWT is BASE64 encoded, NOT encrypted!
# Anyone can decode: base64.b64decode(token.split('.')[1])

# RIGHT: Only store identifiers
payload = {
    "sub": str(user_id),  # User ID only
    "exp": datetime.now(timezone.utc) + timedelta(minutes=15)
}
```

❌ **NEVER use weak or hardcoded secrets**

```python
# WRONG: Weak secret
SECRET_KEY = os.getenv("SECRET_KEY", "development-fallback-secret")  # Easily guessed!
SECRET_KEY = os.getenv("SECRET_KEY", "development-fallback-secret")  # Too short!

# RIGHT: Strong, environment-based secret
import secrets
# Generate once and store in env:
# SECRET_KEY = secrets.token_urlsafe(32)
SECRET_KEY = os.environ["JWT_SECRET_KEY"]  # 256+ bits
```

❌ **NEVER skip token expiration**

```python
# WRONG: Token never expires
payload = {"sub": user_id}  # No expiration!

# RIGHT: Short-lived access tokens
payload = {
    "sub": str(user_id),
    "exp": datetime.now(timezone.utc) + timedelta(minutes=15)
}
```

❌ **NEVER accept algorithm from token**

```python
# WRONG: Algorithm confusion attack
# Attacker can change "alg": "none" and bypass verification!
jwt.decode(token, options={"verify_signature": False})

# RIGHT: Always specify expected algorithm
jwt.decode(token, SECRET_KEY, algorithms=["HS256"])
```

❌ **NEVER transmit tokens without HTTPS**

```
HTTP: Token visible to network sniffers!
→ Attacker captures token → Account compromised

HTTPS: Token encrypted in transit
→ Attacker sees encrypted noise → Token safe
```

**JWT Security Checklist:**
- [ ] No sensitive data in payload
- [ ] Strong secret (256+ bits)
- [ ] Short expiration (15-60 min)
- [ ] Algorithm explicitly specified
- [ ] HTTPS only
- [ ] Refresh token rotation
- [ ] Token stored securely (httpOnly cookies)
