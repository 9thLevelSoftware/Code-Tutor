---
type: "EXAMPLE"
title: "Token-Based Login"
---

**Login Endpoint with JWT**

This example shows how to implement a login endpoint that returns JWT tokens.

**Flow:**
1. User submits email/password via `OAuth2PasswordRequestForm`
2. Server verifies credentials against database
3. If valid, create and return access token
4. Client stores token for subsequent requests

**OAuth2PasswordRequestForm:**
- FastAPI's built-in form for username/password
- Follows OAuth2 spec (uses "username" field)
- Automatically validates form data

```python
from fastapi import FastAPI, HTTPException, Depends
from fastapi.security import OAuth2PasswordRequestForm
from sqlalchemy.ext.asyncio import AsyncSession
import jwt
from datetime import datetime, timedelta
from passlib.context import CryptContext

app = FastAPI()

# Password hashing
pwd_context = CryptContext(schemes=["bcrypt"], deprecated="auto")

# JWT settings
SECRET_KEY = os.getenv("SECRET_KEY", "development-fallback-secret")
ALGORITHM = "HS256"

def create_access_token(data: dict) -> str:
    """Create JWT token with 30 minute expiration."""
    to_encode = data.copy()
    expire = datetime.utcnow() + timedelta(minutes=30)
    to_encode.update({"exp": expire})
    return jwt.encode(to_encode, SECRET_KEY, algorithm=ALGORITHM)

async def authenticate_user(db: AsyncSession, email: str, password: str):
    """Verify user credentials.
    
    Returns user if valid, None otherwise.
    """
    user = await get_user_by_email(db, email)
    if not user:
        return None
    if not pwd_context.verify(password, user.hashed_password):
        return None
    return user

@app.post("/token")
async def login(
    form_data: OAuth2PasswordRequestForm = Depends(),
    db: AsyncSession = Depends(get_db)
):
    """Login endpoint - returns JWT access token.
    
    Expects form data with 'username' (email) and 'password' fields.
    Returns access_token and token_type for OAuth2 compatibility.
    """
    # Authenticate user
    user = await authenticate_user(db, form_data.username, form_data.password)
    if not user:
        raise HTTPException(
            status_code=401,
            detail="Invalid credentials",
            headers={"WWW-Authenticate": "Bearer"}
        )
    
    # Create access token with user email as subject
    access_token = create_access_token(data={"sub": user.email})
    
    return {
        "access_token": access_token,
        "token_type": "bearer"
    }

# Demonstration of the flow
print("=== Token-Based Login Flow ===")
print()
print("1. Client sends POST /token with form data:")
print("   { username: 'alice@example.com', password: 'secret123' }")
print()
print("2. Server authenticates:")
print("   - Looks up user by email")
print("   - Verifies password hash")
print()
print("3. If valid, server responds:")
example_token = jwt.encode(
    {"sub": "alice@example.com", "exp": datetime.utcnow() + timedelta(minutes=30)},
    SECRET_KEY,
    algorithm=ALGORITHM
)
print(f"   {{ access_token: '{example_token[:40]}...', token_type: 'bearer' }}")
print()
print("4. Client stores token and includes in subsequent requests:")
print("   Authorization: Bearer <token>")
```
