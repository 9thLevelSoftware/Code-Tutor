---
type: "WARNING"
title: "FastAPI Common Mistakes"
---

**Pitfalls When Getting Started:**

❌ **Don't mix sync and async incorrectly**

```python
# WRONG: Blocking call in async function
@app.get("/users")
async def get_users():
    # requests.get() blocks the event loop!
    response = requests.get("http://api.example.com/users")
    return response.json()

# RIGHT: Use async HTTP client
import httpx

@app.get("/users")
async def get_users():
    async with httpx.AsyncClient() as client:
        response = await client.get("http://api.example.com/users")
        return response.json()
```

❌ **Don't forget to run with Uvicorn**

```bash
# WRONG: This doesn't work
python main.py

# RIGHT: Use uvicorn
uvicorn main:app --reload
```

❌ **Don't ignore validation errors**

```python
# FastAPI returns 422 for validation errors
# Always handle them in your frontend:
{
    "detail": [
        {
            "loc": ["body", "email"],
            "msg": "value is not a valid email address",
            "type": "value_error.email"
        }
    ]
}
```

❌ **Don't put secrets in code**

```python
# WRONG
SECRET_KEY = os.getenv("SECRET_KEY", "development-fallback-secret")

# RIGHT: Use environment variables
import os
SECRET_KEY = os.getenv("SECRET_KEY")

# BETTER: Use pydantic-settings
from pydantic_settings import BaseSettings

class Settings(BaseSettings):
    secret_key: str
    
    class Config:
        env_file = ".env"
```

❌ **Don't forget response model security**

```python
# WRONG: Exposing internal fields
@app.get("/users/{id}")
def get_user(id: int):
    return db.get_user(id)  # Returns password hash!

# RIGHT: Use response_model
class UserResponse(BaseModel):
    id: int
    email: str
    # No password field!

@app.get("/users/{id}", response_model=UserResponse)
def get_user(id: int):
    return db.get_user(id)  # Password filtered out
```
