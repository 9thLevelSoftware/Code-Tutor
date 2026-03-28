---
type: "EXAMPLE"
title: "Creating JWT Tokens"
---

**Token Creation with PyJWT**

This example shows how to create and decode JWT tokens using the `pyjwt` library.

**Key Concepts:**
- `jwt.encode()`: Creates a token from payload and secret
- `jwt.decode()`: Verifies and extracts payload from token
- `exp` claim: Automatic expiration checking
- Algorithm choice: HS256 is secure for most use cases

**Installation:**
```bash
uv add pyjwt
```

```python
# uv add pyjwt

import jwt
from datetime import datetime, timedelta, timezone

SECRET_KEY = os.getenv("SECRET_KEY", "development-fallback-secret")  # In production: from environment
ALGORITHM = "HS256"
ACCESS_TOKEN_EXPIRE_MINUTES = 30

def create_access_token(data: dict, expires_delta: timedelta | None = None) -> str:
    """Create a JWT access token.
    
    Args:
        data: Payload to encode (e.g., {"sub": "user@example.com"})
        expires_delta: Optional custom expiration time
        
    Returns:
        Encoded JWT token string
    """
    to_encode = data.copy()
    expire = datetime.now(timezone.utc) + (expires_delta or timedelta(minutes=15))
    to_encode.update({"exp": expire})
    return jwt.encode(to_encode, SECRET_KEY, algorithm=ALGORITHM)

def decode_token(token: str) -> dict:
    """Decode and verify a JWT token.
    
    Args:
        token: The JWT token string
        
    Returns:
        Decoded payload dictionary
        
    Raises:
        jwt.ExpiredSignatureError: If token has expired
        jwt.InvalidTokenError: If token is invalid
    """
    return jwt.decode(token, SECRET_KEY, algorithms=[ALGORITHM])

# Demonstration
print("=== JWT Token Creation Demo ===")
print()

# Create a token for a user
user_data = {"sub": "alice@example.com", "role": "admin"}
token = create_access_token(user_data, timedelta(minutes=30))

print(f"User data: {user_data}")
print(f"\nGenerated token:")
print(f"  {token[:50]}...")
print(f"  Length: {len(token)} characters")
print()

# Decode the token
decoded = decode_token(token)
print(f"Decoded payload:")
print(f"  sub: {decoded['sub']}")
print(f"  role: {decoded['role']}")
print(f"  exp: {datetime.fromtimestamp(decoded['exp'])}")
print()

# Show token structure
parts = token.split('.')
print("Token structure:")
print(f"  Header:    {parts[0][:30]}...")
print(f"  Payload:   {parts[1][:30]}...")
print(f"  Signature: {parts[2][:30]}...")
```
