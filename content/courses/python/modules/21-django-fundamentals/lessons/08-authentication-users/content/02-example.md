---
type: "EXAMPLE"
title: "Login, Logout, and Registration"
---

Implementing user authentication views for our finance tracker:

**Expected Output:**
```
User 'john' registered successfully
Login successful for john
Logout completed
```

```python
from dataclasses import dataclass, field
from typing import Dict, Optional, List
from datetime import datetime
from django.contrib.auth.hashers import make_password
import secrets
import re

@dataclass
class User:
    """Simulated Django User model."""
    id: int
    username: str
    email: str
    password_hash: str
    first_name: str = ""
    last_name: str = ""
    is_active: bool = True
    is_staff: bool = False
    is_superuser: bool = False
    date_joined: datetime = field(default_factory=datetime.now)
    last_login: Optional[datetime] = None
    
    @property
    def is_authenticated(self) -> bool:
        """Always True for real users (False for AnonymousUser)."""
        return True
    
    def get_full_name(self) -> str:
        return f"{self.first_name} {self.last_name}".strip()
    
    def check_password(self, raw_password: str) -> bool:
        """Verify password against stored hash."""
        return hash_password(raw_password) == self.password_hash


@dataclass
class AnonymousUser:
    """Represents unauthenticated users."""
    is_authenticated: bool = False
    is_active: bool = False
    is_staff: bool = False
    is_superuser: bool = False
    username: str = ""


def hash_password(password: str) -> str:
    """Hash password (simplified - Django uses PBKDF2/bcrypt)."""
    return make_password(password)


class AuthenticationError(Exception):
    """Raised when authentication fails."""
    pass


class AuthSystem:
    """Simulated Django authentication system."""
    
    def __init__(self):
        self.users: Dict[str, User] = {}
        self.sessions: Dict[str, User] = {}  # session_id -> user
        self._next_id = 1
    
    def create_user(self, username: str, email: str, password: str,
                    **extra_fields) -> User:
        """Create and save a new user."""
        # Validate
        if not username:
            raise ValueError("Username is required")
        if username in self.users:
            raise ValueError("Username already exists")
        if not self._is_valid_email(email):
            raise ValueError("Invalid email format")
        if len(password) < 8:
            raise ValueError("Password must be at least 8 characters")
        
        user = User(
            id=self._next_id,
            username=username,
            email=email,
            password_hash=hash_password(password),
            **extra_fields
        )
        self._next_id += 1
        self.users[username] = user
        return user
    
    def authenticate(self, username: str, password: str) -> Optional[User]:
        """Verify credentials and return user or None."""
        user = self.users.get(username)
        if user and user.is_active and user.check_password(password):
            return user
        return None
    
    def login(self, username: str, password: str) -> str:
        """Authenticate and create session. Returns session ID."""
        user = self.authenticate(username, password)
        if not user:
            raise AuthenticationError("Invalid username or password")
        
        # Create session
        session_id = secrets.token_urlsafe(32)
        self.sessions[session_id] = user
        user.last_login = datetime.now()
        return session_id
    
    def logout(self, session_id: str) -> bool:
        """End session."""
        if session_id in self.sessions:
            del self.sessions[session_id]
            return True
        return False
    
    def get_user(self, session_id: str) -> User:
        """Get user from session (like request.user)."""
        return self.sessions.get(session_id, AnonymousUser())
    
    def _is_valid_email(self, email: str) -> bool:
        pattern = r'^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$'
        return bool(re.match(pattern, email))


# Demo the authentication system
print("=== Django Authentication Demo ===")

auth = AuthSystem()

# Register a new user
print("\n1. User Registration:")
user = auth.create_user(
    username="john",
    email="john@example.com",
    password="securepass123",
    first_name="John",
    last_name="Doe"
)
print(f"User 'john' registered successfully")
print(f"   ID: {user.id}, Email: {user.email}")

# Login
print("\n2. User Login:")
session_id = auth.login("john", "securepass123")
print(f"Login successful for john")
print(f"   Session created: {session_id[:20]}...")

# Check authentication in a view
print("\n3. Checking Authentication:")
current_user = auth.get_user(session_id)
if current_user.is_authenticated:
    print(f"   Logged in as: {current_user.username}")
    print(f"   Full name: {current_user.get_full_name()}")

# Logout
print("\n4. User Logout:")
auth.logout(session_id)
print("Logout completed")

# Verify logged out
current_user = auth.get_user(session_id)
print(f"   Authenticated: {current_user.is_authenticated}")
```
