---
type: "EXAMPLE"
title: "Complete Token Refresh Flow"
---

**Implementing the full refresh token rotation pattern:**

```python
import jwt
import secrets
from datetime import datetime, timedelta, timezone
from typing import Optional, Tuple
from dataclasses import dataclass, field

SECRET_KEY = os.getenv("SECRET_KEY", "development-fallback-secret")
ALGORITHM = "HS256"

@dataclass
class TokenStore:
    """Simulates a database store for refresh tokens.
    In production, use Redis or a database table."""
    tokens: dict = field(default_factory=dict)  # jti -> {user_id, revoked, created_at}
    
    def store_refresh_token(self, jti: str, user_id: int):
        self.tokens[jti] = {
            "user_id": user_id,
            "revoked": False,
            "created_at": datetime.now(timezone.utc)
        }
    
    def is_token_valid(self, jti: str) -> bool:
        token_data = self.tokens.get(jti)
        return token_data is not None and not token_data["revoked"]
    
    def revoke_token(self, jti: str):
        if jti in self.tokens:
            self.tokens[jti]["revoked"] = True
    
    def revoke_all_user_tokens(self, user_id: int):
        """Revoke all refresh tokens for a user (logout everywhere)"""
        for jti, data in self.tokens.items():
            if data["user_id"] == user_id:
                data["revoked"] = True

# Global token store (in production: use Redis)
token_store = TokenStore()

class TokenService:
    def __init__(self, store: TokenStore):
        self.store = store
    
    def create_tokens(self, user_id: int, email: str) -> dict:
        """Create access and refresh tokens, store refresh token."""
        # Create access token (short-lived)
        access_token = jwt.encode(
            {
                "sub": str(user_id),
                "email": email,
                "type": "access",
                "exp": datetime.now(timezone.utc) + timedelta(minutes=15)
            },
            SECRET_KEY,
            algorithm=ALGORITHM
        )
        
        # Create refresh token with unique ID
        jti = secrets.token_hex(16)
        refresh_token = jwt.encode(
            {
                "sub": str(user_id),
                "type": "refresh",
                "jti": jti,
                "exp": datetime.now(timezone.utc) + timedelta(days=7)
            },
            SECRET_KEY,
            algorithm=ALGORITHM
        )
        
        # Store refresh token ID for revocation checks
        self.store.store_refresh_token(jti, user_id)
        
        return {
            "access_token": access_token,
            "refresh_token": refresh_token,
            "expires_in": 900  # 15 minutes in seconds
        }
    
    def refresh_access_token(self, refresh_token: str) -> Optional[dict]:
        """
        Use refresh token to get new access token.
        Implements token rotation: old refresh token is revoked,
        new refresh token is issued.
        """
        try:
            # Verify refresh token
            payload = jwt.decode(
                refresh_token,
                SECRET_KEY,
                algorithms=[ALGORITHM]
            )
            
            # Check token type
            if payload.get("type") != "refresh":
                print("Error: Not a refresh token")
                return None
            
            # Check if token is revoked
            jti = payload.get("jti")
            if not self.store.is_token_valid(jti):
                print("Error: Refresh token has been revoked")
                # Security: Possible token reuse attack!
                # Revoke all tokens for this user
                self.store.revoke_all_user_tokens(int(payload["sub"]))
                return None
            
            # Token is valid - rotate it
            # Revoke old refresh token
            self.store.revoke_token(jti)
            
            # Issue new tokens
            user_id = int(payload["sub"])
            return self.create_tokens(user_id, f"user{user_id}@example.com")
            
        except jwt.ExpiredSignatureError:
            print("Error: Refresh token expired")
            return None
        except jwt.InvalidTokenError:
            print("Error: Invalid refresh token")
            return None
    
    def logout(self, refresh_token: str) -> bool:
        """Revoke the refresh token on logout."""
        try:
            payload = jwt.decode(
                refresh_token,
                SECRET_KEY,
                algorithms=[ALGORITHM]
            )
            jti = payload.get("jti")
            if jti:
                self.store.revoke_token(jti)
                return True
        except jwt.InvalidTokenError:
            pass
        return False
    
    def logout_everywhere(self, user_id: int):
        """Revoke all refresh tokens for a user."""
        self.store.revoke_all_user_tokens(user_id)

# Demonstration
print("Finance Tracker - Token Refresh Flow")
print("=" * 50)

service = TokenService(token_store)

# User logs in
print("\n1. User Login:")
tokens = service.create_tokens(user_id=42, email="alice@example.com")
print(f"   Access token issued (expires in {tokens['expires_in']}s)")
print(f"   Refresh token issued")

# Simulate access token expiration - use refresh token
print("\n2. Access Token Expired - Refreshing:")
new_tokens = service.refresh_access_token(tokens["refresh_token"])
if new_tokens:
    print(f"   New access token issued!")
    print(f"   Old refresh token revoked, new one issued")

# Try to reuse old refresh token (attack simulation)
print("\n3. Attacker Tries to Reuse Old Refresh Token:")
attack_result = service.refresh_access_token(tokens["refresh_token"])
if attack_result is None:
    print("   Attack blocked! Old token was already revoked.")

# User logs out
print("\n4. User Logout:")
if new_tokens:
    service.logout(new_tokens["refresh_token"])
    print("   Refresh token revoked")

# Verify logout worked
print("\n5. Try to Use Token After Logout:")
if new_tokens:
    post_logout = service.refresh_access_token(new_tokens["refresh_token"])
    if post_logout is None:
        print("   Correctly rejected - user is logged out")
```
