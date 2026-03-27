---
type: "WARNING"
title: "Password Security Critical Mistakes"
---

**Security Vulnerabilities to Avoid:**

❌ **NEVER store plain text passwords**

```python
# CATASTROPHIC: One breach exposes all passwords
class User(Base):
    password: str  # Plain text!

# RIGHT: Store hashes only
class User(Base):
    password_hash: str  # Hash, never reversible
```

❌ **NEVER use weak hashing algorithms**

```python
# WRONG: MD5/SHA1/SHA256 are NOT for passwords
from passlib.context import CryptContext

pwd_context = CryptContext(schemes=["bcrypt"], deprecated="auto")
hashed = pwd_context.hash(password)
# Too fast! Attacker can try billions per second

# RIGHT: Use bcrypt (intentionally slow)
from passlib.context import CryptContext
pwd_context = CryptContext(schemes=["bcrypt"])
hashed = pwd_context.hash(password)
# ~100ms per hash - brute force impractical
```

❌ **NEVER log passwords**

```python
# WRONG: Password in logs
logger.info(f"User login: {username}, password: {password}")

# RIGHT: Never log sensitive data
logger.info(f"User login attempt: {username}")
```

❌ **NEVER compare passwords directly**

```python
# WRONG: Timing attack vulnerability
if stored_hash == computed_hash:  # Can leak info!
    return True

# RIGHT: Use constant-time comparison
from passlib.context import CryptContext
pwd_context.verify(password, stored_hash)  # Timing-safe
```

❌ **NEVER use default/weak cost factors**

```python
# WRONG: Cost too low (too fast)
CryptContext(schemes=["bcrypt"], bcrypt__rounds=4)

# RIGHT: Use appropriate rounds (12+ recommended)
CryptContext(schemes=["bcrypt"], bcrypt__rounds=12)
# Adjust based on server capacity and security needs
```

**Security Checklist:**
- [ ] Using bcrypt or argon2 (NOT MD5/SHA)
- [ ] Cost factor of 12 or higher
- [ ] Passwords never logged
- [ ] Constant-time comparison
- [ ] Password requirements enforced
