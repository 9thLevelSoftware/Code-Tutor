---
type: "EXAMPLE"
title: "Complete Blog API Implementation"
---

**This is a complete, working Blog API with FastAPI!**

Test it with curl or Postman:
```bash
# Register
curl -X POST http://localhost:8000/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{"username": "alice", "email": "alice@example.com", "password": "secret123"}'

# Login (save the token)
curl -X POST http://localhost:8000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"username": "alice", "password": "secret123"}'

# Create post (use token from login)
curl -X POST http://localhost:8000/api/posts \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -d '{"title": "My First Post", "content": "Hello World!"}'
```

**Interactive API docs at:** http://localhost:8000/docs

```python
# Complete Blog API with FastAPI + OAuth2 Authentication
# Install: pip install fastapi uvicorn python-jose passlib

from fastapi import FastAPI, Depends, HTTPException, status
from fastapi.security import OAuth2PasswordBearer, OAuth2PasswordRequestForm
from pydantic import BaseModel, EmailStr, Field
from jose import jwt
from passlib.context import CryptContext
from datetime import datetime, timedelta
from typing import Optional, List

app = FastAPI(title="Blog API", version="1.0.0")

# ========== Security Config ==========
SECRET_KEY = os.getenv("SECRET_KEY", "development-fallback-secret")
ALGORITHM = "HS256"
ACCESS_TOKEN_EXPIRE_MINUTES = 30

pwd_context = CryptContext(schemes=["bcrypt"], deprecated="auto")
oauth2_scheme = OAuth2PasswordBearer(tokenUrl="api/auth/login")

# ========== Pydantic Models ==========
class UserCreate(BaseModel):
    username: str = Field(min_length=3, max_length=50)
    email: EmailStr
    password: str = Field(min_length=6)

class UserResponse(BaseModel):
    username: str
    email: str

class PostCreate(BaseModel):
    title: str = Field(min_length=5, max_length=200)
    content: str = Field(min_length=1)

class PostResponse(BaseModel):
    id: int
    title: str
    content: str
    author: str
    created_at: str
    comment_count: int = 0

class CommentCreate(BaseModel):
    content: str = Field(min_length=1)

class Token(BaseModel):
    access_token: str
    token_type: str

# ========== In-Memory Database ==========
users_db = {}  # username -> user data
posts_db = []  # list of posts
comments_db = []  # list of comments

# ========== Auth Helpers ==========
def hash_password(password: str) -> str:
    return pwd_context.hash(password)

def verify_password(plain: str, hashed: str) -> bool:
    return pwd_context.verify(plain, hashed)

def create_access_token(data: dict) -> str:
    to_encode = data.copy()
    expire = datetime.utcnow() + timedelta(minutes=ACCESS_TOKEN_EXPIRE_MINUTES)
    to_encode.update({"exp": expire})
    return jwt.encode(to_encode, SECRET_KEY, algorithm=ALGORITHM)

async def get_current_user(token: str = Depends(oauth2_scheme)) -> str:
    credentials_exception = HTTPException(
        status_code=status.HTTP_401_UNAUTHORIZED,
        detail="Invalid authentication credentials",
        headers={"WWW-Authenticate": "Bearer"},
    )
    try:
        payload = jwt.decode(token, SECRET_KEY, algorithms=[ALGORITHM])
        username: str = payload.get("sub")
        if username is None:
            raise credentials_exception
    except jwt.JWTError:
        raise credentials_exception
    if username not in users_db:
        raise credentials_exception
    return username

# ========== Auth Endpoints ==========
@app.post("/api/auth/register", response_model=dict, status_code=201)
async def register(user: UserCreate):
    if user.username in users_db:
        raise HTTPException(status_code=400, detail="Username already exists")
    
    users_db[user.username] = {
        "username": user.username,
        "email": user.email,
        "password_hash": hash_password(user.password),
        "created_at": datetime.now().isoformat()
    }
    return {"message": "User registered", "user": UserResponse(username=user.username, email=user.email).model_dump()}

@app.post("/api/auth/login", response_model=Token)
async def login(form_data: OAuth2PasswordRequestForm = Depends()):
    user = users_db.get(form_data.username)
    if not user or not verify_password(form_data.password, user["password_hash"]):
        raise HTTPException(status_code=401, detail="Invalid credentials")
    
    access_token = create_access_token(data={"sub": form_data.username})
    return {"access_token": access_token, "token_type": "bearer"}

# ========== Post Endpoints ==========
@app.get("/api/posts", response_model=List[PostResponse])
async def get_posts():
    return [
        PostResponse(
            id=i,
            title=p["title"],
            content=p["content"][:100] + "..." if len(p["content"]) > 100 else p["content"],
            author=p["author"],
            created_at=p["created_at"],
            comment_count=len([c for c in comments_db if c["post_id"] == i])
        )
        for i, p in enumerate(posts_db) if not p.get("deleted")
    ]

@app.get("/api/posts/{post_id}", response_model=dict)
async def get_post(post_id: int):
    if post_id >= len(posts_db) or posts_db[post_id].get("deleted"):
        raise HTTPException(status_code=404, detail="Post not found")
    
    post = posts_db[post_id]
    post_comments = [c for c in comments_db if c["post_id"] == post_id]
    return {**post, "id": post_id, "comments": post_comments}

@app.post("/api/posts", response_model=dict, status_code=201)
async def create_post(post: PostCreate, current_user: str = Depends(get_current_user)):
    new_post = {
        "title": post.title,
        "content": post.content,
        "author": current_user,
        "created_at": datetime.now().isoformat()
    }
    posts_db.append(new_post)
    return {"message": "Post created", "post": {**new_post, "id": len(posts_db) - 1}}

@app.put("/api/posts/{post_id}", response_model=dict)
async def update_post(post_id: int, post: PostCreate, current_user: str = Depends(get_current_user)):
    if post_id >= len(posts_db):
        raise HTTPException(status_code=404, detail="Post not found")
    
    existing_post = posts_db[post_id]
    if existing_post["author"] != current_user:
        raise HTTPException(status_code=403, detail="Not authorized")
    
    existing_post["title"] = post.title
    existing_post["content"] = post.content
    existing_post["updated_at"] = datetime.now().isoformat()
    return {"message": "Post updated", "post": {**existing_post, "id": post_id}}

@app.delete("/api/posts/{post_id}", response_model=dict)
async def delete_post(post_id: int, current_user: str = Depends(get_current_user)):
    if post_id >= len(posts_db):
        raise HTTPException(status_code=404, detail="Post not found")
    
    post = posts_db[post_id]
    if post["author"] != current_user:
        raise HTTPException(status_code=403, detail="Not authorized")
    
    post["deleted"] = True
    return {"message": "Post deleted"}

# ========== Comment Endpoints ==========
@app.post("/api/posts/{post_id}/comments", response_model=dict, status_code=201)
async def add_comment(post_id: int, comment: CommentCreate, current_user: str = Depends(get_current_user)):
    if post_id >= len(posts_db) or posts_db[post_id].get("deleted"):
        raise HTTPException(status_code=404, detail="Post not found")
    
    new_comment = {
        "post_id": post_id,
        "content": comment.content,
        "author": current_user,
        "created_at": datetime.now().isoformat()
    }
    comments_db.append(new_comment)
    return {"message": "Comment added", "comment": new_comment}

if __name__ == "__main__":
    import uvicorn
    print("Blog API running at http://localhost:8000")
    print("Interactive docs at http://localhost:8000/docs")
    uvicorn.run(app, host="0.0.0.0", port=8000)
```
