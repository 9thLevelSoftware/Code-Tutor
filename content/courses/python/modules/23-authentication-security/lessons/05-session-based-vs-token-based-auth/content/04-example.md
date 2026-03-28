---
type: "EXAMPLE"
title: "Token Implementation Comparison"
---



```python
import jwt
from datetime import datetime, timedelta, timezone
from typing import Optional, Dict, Any

class JWTTokenAuth:
    def __init__(self, secret_key: str):
        self.secret = secret_key
        self.algorithm = "HS256"
        self.access_ttl = timedelta(minutes=15)
    
    def create_token(self, user_id: int, user_data: Dict[str, Any]) -> str:
        """Create JWT token - no storage needed."""
        payload = {
            "sub": user_id,
            "email": user_data.get("email"),
            "iat": datetime.now(timezone.utc),
            "exp": datetime.now(timezone.utc) + self.access_ttl
        }
        return jwt.encode(payload, self.secret, self.algorithm)
    
    def verify_token(self, token: str) -> Optional[Dict[str, Any]]:
        """Verify token - stateless validation."""
        try:
            return jwt.decode(token, self.secret, algorithms=[self.algorithm])
        except jwt.ExpiredSignatureError:
            return None  # Token expired
        except jwt.InvalidTokenError:
            return None  # Invalid signature
    
    # Note: No destroy_token - tokens valid until expiry
    # For revocation, need a blocklist (adds state)

# Comparison:
# Session: Store session_id in cookie, lookup in Redis
# Token: Store JWT in header, validate signature only

# Request handling:
# Session: session = store.get_session(request.cookies["session_id"])
# Token: claims = auth.verify_token(request.headers["Authorization"].split()[1])
```
