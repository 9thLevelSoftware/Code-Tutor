---
type: "EXAMPLE"
title: "Protected Routes"
---

**Protecting Endpoints with JWT**

This example shows how to create protected routes that require valid JWT tokens.

**Key Components:**
- `OAuth2PasswordBearer`: Extracts token from Authorization header
- `get_current_user`: Dependency that validates token and returns user
- Protected endpoints use `Depends(get_current_user)`

**Error Handling:**
- `ExpiredSignatureError`: Token has expired
- `InvalidTokenError`: Token is malformed or signature invalid
- Always return 401 for auth failures

```python
from fastapi import FastAPI, Depends, HTTPException
from fastapi.security import OAuth2PasswordBearer
from sqlalchemy.ext.asyncio import AsyncSession
import jwt
from datetime import datetime, timedelta

app = FastAPI()

# JWT settings
SECRET_KEY = os.getenv("SECRET_KEY", "development-fallback-secret")
ALGORITHM = "HS256"

# OAuth2 scheme - tells FastAPI where to find the token
# tokenUrl points to our login endpoint
oauth2_scheme = OAuth2PasswordBearer(tokenUrl="token")

def decode_token(token: str) -> dict:
    """Decode and verify JWT token."""
    return jwt.decode(token, SECRET_KEY, algorithms=[ALGORITHM])

async def get_current_user(
    token: str = Depends(oauth2_scheme),
    db: AsyncSession = Depends(get_db)
):
    """Dependency to get current authenticated user.
    
    Extracts and validates JWT from Authorization header,
    then fetches the user from database.
    
    Raises HTTPException 401 if:
    - Token is missing or invalid
    - Token has expired
    - User not found in database
    """
    credentials_exception = HTTPException(
        status_code=401,
        detail="Could not validate credentials",
        headers={"WWW-Authenticate": "Bearer"}
    )
    
    try:
        # Decode and verify token
        payload = decode_token(token)
        email: str = payload.get("sub")
        if email is None:
            raise credentials_exception
    except jwt.ExpiredSignatureError:
        raise HTTPException(
            status_code=401,
            detail="Token expired",
            headers={"WWW-Authenticate": "Bearer"}
        )
    except jwt.InvalidTokenError:
        raise HTTPException(
            status_code=401,
            detail="Invalid token",
            headers={"WWW-Authenticate": "Bearer"}
        )
    
    # Fetch user from database
    user = await get_user_by_email(db, email)
    if user is None:
        raise credentials_exception
    
    return user

# Protected endpoint - requires valid JWT
@app.get("/users/me")
async def read_users_me(current_user = Depends(get_current_user)):
    """Get current user's profile.
    
    This endpoint is protected - requires valid JWT token
    in Authorization header.
    """
    return {
        "id": current_user.id,
        "email": current_user.email,
        "name": current_user.name
    }

@app.get("/users/me/items")
async def read_own_items(current_user = Depends(get_current_user)):
    """Get current user's items.
    
    Another protected endpoint example.
    """
    return {"items": [], "owner": current_user.email}

# Demonstration
print("=== Protected Routes Demo ===")
print()
print("Request to protected endpoint:")
print("  GET /users/me")
print("  Headers: { Authorization: 'Bearer eyJhbG...' }")
print()
print("Server processing:")
print("  1. OAuth2PasswordBearer extracts token from header")
print("  2. get_current_user dependency validates token")
print("  3. If valid, user object passed to endpoint")
print("  4. Endpoint returns user data")
print()
print("Possible responses:")
print("  200: { id: 1, email: 'alice@example.com', name: 'Alice' }")
print("  401: { detail: 'Token expired' }")
print("  401: { detail: 'Invalid token' }")
print("  401: { detail: 'Could not validate credentials' }")
```
