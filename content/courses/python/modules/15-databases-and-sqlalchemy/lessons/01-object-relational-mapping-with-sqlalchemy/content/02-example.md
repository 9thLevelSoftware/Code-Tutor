---
type: "EXAMPLE"
title: "Code Example: SQLAlchemy Basics"
---

**SQLAlchemy setup:**

```python
from sqlalchemy import create_engine
from sqlalchemy.orm import sessionmaker, declarative_base

# Create engine (database connection)
engine = create_engine('sqlite:///app.db')

# Create base class for models
Base = declarative_base()

# Create session factory
Session = sessionmaker(bind=engine)
```

**Define a model:**
```python
from sqlalchemy import Column, Integer, String

class User(Base):
    __tablename__ = 'users'
    
    id = Column(Integer, primary_key=True)
    name = Column(String(100), nullable=False)
    email = Column(String(100), unique=True)
```

**CRUD Operations:**
```python
# Create
user = User(name='Alice', email='alice@test.com')
session.add(user)
session.commit()

# Read
user = session.query(User).filter_by(email='alice@test.com').first()

# Update
user.name = 'Alice Smith'
session.commit()

# Delete
session.delete(user)
session.commit()
```

```python
from sqlalchemy import create_engine, Column, Integer, String, ForeignKey, DateTime
from sqlalchemy.orm import sessionmaker, declarative_base, relationship
from datetime import datetime, timezone

print("=== SQLAlchemy Setup ===")

# Create in-memory SQLite database
engine = create_engine('sqlite:///:memory:', echo=False)

# Base class for all models
Base = declarative_base()

# Session factory
Session = sessionmaker(bind=engine)

print("\n=== Defining Models ===")

class User(Base):
    """User model with SQLAlchemy"""
    __tablename__ = 'users'
    
    # Columns
    id = Column(Integer, primary_key=True)
    name = Column(String(100), nullable=False)
    email = Column(String(100), unique=True, nullable=False)
    created_at = Column(DateTime, default=datetime.now(timezone.utc))
    
    # Relationship to posts
    posts = relationship('Post', back_populates='author')
    
    def __repr__(self):
        return f"<User(id={self.id}, name='{self.name}')>"

class Post(Base):
    """Blog post model"""
    __tablename__ = 'posts'
    
    id = Column(Integer, primary_key=True)
    title = Column(String(200), nullable=False)
    content = Column(String(1000))
    author_id = Column(Integer, ForeignKey('users.id'))
    created_at = Column(DateTime, default=datetime.now(timezone.utc))
    
    # Relationship to user
    author = relationship('User', back_populates='posts')
    
    def __repr__(self):
        return f"<Post(id={self.id}, title='{self.title}')>"

# Create all tables
Base.metadata.create_all(engine)
print("Tables created: users, posts")

print("\n=== CRUD Operations ===")

# Create a session
session = Session()

# CREATE - Add users
print("\n1. CREATE - Adding users...")
alice = User(name='Alice', email='alice@example.com')
bob = User(name='Bob', email='bob@example.com')

session.add(alice)
session.add(bob)
session.commit()

print(f"  Created: {alice}")
print(f"  Created: {bob}")

# CREATE - Add posts
post1 = Post(title='Hello World', content='My first post!', author=alice)
post2 = Post(title='SQLAlchemy Tips', content='ORMs are great!', author=alice)
post3 = Post(title='Python Tutorial', content='Learn Python', author=bob)

session.add_all([post1, post2, post3])
session.commit()

print(f"  Created: {post1}")
print(f"  Created: {post2}")
print(f"  Created: {post3}")

# READ - Query users
print("\n2. READ - Querying...")

# Get all users
all_users = session.query(User).all()
print(f"  All users: {all_users}")

# Filter by email
user = session.query(User).filter_by(email='alice@example.com').first()
print(f"  Found by email: {user}")

# Filter with conditions
users_with_a = session.query(User).filter(User.name.like('A%')).all()
print(f"  Names starting with A: {users_with_a}")

# Access relationships
print(f"  Alice's posts: {user.posts}")

# UPDATE
print("\n3. UPDATE - Modifying...")
alice.name = 'Alice Smith'
session.commit()
print(f"  Updated: {alice}")

# DELETE
print("\n4. DELETE - Removing...")
session.delete(post3)
session.commit()
print("  Deleted Bob's post")

# Verify deletion
remaining_posts = session.query(Post).all()
print(f"  Remaining posts: {remaining_posts}")

print("\n=== Advanced Queries ===")

# Join query
print("\nPosts with author names:")
for post in session.query(Post).join(User).all():
    print(f"  '{post.title}' by {post.author.name}")

# Count
post_count = session.query(Post).count()
print(f"\nTotal posts: {post_count}")

# Order by
print("\nPosts ordered by title:")
for post in session.query(Post).order_by(Post.title).all():
    print(f"  {post.title}")

session.close()

print("\n=== SQLAlchemy with FastAPI ===")

fastapi_example = '''
from fastapi import FastAPI, Depends, HTTPException
from sqlalchemy import create_engine
from sqlalchemy.orm import sessionmaker, Session

# Database setup
engine = create_engine('sqlite:///app.db')
SessionLocal = sessionmaker(bind=engine)

# Dependency to get database session
def get_db():
    db = SessionLocal()
    try:
        yield db
    finally:
        db.close()

app = FastAPI()

@app.get('/users')
def get_users(db: Session = Depends(get_db)):
    users = db.query(User).all()
    return [{'id': u.id, 'name': u.name} for u in users]

@app.post('/users', status_code=201)
def create_user(name: str, email: str, db: Session = Depends(get_db)):
    user = User(name=name, email=email)
    db.add(user)
    db.commit()
    db.refresh(user)
    return {'id': user.id}
'''

print(fastapi_example)
```
