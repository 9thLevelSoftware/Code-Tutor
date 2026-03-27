---
type: "EXAMPLE"
title: "Complete OAuth2 Setup"
---

**Setting up OAuth2 with FastAPI:**

This example shows a complete OAuth2 password flow implementation with FastAPI. The `OAuth2PasswordBearer` creates a security scheme that expects a token from the `/token` endpoint.

```python
from fastapi import FastAPI, Depends, HTTPException, status
from fastapi.security import OAuth2PasswordBearer, OAuth2PasswordRequestForm

app = FastAPI()

oauth2_scheme = OAuth2PasswordBearer(tokenUrl="token")

@app.post("/token")
async def login(form_data: OAuth2PasswordRequestForm = Depends()):
    user = await authenticate_user(form_data.username, form_data.password)
    if not user:
        raise HTTPException(
            status_code=status.HTTP_401_UNAUTHORIZED,
            detail="Incorrect username or password",
            headers={"WWW-Authenticate": "Bearer"},
        )
    access_token = create_access_token(data={"sub": user.email})
    return {"access_token": access_token, "token_type": "bearer"}
```

**Key Components:**
- `OAuth2PasswordBearer(tokenUrl="token")` - Declares the token endpoint
- `OAuth2PasswordRequestForm` - Handles `username` and `password` form fields
- Returns token in standard OAuth2 format: `{access_token, token_type}`

```python
from fastapi import FastAPI, Depends, HTTPException, status
from fastapi.security import OAuth2PasswordBearer, OAuth2PasswordRequestForm
from datetime import datetime, timedelta
import jwt

print("=== OAuth2 with FastAPI ===")

# Configuration
SECRET_KEY = os.getenv("SECRET_KEY", "development-fallback-secret")
ALGORITHM = "HS256"
ACCESS_TOKEN_EXPIRE_MINUTES = 30

# Simulated user database
fake_users_db = {
    "alice@example.com": {
        "email": "alice@example.com",
        "hashed_password": "fakehashed_secret123",
        "full_name": "Alice Smith",
        "disabled": False
    }
}

print("\n1. OAuth2PasswordBearer Setup:")
print("   oauth2_scheme = OAuth2PasswordBearer(tokenUrl='token')")
print("   - Declares /token as the endpoint for obtaining tokens")
print("   - Automatically extracts Bearer token from Authorization header")

def fake_hash_password(password: str) -> str:
    return "fakehashed_" + password

def verify_password(plain_password: str, hashed_password: str) -> bool:
    return fake_hash_password(plain_password) == hashed_password

def authenticate_user(email: str, password: str):
    user = fake_users_db.get(email)
    if not user:
        return None
    if not verify_password(password, user["hashed_password"]):
        return None
    return user

def create_access_token(data: dict, expires_delta: timedelta = None) -> str:
    to_encode = data.copy()
    expire = datetime.utcnow() + (expires_delta or timedelta(minutes=15))
    to_encode.update({"exp": expire})
    return jwt.encode(to_encode, SECRET_KEY, algorithm=ALGORITHM)

print("\n2. Token Endpoint Implementation:")
print("   @app.post('/token')")
print("   async def login(form_data: OAuth2PasswordRequestForm = Depends()):")
print("       # Validate credentials")
print("       # Return {access_token, token_type}")

# Simulate the login flow
print("\n3. Simulating Login Flow:")

# Step 1: User sends credentials
print("\n   Step 1: User sends credentials")
print("   POST /token")
print("   Content-Type: application/x-www-form-urlencoded")
print("   Body: username=alice@example.com&password=secret123")

# Step 2: Server authenticates
user = authenticate_user("alice@example.com", "secret123")
if user:
    print(f"\n   Step 2: Server authenticates user: {user['full_name']}")
    
    # Step 3: Server creates token
    access_token = create_access_token(
        data={"sub": user["email"]},
        expires_delta=timedelta(minutes=ACCESS_TOKEN_EXPIRE_MINUTES)
    )
    print(f"\n   Step 3: Server creates access token")
    print(f"   Token: {access_token[:50]}...")
    
    # Step 4: Return OAuth2 response
    response = {"access_token": access_token, "token_type": "bearer"}
    print(f"\n   Step 4: Server returns OAuth2 response:")
    print(f"   {response}")

print("\n4. Using the Token:")
print("   GET /protected-route")
print("   Authorization: Bearer <access_token>")

print("\n5. Protected Route Example:")
print('''
   @app.get("/users/me")
   async def read_users_me(token: str = Depends(oauth2_scheme)):
       user = await get_current_user(token)
       return user
''')

print("\n=== Complete OAuth2 Flow ===")
print("""
┌─────────┐     ┌─────────┐     ┌─────────┐
│ Client  │────>│ /token  │────>│ Server  │
│         │     │ (POST)  │     │         │
└─────────┘     └─────────┘     └─────────┘
     │               │               │
     │  username +   │               │
     │  password     │               │
     │──────────────>│               │
     │               │  validate     │
     │               │──────────────>│
     │               │               │
     │               │  access_token │
     │<──────────────│<──────────────│
     │               │               │
     │  Use token    │               │
     │  in headers   │               │
     │──────────────────────────────>│
     │               │               │
""")
```
