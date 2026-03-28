import os
import jwt
from datetime import datetime, timedelta, timezone
from typing import Optional, List

SECRET_KEY = os.getenv("SECRET_KEY", "development-fallback-secret")
ALGORITHM = "HS256"
ACCESS_TOKEN_EXPIRE_MINUTES = 30
REFRESH_TOKEN_EXPIRE_DAYS = 7

def create_access_token(data: dict, scopes: List[str] = None) -> str:
    """Create an access token with optional scopes."""
    to_encode = data.copy()
    expire = datetime.now(timezone.utc) + timedelta(minutes=ACCESS_TOKEN_EXPIRE_MINUTES)
    to_encode.update({
        "exp": expire,
        "type": "access",
        "scopes": scopes or []
    })
    return jwt.encode(to_encode, SECRET_KEY, algorithm=ALGORITHM)

def create_refresh_token(data: dict) -> str:
    """Create a refresh token."""
    to_encode = data.copy()
    expire = datetime.now(timezone.utc) + timedelta(days=REFRESH_TOKEN_EXPIRE_DAYS)
    to_encode.update({
        "exp": expire,
        "type": "refresh"
    })
    return jwt.encode(to_encode, SECRET_KEY, algorithm=ALGORITHM)

def verify_token_scopes(token: str, required_scopes: List[str]) -> dict:
    """Verify token and check for required scopes."""
    try:
        payload = jwt.decode(token, SECRET_KEY, algorithms=[ALGORITHM])
        token_scopes = payload.get("scopes", [])
        
        # Check all required scopes are present
        for scope in required_scopes:
            if scope not in token_scopes:
                return {
                    "valid": False,
                    "error": f"Missing required scope: {scope}"
                }
        
        return {
            "valid": True,
            "user": payload.get("sub"),
            "scopes": token_scopes
        }
    except jwt.ExpiredSignatureError:
        return {"valid": False, "error": "Token expired"}
    except jwt.InvalidTokenError:
        return {"valid": False, "error": "Invalid token"}

def refresh_access_token(refresh_token: str, scopes: List[str] = None) -> dict:
    """Exchange refresh token for new access token."""
    try:
        payload = jwt.decode(refresh_token, SECRET_KEY, algorithms=[ALGORITHM])
        
        # Verify this is a refresh token
        if payload.get("type") != "refresh":
            return {"error": "Invalid token type - expected refresh token"}
        
        # Get user from token
        user_email = payload.get("sub")
        if not user_email:
            return {"error": "Invalid token - no user"}
        
        # Create new access token
        new_access_token = create_access_token(
            data={"sub": user_email},
            scopes=scopes or []
        )
        
        return {
            "access_token": new_access_token,
            "token_type": "bearer"
        }
    except jwt.ExpiredSignatureError:
        return {"error": "Refresh token expired - please login again"}
    except jwt.InvalidTokenError:
        return {"error": "Invalid refresh token"}

# Test the implementation
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
    
    # Verify new access token works
    if "access_token" in new_tokens:
        result = verify_token_scopes(new_tokens["access_token"], ["read"])
        print(f"   New token verification: {result}")
    
    print("\n=== All tests completed ===")