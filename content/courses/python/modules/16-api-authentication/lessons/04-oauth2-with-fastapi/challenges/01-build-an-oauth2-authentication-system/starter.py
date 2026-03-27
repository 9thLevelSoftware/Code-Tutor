import os
import jwt
from datetime import datetime, timedelta
from typing import Optional, List

SECRET_KEY = os.getenv("SECRET_KEY", "development-fallback-secret")
ALGORITHM = "HS256"
ACCESS_TOKEN_EXPIRE_MINUTES = 30
REFRESH_TOKEN_EXPIRE_DAYS = 7

# TODO: Implement create_access_token with scopes
def create_access_token(data: dict, scopes: List[str] = None) -> str:
    """Create an access token with optional scopes."""
    pass

# TODO: Implement create_refresh_token
def create_refresh_token(data: dict) -> str:
    """Create a refresh token."""
    pass

# TODO: Implement verify_token_scopes
def verify_token_scopes(token: str, required_scopes: List[str]) -> dict:
    """Verify token and check for required scopes.
    
    Returns dict with 'valid', 'user', 'scopes' on success.
    Returns dict with 'valid': False, 'error' on failure.
    """
    pass

# TODO: Implement refresh_access_token
def refresh_access_token(refresh_token: str, scopes: List[str] = None) -> dict:
    """Exchange refresh token for new access token.
    
    Returns dict with 'access_token', 'token_type' on success.
    Returns dict with 'error' on failure.
    """
    pass

# Test your implementation
if __name__ == "__main__":
    print("=== OAuth2 Authentication Test ===")
    
    # Test 1: Create tokens for regular user
    print("\n1. Creating tokens for regular user...")
    user_data = {"sub": "user@example.com"}
    access_token = create_access_token(user_data, scopes=["read"])
    refresh_token = create_refresh_token(user_data)
    print(f"   Access Token: {access_token[:40]}...")
    print(f"   Refresh Token: {refresh_token[:40]}...")
    
    # Test 2: Verify read scope (should succeed)
    print("\n2. Verifying 'read' scope...")
    result = verify_token_scopes(access_token, ["read"])
    print(f"   Result: {result}")
    
    # Test 3: Verify admin scope (should fail)
    print("\n3. Verifying 'admin' scope...")
    result = verify_token_scopes(access_token, ["admin"])
    print(f"   Result: {result}")
    
    # Test 4: Create admin token and verify
    print("\n4. Creating admin token...")
    admin_data = {"sub": "admin@example.com"}
    admin_token = create_access_token(admin_data, scopes=["read", "write", "admin"])
    result = verify_token_scopes(admin_token, ["admin"])
    print(f"   Admin scope check: {result}")
    
    # Test 5: Refresh token
    print("\n5. Refreshing access token...")
    new_tokens = refresh_access_token(refresh_token, scopes=["read"])
    print(f"   New tokens: {new_tokens}")