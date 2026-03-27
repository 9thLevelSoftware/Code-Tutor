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

@dataclass
class AnonymousUser:
    is_authenticated: bool = False
    username: str = ""
    permissions: List[str] = field(default_factory=list)

def hash_password(password: str) -> str:
    return make_password(password)

class AuthSystem:
    MAX_FAILED_ATTEMPTS = 5
    LOCKOUT_DURATION = timedelta(minutes=15)
    
    def __init__(self):
        self.users: Dict[str, User] = {}
        self.sessions: Dict[str, User] = {}
        self._next_id = 1
    
    def _validate_username(self, username: str) -> None:
        if not username or len(username) < 3 or len(username) > 20:
            raise AuthError("Username must be 3-20 characters")
        if not re.match(r'^[a-zA-Z0-9_]+$', username):
            raise AuthError("Username must be alphanumeric")
    
    def _validate_email(self, email: str) -> None:
        pattern = r'^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$'
        if not re.match(pattern, email):
            raise AuthError("Invalid email format")
    
    def _validate_password(self, password: str) -> None:
        if len(password) < 8:
            raise AuthError("Password must be at least 8 characters")
        if not re.search(r'[a-zA-Z]', password):
            raise AuthError("Password must contain a letter")
        if not re.search(r'[0-9]', password):
            raise AuthError("Password must contain a number")
    
    def register(self, username: str, email: str, password: str) -> User:
        self._validate_username(username)
        self._validate_email(email)
        self._validate_password(password)
        
        if username in self.users:
            raise AuthError("Username already exists")
        
        user = User(
            id=self._next_id,
            username=username,
            email=email,
            password_hash=hash_password(password)
        )
        self._next_id += 1
        self.users[username] = user
        return user
    
    def login(self, username: str, password: str) -> str:
        user = self.users.get(username)
        if not user:
            raise AuthError("Invalid username or password")
        
        # Check if locked
        if user.locked_until and datetime.now() < user.locked_until:
            remaining = (user.locked_until - datetime.now()).seconds // 60
            raise AuthError(f"Account locked. Try again in {remaining} minutes")
        
        # Reset lock if expired
        if user.locked_until and datetime.now() >= user.locked_until:
            user.locked_until = None
            user.failed_attempts = 0
        
        if not user.check_password(password):
            user.failed_attempts += 1
            if user.failed_attempts >= self.MAX_FAILED_ATTEMPTS:
                user.locked_until = datetime.now() + self.LOCKOUT_DURATION
                raise AuthError("Account locked due to too many failed attempts")
            raise AuthError("Invalid username or password")
        
        # Success - reset attempts and create session
        user.failed_attempts = 0
        session_id = secrets.token_urlsafe(32)
        self.sessions[session_id] = user
        return session_id
    
    def logout(self, session_id: str) -> bool:
        if session_id in self.sessions:
            del self.sessions[session_id]
            return True
        return False
    
    def change_password(self, username: str, old_password: str, new_password: str) -> bool:
        user = self.users.get(username)
        if not user:
            raise AuthError("User not found")
        if not user.check_password(old_password):
            raise AuthError("Current password is incorrect")
        self._validate_password(new_password)
        user.set_password(new_password)
        return True
    
    def get_user(self, session_id: str):
        return self.sessions.get(session_id, AnonymousUser())


# Decorators
def login_required(view_func: Callable) -> Callable:
    @wraps(view_func)
    def wrapper(request, *args, **kwargs):
        if not request.user.is_authenticated:
            raise AuthError("Login required")
        return view_func(request, *args, **kwargs)
    return wrapper

def permission_required(perm: str) -> Callable:
    def decorator(view_func: Callable) -> Callable:
        @wraps(view_func)
        def wrapper(request, *args, **kwargs):
            if not request.user.is_authenticated:
                raise AuthError("Login required")
            if perm not in request.user.permissions:
                raise AuthError(f"Permission '{perm}' required")
            return view_func(request, *args, **kwargs)
        return wrapper
    return decorator


print("=== Auth System Tests ===")

auth = AuthSystem()

print("\n1. Registration:")
try:
    user = auth.register("alice", "alice@example.com", "SecurePass123")
    print(f"   Registered: {user.username}")
except AuthError as e:
    print(f"   Error: {e}")

print("\n2. Login:")
try:
    session = auth.login("alice", "SecurePass123")
    print(f"   Session created: {session[:20]}...")
except AuthError as e:
    print(f"   Error: {e}")

print("\n3. Failed Login Attempts:")
for i in range(6):
    try:
        auth.login("alice", "wrongpassword")
    except AuthError as e:
        print(f"   Attempt {i+1}: {e}")
        if "locked" in str(e).lower():
            break

# Reset for password change test
auth.users["alice"].locked_until = None
auth.users["alice"].failed_attempts = 0

print("\n4. Password Change:")
try:
    auth.change_password("alice", "SecurePass123", "NewSecure456")
    print("   Password changed successfully")
except AuthError as e:
    print(f"   Error: {e}")