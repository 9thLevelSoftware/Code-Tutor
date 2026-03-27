---
type: "EXAMPLE"
title: "Creating Access and Refresh Tokens with PyJWT"
---

**Finance Tracker JWT Implementation:**

**Installation:**
```bash
pip install PyJWT
```

```python
import jwt
from datetime import datetime, timedelta, timezone
from typing import Optional
import secrets

# Configuration
SECRET_KEY = os.getenv("SECRET_KEY", "development-fallback-secret")  # In production: use environment variable
ALGORITHM = "HS256"
ACCESS_TOKEN_EXPIRE_MINUTES = 15
REFRESH_TOKEN_EXPIRE_DAYS = 7

def create_access_token(user_id: int, email: str, role: str = "user") -> str:
    """
    Create a short-lived access token for API authentication.
    Contains user identity and permissions.
    """
    payload = {
        "sub": str(user_id),          # Subject (user identifier)
        "email": email,                 # User email for display
        "role": role,                   # User role for authorization
        "type": "access",               # Token type
        "iat": datetime.now(timezone.utc),  # Issued at
        "exp": datetime.now(timezone.utc) + timedelta(minutes=ACCESS_TOKEN_EXPIRE_MINUTES)
    }
    return jwt.encode(payload, SECRET_KEY, algorithm=ALGORITHM)

def create_refresh_token(user_id: int) -> str:
    """
    Create a long-lived refresh token.
    Only contains user ID - minimal data for security.
    """
    payload = {
        "sub": str(user_id),
        "type": "refresh",
        "jti": secrets.token_hex(16),   # Unique ID for token revocation
        "iat": datetime.now(timezone.utc),
        "exp": datetime.now(timezone.utc) + timedelta(days=REFRESH_TOKEN_EXPIRE_DAYS)
    }
    return jwt.encode(payload, SECRET_KEY, algorithm=ALGORITHM)

def create_tokens(user_id: int, email: str, role: str = "user") -> dict:
    """
    Create both access and refresh tokens for a user.
    Called after successful login.
    """
    return {
        "access_token": create_access_token(user_id, email, role),
        "refresh_token": create_refresh_token(user_id),
        "token_type": "Bearer",
        "expires_in": ACCESS_TOKEN_EXPIRE_MINUTES * 60  # Seconds
    }

# Example: User logs in to Finance Tracker
print("Finance Tracker - JWT Token Generation")
print("=" * 50)

tokens = create_tokens(user_id=42, email="alice@example.com", role="user")

print(f"\nAccess Token (first 50 chars): {tokens['access_token'][:50]}...")
print(f"Refresh Token (first 50 chars): {tokens['refresh_token'][:50]}...")
print(f"Token Type: {tokens['token_type']}")
print(f"Expires In: {tokens['expires_in']} seconds")

# Decode to show structure (without verification for demo)
print("\n--- Access Token Payload ---")
access_payload = jwt.decode(tokens['access_token'], SECRET_KEY, algorithms=[ALGORITHM])
for key, value in access_payload.items():
    print(f"  {key}: {value}")
```
