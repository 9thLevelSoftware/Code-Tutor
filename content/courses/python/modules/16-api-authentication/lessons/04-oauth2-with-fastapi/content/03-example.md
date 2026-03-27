---
type: "EXAMPLE"
title: "Using Scopes"
---

**Scopes provide fine-grained permission control:**

Scopes define what actions a token is authorized to perform. This allows you to create tokens with limited permissions.

```python
from fastapi.security import SecurityScopes

oauth2_scheme = OAuth2PasswordBearer(
    tokenUrl="token",
    scopes={"read": "Read access", "write": "Write access", "admin": "Admin access"}
)

async def get_current_user(
    security_scopes: SecurityScopes,
    token: str = Depends(oauth2_scheme)
) -> User:
    # Verify token and check scopes
    ...

@app.get("/admin/users")
async def list_all_users(
    current_user: User = Security(get_current_user, scopes=["admin"])
):
    return await get_all_users()
```

**Common Scope Patterns:**
- `read` - View data only
- `write` - Create and modify data
- `delete` - Remove data
- `admin` - Full administrative access
- `users:read`, `users:write` - Resource-specific scopes

```python
from datetime import datetime, timedelta
import jwt

print("=== OAuth2 Scopes in FastAPI ===")

SECRET_KEY = os.getenv("SECRET_KEY", "development-fallback-secret")
ALGORITHM = "HS256"

print("\n1. Defining Scopes:")
print('''
   oauth2_scheme = OAuth2PasswordBearer(
       tokenUrl="token",
       scopes={
           "read": "Read access",
           "write": "Write access",
           "admin": "Admin access"
       }
   )
''')

print("\n2. Creating Tokens with Scopes:")

def create_access_token_with_scopes(data: dict, scopes: list) -> str:
    to_encode = data.copy()
    to_encode.update({
        "exp": datetime.utcnow() + timedelta(minutes=30),
        "scopes": scopes
    })
    return jwt.encode(to_encode, SECRET_KEY, algorithm=ALGORITHM)

# Regular user token
user_token = create_access_token_with_scopes(
    data={"sub": "user@example.com"},
    scopes=["read"]
)
print(f"\n   Regular user token (read only):")
print(f"   {user_token[:60]}...")

# Admin token
admin_token = create_access_token_with_scopes(
    data={"sub": "admin@example.com"},
    scopes=["read", "write", "admin"]
)
print(f"\n   Admin token (full access):")
print(f"   {admin_token[:60]}...")

print("\n3. Verifying Scopes:")

def verify_token_scopes(token: str, required_scopes: list) -> dict:
    try:
        payload = jwt.decode(token, SECRET_KEY, algorithms=[ALGORITHM])
        token_scopes = payload.get("scopes", [])
        
        # Check if all required scopes are present
        for scope in required_scopes:
            if scope not in token_scopes:
                return {"valid": False, "error": f"Missing scope: {scope}"}
        
        return {
            "valid": True,
            "user": payload.get("sub"),
            "scopes": token_scopes
        }
    except jwt.InvalidTokenError:
        return {"valid": False, "error": "Invalid token"}

# Test scope verification
print("\n   Testing regular user token for 'read' scope:")
result = verify_token_scopes(user_token, ["read"])
print(f"   {result}")

print("\n   Testing regular user token for 'admin' scope:")
result = verify_token_scopes(user_token, ["admin"])
print(f"   {result}")

print("\n   Testing admin token for 'admin' scope:")
result = verify_token_scopes(admin_token, ["admin"])
print(f"   {result}")

print("\n4. FastAPI Security Dependency:")
print('''
   from fastapi import Security
   from fastapi.security import SecurityScopes
   
   async def get_current_user(
       security_scopes: SecurityScopes,
       token: str = Depends(oauth2_scheme)
   ) -> User:
       # Decode token
       payload = jwt.decode(token, SECRET_KEY, algorithms=[ALGORITHM])
       
       # Get token scopes
       token_scopes = payload.get("scopes", [])
       
       # Verify required scopes
       for scope in security_scopes.scopes:
           if scope not in token_scopes:
               raise HTTPException(
                   status_code=403,
                   detail=f"Not enough permissions. Required: {scope}"
               )
       
       return await get_user_by_email(payload.get("sub"))
''')

print("\n5. Protecting Routes with Scopes:")
print('''
   @app.get("/items")
   async def read_items(
       user: User = Security(get_current_user, scopes=["read"])
   ):
       return await get_all_items()
   
   @app.post("/items")
   async def create_item(
       item: Item,
       user: User = Security(get_current_user, scopes=["write"])
   ):
       return await save_item(item)
   
   @app.get("/admin/users")
   async def list_all_users(
       user: User = Security(get_current_user, scopes=["admin"])
   ):
       return await get_all_users()
''')

print("\n=== Scope Hierarchy Pattern ===")
print("""
   admin ─────────────────────────>
     │                             │
     ├── write ──────────────────> │  All permissions
     │     │                       │
     │     └── read ─────────────> │
     │                             │
   
   Users can have multiple scopes:
   - Regular user: ["read"]
   - Editor: ["read", "write"]
   - Admin: ["read", "write", "admin"]
""")
```
