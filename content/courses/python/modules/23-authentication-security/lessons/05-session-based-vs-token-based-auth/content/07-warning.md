---
type: "WARNING"
title: "Session Security Vulnerabilities"
---

**Critical session security risks to avoid:**

⚠️ **Session Fixation Attack**
```python
# DANGEROUS: Reusing attacker-provided session ID
session_id = request.cookies.get("session_id")  # Attacker sets this!
user = authenticate(credentials)
store.save(session_id, user)  # Attacker now has valid session!

# SECURE: Always regenerate ID after authentication
session_id = secrets.token_urlsafe(32)  # New ID
user = authenticate(credentials)
store.save(session_id, user)
response.set_cookie("session_id", session_id, httponly=True, secure=True)
```

⚠️ **Session Hijacking via Predictable IDs**
```python
# DANGEROUS: Predictable session IDs
session_id = str(user_id) + str(int(time.time()))  # Easy to guess!

# SECURE: Cryptographically random
session_id = secrets.token_urlsafe(32)  # 256 bits of entropy
```

⚠️ **Missing HttpOnly Cookie Flag**
```python
# DANGEROUS: JavaScript can steal session cookie
response.set_cookie("session_id", session_id)  # Missing flags!

# SECURE: Protect against XSS
response.set_cookie(
    "session_id", 
    session_id,
    httponly=True,      # No JavaScript access
    secure=True,        # HTTPS only
    samesite="Strict",  # CSRF protection
    max_age=86400       # 24 hours
)
```

⚠️ **Session Data Exposure**
```python
# DANGEROUS: Storing sensitive data in cookie
response.set_cookie("user_data", json.dumps({"role": "admin", "ssn": "123-45-6789"}))

# SECURE: Minimal cookie, server-side storage
cookie: session_id only
server: {user_id: 42, role: "admin"}  # SSN never stored
```

⚠️ **No Server-Side Invalidation**
```python
# DANGEROUS: Only clearing client cookie
def logout():
    response.delete_cookie("session_id")  # Attacker still has valid server session!

# SECURE: Destroy server-side session
def logout():
    store.destroy_session(request.cookies["session_id"])
    response.delete_cookie("session_id")
```
