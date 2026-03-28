---
type: "EXAMPLE"
title: "Hybrid Auth with FastAPI"
---

**Complete implementation showing session-based web auth and token-based API auth:**

```python
from fastapi import FastAPI, Request, Response, Depends, HTTPException
from fastapi.security import HTTPBearer, HTTPAuthorizationCredentials
import secrets
import jwt
from datetime import datetime, timedelta, timezone
from typing import Optional, Dict, Any
import redis

app = FastAPI()
security = HTTPBearer()

# Redis for session storage
redis_client = redis.from_url("redis://localhost:6379")
JWT_SECRET = secrets.token_hex(32)  # In production, load from env

class HybridAuthManager:
    """Handles both session and JWT authentication."""
    
    def __init__(self):
        self.session_ttl = timedelta(hours=24)
        self.jwt_ttl = timedelta(minutes=15)
        self.refresh_ttl = timedelta(days=7)
    
    # ========== Session Methods (for Web) ==========
    
    def create_session(self, response: Response, user_id: int, 
                       user_data: Dict[str, Any]) -> str:
        """Create server-side session and set httpOnly cookie."""
        session_id = secrets.token_urlsafe(32)
        session_data = {
            "user_id": user_id,
            "user_data": user_data,
            "created_at": datetime.now(timezone.utc).isoformat()
        }
        
        redis_client.setex(
            f"session:{session_id}",
            int(self.session_ttl.total_seconds()),
            str(session_data)
        )
        
        # Set secure cookie
        response.set_cookie(
            key="session_id",
            value=session_id,
            httponly=True,
            secure=True,
            samesite="Strict",
            max_age=int(self.session_ttl.total_seconds())
        )
        return session_id
    
    def validate_session(self, request: Request) -> Optional[Dict]:
        """Validate session from cookie."""
        session_id = request.cookies.get("session_id")
        if not session_id:
            return None
        
        data = redis_client.get(f"session:{session_id}")
        if not data:
            return None
        
        # Refresh TTL on access
        redis_client.expire(f"session:{session_id}", 
                           int(self.session_ttl.total_seconds()))
        return eval(data)  # In production, use JSON
    
    # ========== JWT Methods (for API/Mobile) ==========
    
    def create_tokens(self, user_id: int, user_data: Dict[str, Any]) -> Dict:
        """Create access and refresh tokens."""
        now = datetime.now(timezone.utc)
        
        # Access token (short-lived)
        access_payload = {
            "sub": user_id,
            "email": user_data.get("email"),
            "type": "access",
            "iat": now,
            "exp": now + self.jwt_ttl
        }
        access_token = jwt.encode(access_payload, JWT_SECRET, algorithm="HS256")
        
        # Refresh token (long-lived, stored server-side)
        refresh_token = secrets.token_urlsafe(32)
        redis_client.setex(
            f"refresh:{refresh_token}",
            int(self.refresh_ttl.total_seconds()),
            str({"user_id": user_id, "user_data": user_data})
        )
        
        return {
            "access_token": access_token,
            "refresh_token": refresh_token,
            "token_type": "Bearer",
            "expires_in": int(self.jwt_ttl.total_seconds())
        }
    
    def validate_jwt(self, token: str) -> Optional[Dict]:
        """Validate JWT access token."""
        try:
            payload = jwt.decode(token, JWT_SECRET, algorithms=["HS256"])
            if payload.get("type") != "access":
                return None
            return payload
        except jwt.ExpiredSignatureError:
            return None
        except jwt.InvalidTokenError:
            return None
    
    def refresh_access_token(self, refresh_token: str) -> Optional[Dict]:
        """Create new access token using refresh token."""
        data = redis_client.get(f"refresh:{refresh_token}")
        if not data:
            return None
        
        stored = eval(data)  # In production, use JSON
        
        # Rotate refresh token (security best practice)
        redis_client.delete(f"refresh:{refresh_token}")
        return self.create_tokens(stored["user_id"], stored["user_data"])

auth_manager = HybridAuthManager()

# ========== Web Routes (Session-based) ==========

@app.post("/web/login")
def web_login(response: Response, email: str, password: str):
    """Traditional web login with session cookie."""
    # Verify credentials (simplified)
    if email == "alice@example.com" and password == "correct":
        auth_manager.create_session(
            response, 
            user_id=1,
            user_data={"email": email, "role": "user"}
        )
        return {"message": "Logged in successfully"}
    raise HTTPException(401, "Invalid credentials")

@app.get("/web/dashboard")
def web_dashboard(request: Request):
    """Session-protected web route."""
    session = auth_manager.validate_session(request)
    if not session:
        raise HTTPException(401, "Not authenticated")
    return {"message": f"Welcome {session['user_data']['email']}"}

@app.post("/web/logout")
def web_logout(response: Response, request: Request):
    """Clear session cookie and invalidate server-side."""
    session_id = request.cookies.get("session_id")
    if session_id:
        redis_client.delete(f"session:{session_id}")
    response.delete_cookie("session_id")
    return {"message": "Logged out"}

# ========== API Routes (JWT-based) ==========

@app.post("/api/login")
def api_login(email: str, password: str):
    """API login returning JWT tokens."""
    if email == "alice@example.com" and password == "correct":
        return auth_manager.create_tokens(
            user_id=1,
            user_data={"email": email, "role": "user"}
        )
    raise HTTPException(401, "Invalid credentials")

@app.get("/api/data")
def api_data(credentials: HTTPAuthorizationCredentials = Depends(security)):
    """JWT-protected API endpoint."""
    payload = auth_manager.validate_jwt(credentials.credentials)
    if not payload:
        raise HTTPException(401, "Invalid or expired token")
    return {"data": "sensitive info", "user": payload["email"]}

@app.post("/api/refresh")
def api_refresh(refresh_token: str):
    """Refresh access token."""
    tokens = auth_manager.refresh_access_token(refresh_token)
    if not tokens:
        raise HTTPException(401, "Invalid refresh token")
    return tokens

# Example usage comments:
# Web: POST /web/login → Sets httpOnly cookie → GET /web/dashboard (cookie auto-sent)
# API: POST /api/login → Returns tokens → GET /api/data (header: Bearer <token>)
```
