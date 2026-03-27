from dataclasses import dataclass, field
from typing import Dict, Optional, List, Callable
from datetime import datetime, timedelta
from functools import wraps
from django.contrib.auth.hashers import make_password
import secrets
import re

class AuthError(Exception):
    pass

@dataclass
class User:
    id: int
    username: str
    email: str
    password_hash: str
    is_active: bool = True
    is_staff: bool = False
    permissions: List[str] = field(default_factory=list)
    failed_attempts: int = 0
    locked_until: Optional[datetime] = None
    
    @property
    def is_authenticated(self) -> bool:
        return True
    
    def check_password(self, raw_password: str) -> bool:
        return hash_password(raw_password) == self.password_hash
    
    def set_password(self, raw_password: str) -> None:
        self.password_hash = hash_password(raw_password)

def hash_password(password: str) -> str:
    return make_password(password)

class AuthSystem:
    MAX_FAILED_ATTEMPTS = 5
    LOCKOUT_DURATION = timedelta(minutes=15)
    
    def __init__(self):
        self.users: Dict[str, User] = {}
        self.sessions: Dict[str, User] = {}
        self._next_id = 1
    
    def register(self, username: str, email: str, password: str) -> User:
        """Register a new user with validation."""
        # TODO: Validate username (3-20 chars, alphanumeric)
        # TODO: Validate email format
        # TODO: Validate password (8+ chars, has number and letter)
        # TODO: Check username not taken
        # TODO: Create and store user
        pass
    
    def login(self, username: str, password: str) -> str:
        """Login user and return session ID."""
        # TODO: Check if user exists
        # TODO: Check if account is locked
        # TODO: Verify password
        # TODO: Track failed attempts
        # TODO: Create session on success
        pass
    
    def logout(self, session_id: str) -> bool:
        """Logout user by clearing session."""
        pass
    
    def change_password(self, username: str, old_password: str, new_password: str) -> bool:
        """Change password (requires old password)."""
        pass
    
    def get_user(self, session_id: str):
        """Get user from session."""
        pass


# Decorators
def login_required(view_func: Callable) -> Callable:
    """Require authenticated user."""
    pass

def permission_required(perm: str) -> Callable:
    """Require specific permission."""
    pass


# Test the system
print("=== Auth System Tests ===")

auth = AuthSystem()

# Test registration
print("\n1. Registration:")
try:
    user = auth.register("alice", "alice@example.com", "SecurePass123")
    print(f"   Registered: {user.username}")
except AuthError as e:
    print(f"   Error: {e}")

# Test login
print("\n2. Login:")
try:
    session = auth.login("alice", "SecurePass123")
    print(f"   Session created: {session[:20]}...")
except AuthError as e:
    print(f"   Error: {e}")

# Test failed login attempts
print("\n3. Failed Login Attempts:")
for i in range(6):
    try:
        auth.login("alice", "wrongpassword")
    except AuthError as e:
        print(f"   Attempt {i+1}: {e}")

# Test password change
print("\n4. Password Change:")
try:
    auth.change_password("alice", "SecurePass123", "NewSecure456")
    print("   Password changed successfully")
except AuthError as e:
    print(f"   Error: {e}")