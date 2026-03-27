---
type: "EXAMPLE"
title: "Code Example: Project Structure"
---

**Project structure breakdown:**

**1. Root level:**
- `README.md`: First thing people see
- `pyproject.toml`: Modern package and dependency management
- `.gitignore`: Exclude temp files, secrets

**2. Source code (`src/`):**
- `models/`: Data structures
- `services/`: Business logic
- `repositories/`: Database access
- `api/`: HTTP endpoints
- `utils/`: Helper functions

**3. Tests (`tests/`):**
- Mirror src/ structure
- One test file per source file
- Use pytest conventions

**4. Documentation (`docs/`):**
- Architecture decisions
- API documentation
- User guides

**Layered architecture:**
```
API → Services → Repositories → Database
```
Each layer only talks to layer below it.

```python
# Standard Python project structure
"""
my_project/
│
├── README.md              # Project overview and setup instructions
├── pyproject.toml         # Modern project and dependency config
├── .gitignore            # Files to ignore in version control
├── .env.example          # Environment variables template
│
├── docs/                 # Documentation
│   ├── architecture.md
│   ├── api.md
│   └── user_guide.md
│
├── tests/                # Test files
│   ├── __init__.py
│   ├── test_models.py
│   ├── test_services.py
│   └── test_utils.py
│
├── src/                  # Source code
│   ├── __init__.py
│   │
│   ├── models/           # Data models
│   │   ├── __init__.py
│   │   ├── user.py
│   │   └── post.py
│   │
│   ├── services/         # Business logic
│   │   ├── __init__.py
│   │   ├── auth_service.py
│   │   └── post_service.py
│   │
│   ├── repositories/     # Data access
│   │   ├── __init__.py
│   │   ├── user_repo.py
│   │   └── post_repo.py
│   │
│   ├── api/              # API endpoints
│   │   ├── __init__.py
│   │   ├── routes.py
│   │   └── middleware.py
│   │
│   ├── utils/            # Utility functions
│   │   ├── __init__.py
│   │   ├── validators.py
│   │   └── helpers.py
│   │
│   └── config.py         # Configuration
│
└── scripts/              # Utility scripts
    ├── setup_db.py
    └── seed_data.py
"""

print("=== Example: Planning a Blog Application ===")

# Step 1: Define requirements
requirements = {
    "project_name": "Simple Blog",
    "features": [
        "User registration and authentication",
        "Create, read, update, delete posts",
        "Comment on posts",
        "Search functionality",
        "User profiles"
    ],
    "tech_stack": {
        "backend": "Flask",
        "database": "SQLite/PostgreSQL",
        "authentication": "JWT",
        "testing": "pytest"
    },
    "constraints": [
        "Must be RESTful API",
        "Must handle 100 concurrent users",
        "Response time < 200ms"
    ]
}

print("\n📋 Project Requirements:")
print(f"Project: {requirements['project_name']}")
print(f"\nFeatures:")
for feature in requirements['features']:
    print(f"  - {feature}")

print(f"\nTech Stack:")
for key, value in requirements['tech_stack'].items():
    print(f"  {key}: {value}")

# Step 2: Design data models
print("\n=== Data Models ===")

class DesignDoc:
    """Documentation for data model design"""
    
    USER_MODEL = """
    User:
      - id: int (primary key)
      - username: str (unique, required)
      - email: str (unique, required)
      - password_hash: str (required)
      - created_at: datetime
      - updated_at: datetime
      
      Relationships:
        - posts: one-to-many
        - comments: one-to-many
    """
    
    POST_MODEL = """
    Post:
      - id: int (primary key)
      - title: str (required, max 200)
      - content: text (required)
      - author_id: int (foreign key -> User)
      - created_at: datetime
      - updated_at: datetime
      
      Relationships:
        - author: many-to-one (User)
        - comments: one-to-many
    """
    
    COMMENT_MODEL = """
    Comment:
      - id: int (primary key)
      - content: text (required)
      - author_id: int (foreign key -> User)
      - post_id: int (foreign key -> Post)
      - created_at: datetime
      
      Relationships:
        - author: many-to-one (User)
        - post: many-to-one (Post)
    """

print("User Model:")
print(DesignDoc.USER_MODEL)

print("\nPost Model:")
print(DesignDoc.POST_MODEL)

# Step 3: Define API endpoints
print("\n=== API Endpoints Design ===")

api_design = {
    "authentication": [
        {"method": "POST", "path": "/api/auth/register", "description": "Register new user"},
        {"method": "POST", "path": "/api/auth/login", "description": "Login user"},
        {"method": "POST", "path": "/api/auth/logout", "description": "Logout user"}
    ],
    "posts": [
        {"method": "GET", "path": "/api/posts", "description": "List all posts"},
        {"method": "GET", "path": "/api/posts/{id}", "description": "Get single post"},
        {"method": "POST", "path": "/api/posts", "description": "Create new post"},
        {"method": "PUT", "path": "/api/posts/{id}", "description": "Update post"},
        {"method": "DELETE", "path": "/api/posts/{id}", "description": "Delete post"}
    ],
    "comments": [
        {"method": "GET", "path": "/api/posts/{id}/comments", "description": "Get post comments"},
        {"method": "POST", "path": "/api/posts/{id}/comments", "description": "Add comment"},
        {"method": "DELETE", "path": "/api/comments/{id}", "description": "Delete comment"}
    ]
}

for category, endpoints in api_design.items():
    print(f"\n{category.upper()}:")
    for endpoint in endpoints:
        print(f"  {endpoint['method']:6} {endpoint['path']:30} - {endpoint['description']}")

# Step 4: Architecture diagram (as text)
print("\n=== System Architecture ===")

architecture = """
┌─────────────┐
│   Client    │  (Browser, Mobile App)
└──────┬──────┘
       │ HTTP/HTTPS
       ↓
┌─────────────────────────────────┐
│         API Layer               │
│  ┌──────────────────────────┐  │
│  │  Routes & Middleware     │  │
│  │  - Authentication        │  │
│  │  - Validation            │  │
│  │  - Error handling        │  │
│  └──────────┬───────────────┘  │
└─────────────┼───────────────────┘
              ↓
┌─────────────────────────────────┐
│      Business Logic Layer       │
│  ┌──────────────────────────┐  │
│  │  Services                │  │
│  │  - Auth Service          │  │
│  │  - Post Service          │  │
│  │  - Comment Service       │  │
│  └──────────┬───────────────┘  │
└─────────────┼───────────────────┘
              ↓
┌─────────────────────────────────┐
│      Data Access Layer          │
│  ┌──────────────────────────┐  │
│  │  Repositories            │  │
│  │  - User Repo             │  │
│  │  - Post Repo             │  │
│  │  - Comment Repo          │  │
│  └──────────┬───────────────┘  │
└─────────────┼───────────────────┘
              ↓
┌─────────────────────────────────┐
│         Database                │
│  ┌──────────────────────────┐  │
│  │  SQLite / PostgreSQL     │  │
│  │  - users table           │  │
│  │  - posts table           │  │
│  │  - comments table        │  │
│  └──────────────────────────┘  │
└─────────────────────────────────┘
"""

print(architecture)

print("\n=== Design Principles ===")

principles = [
    "Separation of Concerns: Each layer has one responsibility",
    "DRY (Don't Repeat Yourself): Reuse code through functions/classes",
    "SOLID Principles: Single responsibility, Open/closed, etc.",
    "Loose Coupling: Components don't depend tightly on each other",
    "High Cohesion: Related functionality grouped together",
    "Fail Fast: Validate early, catch errors quickly",
    "Security First: Never trust user input, validate everything"
]

for i, principle in enumerate(principles, 1):
    print(f"{i}. {principle}")
```
