---
type: "EXAMPLE"
title: "Production-Ready Session Management"
---

**Complete session management implementation with security best practices:**

```python
import secrets
import hashlib
from datetime import datetime, timedelta
from typing import Optional, Dict, Any

class SecureSessionManager:
    """Production-grade session management with security features."""
    
    def __init__(self, redis_client=None):
        self.redis = redis_client
        self.sessions: Dict[str, Dict] = {}  # Fallback if no Redis
        self.default_ttl = timedelta(hours=24)
        self.absolute_max = timedelta(days=7)
    
    def _hash_session_id(self, session_id: str) -> str:
        """Hash session ID for storage (prevents exposure if DB is compromised)."""
        return hashlib.sha256(session_id.encode()).hexdigest()
    
    def create_session(self, user_id: int, user_data: Dict[str, Any],
                     ip_address: str, user_agent: str) -> str:
        """Create a new secure session."""
        # Generate cryptographically secure session ID
        raw_session_id = secrets.token_urlsafe(32)
        hashed_id = self._hash_session_id(raw_session_id)
        
        now = datetime.now()
        session_data = {
            "user_id": user_id,
            "user_data": user_data,
            "ip_address": ip_address,
            "user_agent": user_agent,
            "created_at": now,
            "last_accessed": now,
            "absolute_expires": now + self.absolute_max
        }
        
        # Store in Redis or memory
        if self.redis:
            import json
            self.redis.setex(
                f"session:{hashed_id}",
                int(self.default_ttl.total_seconds()),
                json.dumps(session_data, default=str)
            )
        else:
            self.sessions[hashed_id] = session_data
        
        return raw_session_id  # Return unhashed to client
    
    def validate_session(self, session_id: str, 
                        current_ip: str, current_ua: str) -> Optional[Dict]:
        """Validate session with security checks."""
        hashed_id = self._hash_session_id(session_id)
        
        # Retrieve session
        if self.redis:
            import json
            data = self.redis.get(f"session:{hashed_id}")
            if not data:
                return None
            session = json.loads(data)
        else:
            session = self.sessions.get(hashed_id)
        
        if not session:
            return None
        
        # Check absolute expiry
        expires = datetime.fromisoformat(session["absolute_expires"])
        if datetime.now() > expires:
            self.destroy_session(session_id)
            return None
        
        # Security: Detect potential hijacking
        if session.get("ip_address") != current_ip:
            # Could flag for review or require re-auth
            pass
        
        # Update last accessed (sliding window)
        session["last_accessed"] = datetime.now().isoformat()
        
        # Refresh TTL in Redis
        if self.redis:
            import json
            self.redis.setex(
                f"session:{hashed_id}",
                int(self.default_ttl.total_seconds()),
                json.dumps(session, default=str)
            )
        
        return session
    
    def destroy_session(self, session_id: str) -> bool:
        """Immediately invalidate a session (logout)."""
        hashed_id = self._hash_session_id(session_id)
        
        if self.redis:
            return self.redis.delete(f"session:{hashed_id}") > 0
        else:
            return self.sessions.pop(hashed_id, None) is not None
    
    def destroy_all_user_sessions(self, user_id: int) -> int:
        """Logout user from all devices."""
        count = 0
        
        # In production, use Redis scan or maintain user:session index
        if not self.redis:
            to_delete = [
                sid for sid, data in self.sessions.items()
                if data["user_id"] == user_id
            ]
            for sid in to_delete:
                del self.sessions[sid]
                count += 1
        
        return count

# Usage example
manager = SecureSessionManager()

# Create session after successful login
session_id = manager.create_session(
    user_id=42,
    user_data={"email": "alice@example.com", "role": "user"},
    ip_address="192.168.1.100",
    user_agent="Mozilla/5.0..."
)

print(f"Session created: {session_id[:20]}...")

# Validate on each request
session = manager.validate_session(session_id, "192.168.1.100", "Mozilla/5.0...")
if session:
    print(f"Valid session for user: {session['user_data']['email']}")
else:
    print("Session invalid or expired")
```
