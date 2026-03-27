---
type: "EXAMPLE"
title: "Refresh Tokens"
---

**Refresh tokens allow obtaining new access tokens without re-authenticating:**

Access tokens are short-lived (15-60 minutes). Refresh tokens are long-lived (days/weeks) and can be exchanged for new access tokens.

```python
REFRESH_TOKEN_EXPIRE_DAYS = 7

def create_refresh_token(data: dict) -> str:
    to_encode = data.copy()
    expire = datetime.utcnow() + timedelta(days=REFRESH_TOKEN_EXPIRE_DAYS)
    to_encode.update({"exp": expire, "type": "refresh"})
    return jwt.encode(to_encode, SECRET_KEY, algorithm=ALGORITHM)

@app.post("/token/refresh")
async def refresh_token(refresh_token: str):
    try:
        payload = jwt.decode(refresh_token, SECRET_KEY, algorithms=[ALGORITHM])
        if payload.get("type") != "refresh":
            raise HTTPException(status_code=401, detail="Invalid token type")
        email = payload.get("sub")
        new_access_token = create_access_token(data={"sub": email})
        return {"access_token": new_access_token, "token_type": "bearer"}
    except jwt.ExpiredSignatureError:
        raise HTTPException(status_code=401, detail="Refresh token expired")
```

**Why Use Refresh Tokens?**
- Access tokens expire quickly for security
- Users don't have to log in repeatedly
- Refresh tokens can be revoked server-side

```python
from datetime import datetime, timedelta
import jwt

print("=== Refresh Tokens ===")

SECRET_KEY = os.getenv("SECRET_KEY", "development-fallback-secret")
ALGORITHM = "HS256"
ACCESS_TOKEN_EXPIRE_MINUTES = 30
REFRESH_TOKEN_EXPIRE_DAYS = 7

print("\n1. Token Expiration Strategy:")
print(f"   Access Token: {ACCESS_TOKEN_EXPIRE_MINUTES} minutes (short-lived)")
print(f"   Refresh Token: {REFRESH_TOKEN_EXPIRE_DAYS} days (long-lived)")

def create_access_token(data: dict) -> str:
    to_encode = data.copy()
    expire = datetime.utcnow() + timedelta(minutes=ACCESS_TOKEN_EXPIRE_MINUTES)
    to_encode.update({"exp": expire, "type": "access"})
    return jwt.encode(to_encode, SECRET_KEY, algorithm=ALGORITHM)

def create_refresh_token(data: dict) -> str:
    to_encode = data.copy()
    expire = datetime.utcnow() + timedelta(days=REFRESH_TOKEN_EXPIRE_DAYS)
    to_encode.update({"exp": expire, "type": "refresh"})
    return jwt.encode(to_encode, SECRET_KEY, algorithm=ALGORITHM)

print("\n2. Creating Both Tokens on Login:")

user_email = "alice@example.com"

access_token = create_access_token(data={"sub": user_email})
refresh_token = create_refresh_token(data={"sub": user_email})

print(f"\n   Access Token: {access_token[:50]}...")
print(f"   Refresh Token: {refresh_token[:50]}...")

login_response = {
    "access_token": access_token,
    "refresh_token": refresh_token,
    "token_type": "bearer"
}
print(f"\n   Login Response: {{")
print(f"       access_token: '...',")
print(f"       refresh_token: '...',")
print(f"       token_type: 'bearer'")
print(f"   }}")

print("\n3. Refresh Token Endpoint:")

def refresh_access_token(refresh_token: str) -> dict:
    try:
        payload = jwt.decode(refresh_token, SECRET_KEY, algorithms=[ALGORITHM])
        
        # Verify this is a refresh token
        if payload.get("type") != "refresh":
            return {"error": "Invalid token type"}
        
        # Get user email from token
        email = payload.get("sub")
        
        # Create new access token
        new_access_token = create_access_token(data={"sub": email})
        
        return {
            "access_token": new_access_token,
            "token_type": "bearer"
        }
        
    except jwt.ExpiredSignatureError:
        return {"error": "Refresh token expired - please login again"}
    except jwt.InvalidTokenError:
        return {"error": "Invalid refresh token"}

print("\n   Testing refresh endpoint:")
result = refresh_access_token(refresh_token)
if "access_token" in result:
    print(f"   New Access Token: {result['access_token'][:50]}...")
    print(f"   Token Type: {result['token_type']}")
else:
    print(f"   Error: {result['error']}")

print("\n4. Handling Expired Refresh Token:")
expired_refresh = jwt.encode(
    {"sub": user_email, "exp": datetime.utcnow() - timedelta(days=1), "type": "refresh"},
    SECRET_KEY,
    algorithm=ALGORITHM
)
result = refresh_access_token(expired_refresh)
print(f"   Result: {result}")

print("\n5. FastAPI Refresh Endpoint:")
print('''
   @app.post("/token/refresh")
   async def refresh_token(refresh_token: str):
       try:
           payload = jwt.decode(
               refresh_token, SECRET_KEY, algorithms=[ALGORITHM]
           )
           if payload.get("type") != "refresh":
               raise HTTPException(
                   status_code=401,
                   detail="Invalid token type"
               )
           email = payload.get("sub")
           new_access_token = create_access_token(data={"sub": email})
           return {
               "access_token": new_access_token,
               "token_type": "bearer"
           }
       except jwt.ExpiredSignatureError:
           raise HTTPException(
               status_code=401,
               detail="Refresh token expired"
           )
''')

print("\n=== Token Refresh Flow ===")
print("""
   ┌─────────┐                    ┌─────────┐
   │ Client  │                    │ Server  │
   └────┬────┘                    └────┬────┘
        │                              │
        │ 1. Login (username/password) │
        │─────────────────────────────>│
        │                              │
        │ 2. access_token + refresh    │
        │<─────────────────────────────│
        │                              │
        │ 3. API request (access_token)│
        │─────────────────────────────>│
        │                              │
        │     ...time passes...        │
        │                              │
        │ 4. Access token expired!     │
        │<─────────────────────────────│
        │                              │
        │ 5. POST /token/refresh       │
        │    (with refresh_token)      │
        │─────────────────────────────>│
        │                              │
        │ 6. New access_token          │
        │<─────────────────────────────│
        │                              │
        │ 7. Continue with new token   │
        │─────────────────────────────>│
""")
```
