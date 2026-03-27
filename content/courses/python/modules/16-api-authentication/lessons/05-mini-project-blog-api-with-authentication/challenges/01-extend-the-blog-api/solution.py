import os
from fastapi import FastAPI, Depends, HTTPException, Query
from fastapi.security import OAuth2PasswordBearer
from pydantic import BaseModel
from jose import jwt
from passlib.context import CryptContext
from datetime import datetime, timedelta
from typing import Optional, List

app = FastAPI(title="Extended Blog API")

# Security config
SECRET_KEY = os.getenv("SECRET_KEY", "development-fallback-secret")
ALGORITHM = "HS256"
pwd_context = CryptContext(schemes=["bcrypt"], deprecated="auto")
oauth2_scheme = OAuth2PasswordBearer(tokenUrl="api/auth/login")

# Pydantic models
class UserProfile(BaseModel):
    username: str
    bio: str
    post_count: int
    joined: str

class PostResponse(BaseModel):
    id: int
    title: str
    content: str
    author: str
    created_at: str
    like_count: int
    comment_count: int

# Data stores
users_db = {}
posts_db = []
comments_db = []
likes_db = []

# Auth helpers
async def get_current_user(token: str = Depends(oauth2_scheme)) -> str:
    try:
        payload = jwt.decode(token, SECRET_KEY, algorithms=[ALGORITHM])
        return payload.get("sub")
    except:
        raise HTTPException(status_code=401, detail="Invalid token")

def has_liked(username: str, post_id: int) -> bool:
    return any(l["username"] == username and l["post_id"] == post_id for l in likes_db)

def get_like_count(post_id: int) -> int:
    return sum(1 for l in likes_db if l["post_id"] == post_id)

# Feature 1: Likes System
@app.post("/api/posts/{post_id}/like", status_code=201)
async def like_post(post_id: int, current_user: str = Depends(get_current_user)):
    if post_id >= len(posts_db) or posts_db[post_id].get("deleted"):
        raise HTTPException(status_code=404, detail="Post not found")
    
    if has_liked(current_user, post_id):
        raise HTTPException(status_code=400, detail="Already liked")
    
    likes_db.append({"username": current_user, "post_id": post_id, "created_at": datetime.now().isoformat()})
    return {"message": "Post liked", "like_count": get_like_count(post_id)}

# Feature 2: User Profiles
@app.get("/api/users/{username}", response_model=UserProfile)
async def get_user_profile(username: str):
    if username not in users_db:
        raise HTTPException(status_code=404, detail="User not found")
    
    user = users_db[username]
    post_count = sum(1 for p in posts_db if p["author"] == username and not p.get("deleted"))
    
    return UserProfile(
        username=user["username"],
        bio=user.get("bio", ""),
        post_count=post_count,
        joined=user["created_at"]
    )

# Feature 3: Search Posts
@app.get("/api/posts", response_model=List[PostResponse])
async def get_posts(search: Optional[str] = Query(None)):
    result = []
    search_lower = search.lower().strip() if search else None
    
    for i, post in enumerate(posts_db):
        if post.get("deleted"):
            continue
        
        if search_lower:
            if search_lower not in post["title"].lower() and search_lower not in post["content"].lower():
                continue
        
        result.append(PostResponse(
            id=i,
            title=post["title"],
            content=post["content"][:100] + "..." if len(post["content"]) > 100 else post["content"],
            author=post["author"],
            created_at=post["created_at"],
            like_count=get_like_count(i),
            comment_count=sum(1 for c in comments_db if c["post_id"] == i)
        ))
    
    return result

# Get single post with like_count
@app.get("/api/posts/{post_id}")
async def get_post(post_id: int):
    if post_id >= len(posts_db) or posts_db[post_id].get("deleted"):
        raise HTTPException(status_code=404, detail="Post not found")
    
    post = posts_db[post_id]
    post_comments = [c for c in comments_db if c["post_id"] == post_id]
    
    return {
        **post,
        "id": post_id,
        "like_count": get_like_count(post_id),
        "comments": post_comments
    }

if __name__ == "__main__":
    import uvicorn
    print("Extended Blog API - http://localhost:8000/docs")
    uvicorn.run(app, host="0.0.0.0", port=8000)