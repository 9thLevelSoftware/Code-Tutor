---
type: "ANALOGY"
title: "Syntax Breakdown: Configuration Management"
---

**Managing configuration:**

**1. Environment variables (.env file):**
```bash
# .env (NEVER commit this!)
DATABASE_URL=postgresql://user:pass@localhost/db
SECRET_KEY=your-secret-key-here
DEBUG=True
```

**2. Config file (config.py):**
```python
import os
from dotenv import load_dotenv

load_dotenv()  # Load .env file

class Config:
    SECRET_KEY = os.getenv('SECRET_KEY')
    DATABASE_URL = os.getenv('DATABASE_URL')
    DEBUG = os.getenv('DEBUG', 'False') == 'True'
    
class DevelopmentConfig(Config):
    DEBUG = True
    DATABASE_URL = 'sqlite:///dev.db'
    
class ProductionConfig(Config):
    DEBUG = False
    # Production database from environment
    
config = {
    'development': DevelopmentConfig,
    'production': ProductionConfig,
    'default': DevelopmentConfig
}
```

**3. pyproject.toml:**
```toml
[project]
name = "my_project"
version = "0.1.0"
dependencies = [
    "Flask>=3.0.0",
    "SQLAlchemy>=2.0.0",
    "python-dotenv>=1.0.0"
]

[project.optional-dependencies]
test = [
    "pytest>=7.4.0"
]
```

**Install dependencies:**
```bash
pip install .
```

**4. .gitignore:**
```
# Environment
.env
venv/
__pycache__/

# Database
*.db
*.sqlite

# IDE
.vscode/
.idea/

# OS
.DS_Store
```