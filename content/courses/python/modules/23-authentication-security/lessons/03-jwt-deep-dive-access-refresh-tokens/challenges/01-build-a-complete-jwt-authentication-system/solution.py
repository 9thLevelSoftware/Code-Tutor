import os
import jwt
import secrets
from datetime import datetime, timedelta, timezone
from typing import Optional, Dict, Any
from dataclasses import dataclass, field

SECRET_KEY = os.getenv("SECRET_KEY", "development-fallback-secret")
ALGORITHM = "HS256"

@dataclass
class RefreshTokenStore:
    """Simple in-memory store for refresh token tracking"""
    tokens: Dict[str, Dict] = field(default_factory=dict)
    
    def save(self, jti: str, user_id: int):
        self.tokens[jti] = {"user_id": user_id, "revoked": False}
    
    def is_valid(self, jti: str) -> bool:
        return jti in self.tokens and not self.tokens[jti]["revoked"]
    
    def revoke(self, jti: str):
        if jti in self.tokens:
            self.tokens[jti]["revoked"] = True

class JWTAuthService:
    def __init__(self):
        self.store = RefreshTokenStore()
    
    def create_access_token(self, user_id: int, email: str, role: str = "user") -> str:
        """Create a 15-minute access token with user claims."""
        payload = {
            "sub": str(user_id),
            "email": email,
            "role": role,
            "type": "access",
            "exp": datetime.now(timezone.utc) + timedelta(minutes=15)
        }
        return jwt.encode(payload, SECRET_KEY, algorithm=ALGORITHM)
    
    def create_refresh_token(self, user_id: int) -> str:
        """Create a 7-day refresh token with unique jti."""
        jti = secrets.token_hex(16)
        payload = {
            "sub": str(user_id),
            "type": "refresh",
            "jti": jti,
            "exp": datetime.now(timezone.utc) + timedelta(days=7)
        }
        self.store.save(jti, user_id)
        return jwt.encode(payload, SECRET_KEY, algorithm=ALGORITHM)
    
    def login(self, user_id: int, email: str, role: str = "user") -> Dict[str, Any]:
        """Authenticate user and return both tokens."""
        return {
            "access_token": self.create_access_token(user_id, email, role),
            "refresh_token": self.create_refresh_token(user_id),
            "token_type": "Bearer",
            "expires_in": 900
        }
    
    def verify_access_token(self, token: str) -> Optional[Dict[str, Any]]:
        """Verify access token and return user info or None."""
        try:
            payload = jwt.decode(token, SECRET_KEY, algorithms=[ALGORITHM])
            if payload.get("type") != "access":
                return None
            return {
                "user_id": int(payload["sub"]),
                "email": payload["email"],
                "role": payload["role"]
            }
        except jwt.InvalidTokenError:
            return None
    
    def refresh_tokens(self, refresh_token: str) -> Optional[Dict[str, Any]]:
        """Use refresh token to get new token pair with rotation."""
        try:
            payload = jwt.decode(refresh_token, SECRET_KEY, algorithms=[ALGORITHM])
            if payload.get("type") != "refresh":
                return None
            
            jti = payload.get("jti")
            if not self.store.is_valid(jti):
                return None
            
            # Revoke old token (rotation)
            self.store.revoke(jti)
            
            # Issue new tokens
            user_id = int(payload["sub"])
            return self.login(user_id, f"user{user_id}@financetracker.com")
            
        except jwt.InvalidTokenError:
            return None
    
    def logout(self, refresh_token: str) -> bool:
        """Revoke refresh token on logout."""
        try:
            payload = jwt.decode(refresh_token, SECRET_KEY, algorithms=[ALGORITHM])
            jti = payload.get("jti")
            if jti:
                self.store.revoke(jti)
                return True
        except jwt.InvalidTokenError:
            pass
        return False

# Test the authentication service
auth = JWTAuthService()

print("JWT Authentication Service Test")
print("=" * 50)

# Test login
print("\n1. Login:")
tokens = auth.login(user_id=1, email="alice@financetracker.com", role="user")
print(f"   Access token: {tokens['access_token'][:40]}...")
print(f"   Refresh token: {tokens['refresh_token'][:40]}...")

# Test access token verification
print("\n2. Verify Access Token:")
user_info = auth.verify_access_token(tokens["access_token"])
print(f"   User info: {user_info}")

# Test token refresh
print("\n3. Refresh Tokens:")
new_tokens = auth.refresh_tokens(tokens["refresh_token"])
if new_tokens:
    print(f"   New access token: {new_tokens['access_token'][:40]}...")
    print("   Old refresh token revoked, new one issued")

# Test old refresh token is revoked
print("\n4. Try Old Refresh Token:")
old_refresh_result = auth.refresh_tokens(tokens["refresh_token"])
print(f"   Result: {'Blocked (correct!)' if old_refresh_result is None else 'Allowed (bug!)'}")

# Test logout
print("\n5. Logout:")
if new_tokens:
    logout_success = auth.logout(new_tokens["refresh_token"])
    print(f"   Logout successful: {logout_success}")

# Verify logout worked
print("\n6. Try Token After Logout:")
if new_tokens:
    post_logout = auth.refresh_tokens(new_tokens["refresh_token"])
    print(f"   Result: {'Blocked (correct!)' if post_logout is None else 'Allowed (bug!)'}")