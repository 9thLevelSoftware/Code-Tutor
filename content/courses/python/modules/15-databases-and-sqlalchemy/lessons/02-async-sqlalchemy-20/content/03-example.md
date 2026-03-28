---
type: "EXAMPLE"
title: "Defining Models with Mapped Types"
---

**SQLAlchemy 2.0 models use type annotations for column definitions:**

**Mapped Type Benefits:**
- Full IDE autocomplete support
- Type checking catches errors early
- Cleaner, more Pythonic syntax
- Required vs optional fields are explicit

**Column Patterns:**
```python
# Required field
name: Mapped[str] = mapped_column(String(100))

# Optional field (nullable)
bio: Mapped[str | None] = mapped_column(String(500), nullable=True)

# With default
from datetime import datetime, timezone
created_at: Mapped[datetime] = mapped_column(default=datetime.now(timezone.utc))

# Relationship
posts: Mapped[list["Post"]] = relationship(back_populates="author")
```

```python
from sqlalchemy.orm import DeclarativeBase, Mapped, mapped_column, relationship
from sqlalchemy import ForeignKey, String
from datetime import datetime, timezone
from typing import List

class Base(DeclarativeBase):
    """Base class for all models."""
    pass

class User(Base):
    """User model with typed columns."""
    __tablename__ = "users"
    
    # Primary key - auto-increment integer
    id: Mapped[int] = mapped_column(primary_key=True)
    
    # Required string fields with max length
    email: Mapped[str] = mapped_column(String(255), unique=True)
    name: Mapped[str] = mapped_column(String(100))
    
    # Timestamp with default value
    created_at: Mapped[datetime] = mapped_column(default=datetime.now(timezone.utc))
    
    # One-to-many relationship
    transactions: Mapped[List["Transaction"]] = relationship(
        back_populates="user",
        cascade="all, delete-orphan"  # Delete transactions when user deleted
    )

class Transaction(Base):
    """Transaction model linked to User."""
    __tablename__ = "transactions"
    
    id: Mapped[int] = mapped_column(primary_key=True)
    amount: Mapped[float]  # No mapped_column needed for simple types
    category: Mapped[str] = mapped_column(String(50))
    description: Mapped[str | None] = mapped_column(String(200), nullable=True)
    created_at: Mapped[datetime] = mapped_column(default=datetime.now(timezone.utc))
    
    # Foreign key to users table
    user_id: Mapped[int] = mapped_column(ForeignKey("users.id"))
    
    # Many-to-one relationship back to User
    user: Mapped["User"] = relationship(back_populates="transactions")

# Demonstration
print("=== SQLAlchemy 2.0 Models ===")
print("\nUser model columns:")
for col in User.__table__.columns:
    print(f"  {col.name}: {col.type} (nullable={col.nullable})")

print("\nTransaction model columns:")
for col in Transaction.__table__.columns:
    print(f"  {col.name}: {col.type} (nullable={col.nullable})")

print("\nRelationships:")
print("  User.transactions -> List[Transaction]")
print("  Transaction.user -> User")

print("\nKey improvements in 2.0:")
print("  - Mapped[type] provides type hints")
print("  - mapped_column() replaces Column()")
print("  - Optional fields use str | None syntax")
print("  - DeclarativeBase replaces declarative_base()")
```
