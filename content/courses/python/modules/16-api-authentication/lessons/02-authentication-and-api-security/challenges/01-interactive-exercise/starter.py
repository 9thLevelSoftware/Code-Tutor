from fastapi import FastAPI, Depends, HTTPException, status
from fastapi.security import OAuth2PasswordBearer, OAuth2PasswordRequestForm
from pydantic import BaseModel, EmailStr, Field
from jose import jwt
import hashlib
import os
from datetime import datetime, timedelta
from typing import Optional

app = FastAPI()
SECRET_KEY = os.getenv("SECRET_KEY", "development-fallback-secret")
ALGORITHM = 'HS256'

# OAuth2 scheme
oauth2_scheme = OAuth2PasswordBearer(tokenUrl='token')

users = {}  # In-memory user storage

# TODO: Create Pydantic models for UserCreate and User

# TODO: Implement password hashing
def hash_password(password: str) -> bytes:
    pass

# TODO: Implement password verification
def verify_password(stored_hash: bytes, password: str) -> bool:
    pass

# TODO: Implement JWT token creation
def create_access_token(data: dict) -> str:
    pass

# TODO: Implement get_current_user dependency

# TODO: Implement POST /api/register endpoint

# TODO: Implement POST /token endpoint (OAuth2)

# TODO: Implement GET /api/profile endpoint

# Run with: uvicorn main:app --reload