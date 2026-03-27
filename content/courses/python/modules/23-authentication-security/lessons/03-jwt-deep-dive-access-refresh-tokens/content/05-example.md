---
type: "EXAMPLE"
title: "Verifying and Decoding JWTs"
---

**Secure token verification with proper error handling:**

```python
import jwt
from datetime import datetime, timedelta, timezone
from typing import Optional, Dict, Any
from enum import Enum

SECRET_KEY = os.getenv("SECRET_KEY", "development-fallback-secret")
ALGORITHM = "HS256"

class TokenError(Enum):
    EXPIRED = "Token has expired"
    INVALID = "Invalid token"
    WRONG_TYPE = "Wrong token type"
    MALFORMED = "Malformed token"

class TokenVerificationResult:
    def __init__(self, success: bool, payload: Optional[Dict] = None, 
                 error: Optional[TokenError] = None):
        self.success = success
        self.payload = payload
        self.error = error

def verify_token(token: str, expected_type: str = "access") -> TokenVerificationResult:
    """
    Verify a JWT and return the payload if valid.
    
    Args:
        token: The JWT string to verify
        expected_type: 'access' or 'refresh'
    
    Returns:
        TokenVerificationResult with success status and payload or error
    """
    try:
        # Decode and verify signature + expiration
        payload = jwt.decode(
            token, 
            SECRET_KEY, 
            algorithms=[ALGORITHM],
            options={"require": ["exp", "sub", "type"]}  # Required claims
        )
        
        # Verify token type
        if payload.get("type") != expected_type:
            return TokenVerificationResult(
                success=False, 
                error=TokenError.WRONG_TYPE
            )
        
        return TokenVerificationResult(success=True, payload=payload)
        
    except jwt.ExpiredSignatureError:
        return TokenVerificationResult(success=False, error=TokenError.EXPIRED)
    except jwt.InvalidTokenError:
        return TokenVerificationResult(success=False, error=TokenError.INVALID)
    except Exception:
        return TokenVerificationResult(success=False, error=TokenError.MALFORMED)

def get_user_from_token(token: str) -> Optional[Dict[str, Any]]:
    """
    Extract user information from a valid access token.
    Returns None if token is invalid.
    """
    result = verify_token(token, expected_type="access")
    if result.success:
        return {
            "user_id": int(result.payload["sub"]),
            "email": result.payload.get("email"),
            "role": result.payload.get("role", "user")
        }
    return None

# Demonstration
print("Token Verification Examples")
print("=" * 50)

# Create a valid token
valid_token = jwt.encode(
    {
        "sub": "42",
        "email": "alice@example.com",
        "role": "user",
        "type": "access",
        "exp": datetime.now(timezone.utc) + timedelta(minutes=15)
    },
    SECRET_KEY,
    algorithm=ALGORITHM
)

# Create an expired token
expired_token = jwt.encode(
    {
        "sub": "42",
        "type": "access",
        "exp": datetime.now(timezone.utc) - timedelta(minutes=5)  # Already expired
    },
    SECRET_KEY,
    algorithm=ALGORITHM
)

# Test valid token
print("\n1. Valid Access Token:")
result = verify_token(valid_token)
print(f"   Success: {result.success}")
if result.success:
    user = get_user_from_token(valid_token)
    print(f"   User: {user}")

# Test expired token
print("\n2. Expired Token:")
result = verify_token(expired_token)
print(f"   Success: {result.success}")
print(f"   Error: {result.error.value if result.error else 'None'}")

# Test invalid token
print("\n3. Invalid Token:")
result = verify_token("not.a.valid.token")
print(f"   Success: {result.success}")
print(f"   Error: {result.error.value if result.error else 'None'}")

# Test wrong token type (using access token as refresh)
print("\n4. Wrong Token Type (access used as refresh):")
result = verify_token(valid_token, expected_type="refresh")
print(f"   Success: {result.success}")
print(f"   Error: {result.error.value if result.error else 'None'}")
```
