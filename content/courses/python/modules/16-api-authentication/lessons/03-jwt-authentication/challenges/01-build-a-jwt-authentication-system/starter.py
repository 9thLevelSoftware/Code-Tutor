import os
import jwt
from datetime import datetime, timedelta
from typing import Optional

SECRET_KEY = os.getenv("SECRET_KEY", "development-fallback-secret")
ALGORITHM = "HS256"

# TODO: Implement create_access_token
def create_access_token(data: dict, expires_delta: Optional[timedelta] = None) -> str:
    """Create a JWT access token with expiration."""
    pass

# TODO: Implement decode_token
def decode_token(token: str) -> dict:
    """Decode and verify a JWT token."""
    pass

# TODO: Implement get_current_user
def get_current_user(token: str) -> Optional[dict]:
    """Get user from token.
    
    Returns user dict on success, None on failure.
    Should handle ExpiredSignatureError and InvalidTokenError.
    """
    pass

# Test your implementation
if __name__ == "__main__":
    print("=== JWT Authentication Test ===")
    
    # Test 1: Create and decode valid token
    user_data = {"sub": "alice@example.com", "role": "admin"}
    token = create_access_token(user_data, timedelta(minutes=30))
    print(f"1. Token created: {token[:40]}...")
    
    # Test 2: Get user from valid token
    user = get_current_user(token)
    print(f"2. User from token: {user}")
    
    # Test 3: Handle expired token
    expired_token = create_access_token(user_data, timedelta(seconds=-1))
    expired_user = get_current_user(expired_token)
    print(f"3. Expired token result: {expired_user}")
    
    # Test 4: Handle invalid token
    invalid_user = get_current_user("invalid.token.here")
    print(f"4. Invalid token result: {invalid_user}")