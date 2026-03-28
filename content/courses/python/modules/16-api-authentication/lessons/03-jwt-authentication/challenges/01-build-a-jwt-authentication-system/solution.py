import os
import jwt
from datetime import datetime, timedelta, timezone
from typing import Optional

SECRET_KEY = os.getenv("SECRET_KEY", "development-fallback-secret")
ALGORITHM = "HS256"

def create_access_token(data: dict, expires_delta: Optional[timedelta] = None) -> str:
    """Create a JWT access token with expiration.
    
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

def get_current_user(token: str) -> Optional[dict]:
    """Get user from token.
    
    Returns user dict on success, None on failure.
    Handles ExpiredSignatureError and InvalidTokenError.
    """
    try:
        payload = decode_token(token)
        email = payload.get("sub")
        if email is None:
            return None
        # Return user data from token
        return {
            "email": email,
            "role": payload.get("role"),
            "exp": payload.get("exp")
        }
    except jwt.ExpiredSignatureError:
        print("  Error: Token has expired")
        return None
    except jwt.InvalidTokenError:
        print("  Error: Invalid token")
        return None

# Test the implementation
if __name__ == "__main__":
    print("=== JWT Authentication Test ===")
    print()
    
    # Test 1: Create and decode valid token
    user_data = {"sub": "alice@example.com", "role": "admin"}
    token = create_access_token(user_data, timedelta(minutes=30))
    print(f"1. Token created: {token[:40]}...")
    
    # Test 2: Get user from valid token
    user = get_current_user(token)
    print(f"2. User from token: {user}")
    print()
    
    # Test 3: Handle expired token
    print("3. Testing expired token:")
    expired_token = create_access_token(user_data, timedelta(seconds=-1))
    expired_user = get_current_user(expired_token)
    print(f"   Result: {expired_user}")
    print()
    
    # Test 4: Handle invalid token
    print("4. Testing invalid token:")
    invalid_user = get_current_user("invalid.token.here")
    print(f"   Result: {invalid_user}")
    print()
    
    print("=== All tests completed ===")