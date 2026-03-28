---
type: "EXAMPLE"
title: "Parameterized Queries with asyncpg"
---

## Vulnerable Code (Never Use)

```python
# ❌ DANGEROUS: String concatenation enables SQL injection
async def get_user_unsafe(username: str):
    query = f"SELECT * FROM users WHERE username = '{username}'"
    # Attacker sends: username = "' OR '1'='1"
    # Result: SELECT * FROM users WHERE username = '' OR '1'='1'
    # Returns ALL users!
    return await conn.fetch(query)
```

## Safe Parameterized Queries

```python
# ✅ SAFE: Parameters separated from query structure
import asyncpg

async def get_user_safe(conn: asyncpg.Connection, username: str):
    # asyncpg uses $1, $2 syntax
    query = "SELECT * FROM users WHERE username = $1"
    return await conn.fetch(query, username)
    
    # Even malicious input is treated as data, not SQL
    # username = "' OR '1'='1" → literally searches for that string
```

## Common Database Libraries

**asyncpg (PostgreSQL):**
```python
# Named parameters with $ notation
await conn.fetch(
    "SELECT * FROM users WHERE email = $1 AND active = $2",
    email, True
)

# Executemany for bulk operations
await conn.executemany(
    "INSERT INTO logs (user_id, action) VALUES ($1, $2)",
    [(1, "login"), (2, "logout"), (3, "purchase")]
)
```

**SQLAlchemy (ORM):**
```python
from sqlalchemy import select
from sqlalchemy.orm import Session

# ORM handles parameterization automatically
with Session(engine) as session:
    stmt = select(User).where(User.email == user_email)
    result = session.execute(stmt)
    
# Raw queries with text() - still parameterized
from sqlalchemy import text

with engine.connect() as conn:
    result = conn.execute(
        text("SELECT * FROM users WHERE id = :user_id"),
        {"user_id": user_id}
    )
```

**SQLite3:**
```python
import sqlite3

conn = sqlite3.connect('app.db')
cursor = conn.cursor()

# Use ? placeholders
cursor.execute(
    "SELECT * FROM products WHERE category = ? AND price < ?",
    (category, max_price)
)

# Never do this:
# cursor.execute(f"SELECT * FROM products WHERE name = '{user_input}'")  ❌
```

## Testing SQL Injection Defenses

```python
async def test_sql_injection_protection():
    # These inputs should be treated as literal strings, not SQL
    malicious_inputs = [
        "'; DROP TABLE users; --",
        "' OR '1'='1",
        "1; DELETE FROM orders",
        "admin'--",
    ]
    
    for malicious in malicious_inputs:
        # Should return empty list, not all users
        result = await get_user_safe(conn, malicious)
        assert len(result) == 0 or result[0].username == malicious
```
