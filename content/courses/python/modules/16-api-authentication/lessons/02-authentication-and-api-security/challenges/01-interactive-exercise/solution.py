from fastapi import FastAPI, Depends, HTTPException, status
from fastapi.security import OAuth2PasswordBearer, OAuth2PasswordRequestForm
from pydantic import BaseModel, EmailStr, Field
from jose import JWTError, jwt
import hashlib
import os
from datetime import datetime, timedelta
from typing import Optional

app = FastAPI()
SECRET_KEY = os.getenv("SECRET_KEY", "development-fallback-secret")
ALGORITHM = 'HS256'
ACCESS_TOKEN_EXPIRE_MINUTES = 30

oauth2_scheme = OAuth2PasswordBearer(tokenUrl='token')

users = {}  # email -> user_data

# Pydantic models
class UserCreate(BaseModel):
    email: EmailStr
    password: str = Field(..., min_length=8)

class User(BaseModel):
    id: int
    email: EmailStr

class Token(BaseModel):
    access_token: str
    token_type: str

def hash_password(password: str) -> bytes:
    """Hash password with salt."""
    salt = os.urandom(32)
    pwdhash = hashlib.pbkdf2_hmac('sha256', password.encode(), salt, 100000)
    return salt + pwdhash

def verify_password(stored_hash: bytes, password: str) -> bool:
    """Verify password against stored hash."""
    salt = stored_hash[:32]
    stored_pw = stored_hash[32:]
    test_hash = hashlib.pbkdf2_hmac('sha256', password.encode(), salt, 100000)
    return test_hash == stored_pw

def create_access_token(data: dict) -> str:
    """Create JWT access token."""
    to_encode = data.copy()
    expire = datetime.utcnow() + timedelta(minutes=ACCESS_TOKEN_EXPIRE_MINUTES)
    to_encode.update({'exp': expire})
    return jwt.encode(to_encode, SECRET_KEY, algorithm=ALGORITHM)

async def get_current_user(token: str = Depends(oauth2_scheme)) -> User:
    """Dependency to get current user from token."""
    credentials_exception = HTTPException(
        status_code=status.HTTP_401_UNAUTHORIZED,
        detail='Could not validate credentials',
        headers={'WWW-Authenticate': 'Bearer'},
    )
    try:
        payload = jwt.decode(token, SECRET_KEY, algorithms=[ALGORITHM])
        email: str = payload.get('sub')
        if email is None:
            raise credentials_exception
    except JWTError:
        raise credentials_exception
    
    user = users.get(email)
    if user is None:
        raise credentials_exception
    return User(id=user['id'], email=user['email'])

@app.post('/api/register', response_model=User, status_code=201)
async def register(user_data: UserCreate):
    """Register new user with validated email and password."""
    email = user_data.email.lower()
    if email in users:
        raise HTTPException(status_code=400, detail='User already exists')
    
    users[email] = {
        'id': len(users) + 1,
        'email': email,
        'password_hash': hash_password(user_data.password)
    }
    return User(id=users[email]['id'], email=email)

@app.post('/token', response_model=Token)
async def login(form_data: OAuth2PasswordRequestForm = Depends()):
    """OAuth2 login - returns JWT token."""
    user = users.get(form_data.username.lower())
    if not user or not verify_password(user['password_hash'], form_data.password):
        raise HTTPException(
            status_code=status.HTTP_401_UNAUTHORIZED,
            detail='Incorrect email or password',
            headers={'WWW-Authenticate': 'Bearer'},
        )
    
    access_token = create_access_token(data={'sub': user['email']})
    return {'access_token': access_token, 'token_type': 'bearer'}

@app.get('/api/profile', response_model=User)
async def get_profile(current_user: User = Depends(get_current_user)):
    """Get current user profile (requires token)."""
    return current_user

if __name__ == '__main__':
    print('FastAPI OAuth2 Authentication')
    print('Run with: uvicorn main:app --reload')
    print('Swagger UI: http://localhost:8000/docs')