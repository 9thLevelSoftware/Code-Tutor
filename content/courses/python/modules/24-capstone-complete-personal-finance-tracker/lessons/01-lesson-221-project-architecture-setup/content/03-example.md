---
type: "EXAMPLE"
title: "Project Initialization"
---

Initialize the project with uv and configure dependencies:

**Expected pyproject.toml structure:**

```toml
# Create project with uv (Python 3.12+)
# Run: uv init finance_tracker
# cd finance_tracker
# uv add fastapi uvicorn asyncpg pydantic pydantic-settings python-jose passlib bcrypt
# uv add --dev pytest pytest-asyncio httpx

# pyproject.toml
[project]
name = "finance-tracker"
version = "1.0.0"
description = "Personal Finance Tracker - Python Capstone Project"
requires-python = ">=3.12"
dependencies = [
    "fastapi>=0.115.0",
    "uvicorn[standard]>=0.32.0",
    "asyncpg>=0.30.0",
    "pydantic>=2.10.0",
    "pydantic-settings>=2.6.0",
    "python-jose[cryptography]>=3.3.0",
    "passlib[bcrypt]>=1.7.4",
    "alembic>=1.14.0",
]

[project.optional-dependencies]
dev = [
    "pytest>=8.3.0",
    "pytest-asyncio>=0.24.0",
    "httpx>=0.28.0",
    "ruff>=0.8.0",
    "mypy>=1.13.0",
]

[tool.pytest.ini_options]
asyncio_mode = "auto"
asyncio_default_fixture_loop_scope = "function"

[tool.ruff]
line-length = 100
target-version = "py312"

[tool.mypy]
python_version = "3.12"
strict = true
warn_return_any = true
```
