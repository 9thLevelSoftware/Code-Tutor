---
type: "EXAMPLE"
title: "Code Example: FastAPI OAuth2 with JWT"
---

**JWT Authentication Flow with FastAPI:**

**1. Login (OAuth2 Password Flow):**
```
User → POST /token (form: username, password)
API → Verify credentials
API → Create JWT token
API → Return access_token
```

**2. Authenticated Request:**
```
User → GET /api/protected
Header: Authorization: Bearer <token>
API → OAuth2PasswordBearer extracts token
API → Verify signature and expiration
API → Process request
```

**3. Token Structure:**
```json
{
  "sub": "alice@example.com",
  "role": "admin",
  "exp": 1234567890
}
```

**FastAPI Security Benefits:**
- Built-in OAuth2 support
- Automatic Swagger UI auth button
- Dependency injection for auth
- Type-safe with Pydantic
- Auto-generated API docs

**Best Practices:**
- Use OAuth2PasswordBearer for token extraction
- python-jose for JWT encoding/decoding
- passlib for password hashing

```python
from fastapi import FastAPI, Depends, HTTPException, status
from fastapi.security import OAuth2PasswordBearer, OAuth2PasswordRequestForm
from pydantic import BaseModel, EmailStr
from jose import JWTError, jwt
from datetime import datetime, timedelta, timezone
import hashlib
import os
from typing import Optional

app = FastAPI()

# Configuration
SECRET_KEY = os.getenv("SECRET_KEY", "development-fallback-secret")
ALGORITHM = 'HS256'
ACCESS_TOKEN_EXPIRE_MINUTES = 30

# OAuth2 scheme - tells FastAPI where to find the token
oauth2_scheme = OAuth2PasswordBearer(tokenUrl='token')

# Simulated user database
USERS = {
    'alice@example.com': {
        'id': 1,
        'name': 'Alice',
        'email': 'alice@example.com',
        'password_hash': None,
        'role': 'admin'
    },
    'bob@example.com': {
        'id': 2,
        'name': 'Bob',
        'email': 'bob@example.com',
        'password_hash': None,
        'role': 'user'
    }
}

print("=== FastAPI OAuth2 + JWT Authentication ===")

# Pydantic models
class Token(BaseModel):
    access_token: str
    token_type: str

class TokenData(BaseModel):
    email: Optional[str] = None
    role: Optional[str] = None

class User(BaseModel):
    id: int
    name: str
    email: EmailStr
    role: str

def hash_password(password: str) -> bytes:
    """Hash password with salt"""
    salt = b'demo-salt-change-in-production'
    return hashlib.pbkdf2_hmac('sha256', password.encode(), salt, 100000)

def verify_password(stored_hash: bytes, password: str) -> bool:
    """Verify password against hash"""
    return stored_hash == hash_password(password)

# Set passwords
USERS['alice@example.com']['password_hash'] = hash_password('password123')
USERS['bob@example.com']['password_hash'] = hash_password('password456')

def create_access_token(data: dict, expires_delta: Optional[timedelta] = None):
    """Create JWT access token"""
    to_encode = data.copy()
    expire = datetime.now(timezone.utc) + (expires_delta or timedelta(minutes=15))
    to_encode.update({'exp': expire})
    return jwt.encode(to_encode, SECRET_KEY, algorithm=ALGORITHM)

async def get_current_user(token: str = Depends(oauth2_scheme)) -> User:
    """Dependency to get current user from token"""
    credentials_exception = HTTPException(
        status_code=status.HTTP_401_UNAUTHORIZED,
        detail='Could not validate credentials',
        headers={'WWW-Authenticate': 'Bearer'},
    )
    try:
        payload = jwt.decode(token, SECRET_KEY, algorithms=[ALGORITHM])
        email: str = payload.get('sub')
        if email is None:
            raise credentials_exception
    except JWTError:
        raise credentials_exception
    
    user = USERS.get(email)
    if user is None:
        raise credentials_exception
    
    return User(
        id=user['id'],
        name=user['name'],
        email=user['email'],
        role=user['role']
    )

def require_role(required_role: str):
    """Factory for role-checking dependency"""
    async def role_checker(current_user: User = Depends(get_current_user)):
        if current_user.role != required_role:
            raise HTTPException(
                status_code=status.HTTP_403_FORBIDDEN,
                detail=f'{required_role} role required'
            )
        return current_user
    return role_checker

@app.post('/token', response_model=Token)
async def login(form_data: OAuth2PasswordRequestForm = Depends()):
    """OAuth2 login endpoint - returns JWT token"""
    user = USERS.get(form_data.username)
    if not user:
        raise HTTPException(
            status_code=status.HTTP_401_UNAUTHORIZED,
            detail='Incorrect email or password',
            headers={'WWW-Authenticate': 'Bearer'},
        )
    
    if not verify_password(user['password_hash'], form_data.password):
        raise HTTPException(
            status_code=status.HTTP_401_UNAUTHORIZED,
            detail='Incorrect email or password',
            headers={'WWW-Authenticate': 'Bearer'},
        )
    
    access_token = create_access_token(
        data={'sub': user['email'], 'role': user['role']},
        expires_delta=timedelta(minutes=ACCESS_TOKEN_EXPIRE_MINUTES)
    )
    
    return {'access_token': access_token, 'token_type': 'bearer'}

@app.get('/api/me', response_model=User)
async def get_me(current_user: User = Depends(get_current_user)):
    """Get current user info from token"""
    return current_user

@app.get('/api/admin/users')
async def admin_get_users(current_user: User = Depends(require_role('admin'))):
    """Admin only endpoint"""
    return {
        'users': [
            {'id': u['id'], 'name': u['name'], 'role': u['role']}
            for u in USERS.values()
        ]
    }

@app.get('/api/protected')
async def protected_route(current_user: User = Depends(get_current_user)):
    """Any authenticated user can access"""
    return {
        'message': f'Hello {current_user.name}!',
        'your_role': current_user.role
    }

if __name__ == '__main__':
    print("\n=== FastAPI OAuth2 + JWT API ===")
    print("\nEndpoints:")
    print("  POST /token          - Login (OAuth2 form)")
    print("  GET  /api/me         - Get current user")
    print("  GET  /api/protected  - Protected route")
    print("  GET  /api/admin/users - Admin only")
    print("\nRun with: uvicorn main:app --reload")
    print("\nSwagger UI: http://localhost:8000/docs")
    print("  Click 'Authorize' button to login!")
    print("\nTest credentials:")
    print("  alice@example.com / password123 (admin)")
    print("  bob@example.com / password456 (user)")
```
