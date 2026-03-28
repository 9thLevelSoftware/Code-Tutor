---
type: "WARNING"
title: "CRUD API Common Mistakes"
---

**Pitfalls When Building Complete APIs:**

❌ **Don't expose database IDs directly**

```python
# RISKY: Auto-increment IDs reveal information
GET /users/1  # First user
GET /users/2  # Second user
# Attackers can enumerate all users!

# BETTER: Use UUIDs
import uuid
from sqlalchemy import Column, String

class User(Base):
    id = Column(String, primary_key=True, default=lambda: str(uuid.uuid4()))
```

❌ **Don't forget pagination**

```python
# WRONG: Returns ALL items
@app.get("/items")
def get_items(db = Depends(get_db)):
    return db.query(Item).all()  # 1 million items!

# RIGHT: Paginate
@app.get("/items")
def get_items(
    skip: int = 0,
    limit: int = Query(default=20, le=100),
    db = Depends(get_db)
):
    return db.query(Item).offset(skip).limit(limit).all()
```

❌ **Don't ignore soft deletes**

```python
# RISKY: Hard delete loses data
@app.delete("/users/{id}")
def delete_user(id: int, db = Depends(get_db)):
    db.query(User).filter(User.id == id).delete()
    # Data is GONE forever!

# BETTER: Soft delete
from datetime import datetime, timezone

class User(Base):
    deleted_at = Column(DateTime, nullable=True)

@app.delete("/users/{id}")
def delete_user(id: int, db = Depends(get_db)):
    user = db.query(User).filter(User.id == id).first()
    user.deleted_at = datetime.now(timezone.utc)
    db.commit()
    # Data recoverable!
```

❌ **Don't forget CORS**

```python
# WRONG: Frontend can't access your API
# Browser blocks cross-origin requests

# RIGHT: Configure CORS
from fastapi.middleware.cors import CORSMiddleware

app.add_middleware(
    CORSMiddleware,
    allow_origins=["http://localhost:3000"],  # Your frontend
    allow_methods=["*"],
    allow_headers=["*"],
)
```

❌ **Don't return 200 for everything**

```python
# WRONG: All responses are 200
@app.post("/users")
def create_user(user: User):
    created = db.create(user)
    return created  # 200 OK

# RIGHT: Use appropriate status codes
from fastapi import status

@app.post("/users", status_code=status.HTTP_201_CREATED)
def create_user(user: User):
    created = db.create(user)
    return created  # 201 Created
```
