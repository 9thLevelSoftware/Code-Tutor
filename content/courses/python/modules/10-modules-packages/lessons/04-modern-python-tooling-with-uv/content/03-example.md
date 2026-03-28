---
type: "EXAMPLE"
title: "Code Example: Creating Your First Project with uv"
---

**The modern workflow uses `uv init` to create projects.** This creates a complete project structure with pyproject.toml, README, and a starter Python file.

**Key files uv creates:**
- `pyproject.toml` - Project configuration and dependencies
- `uv.lock` - Exact versions for reproducible builds
- `.python-version` - Python version for this project
- `hello.py` - Starter Python script

```python
# Demonstration of uv project workflow
# Run these commands in your terminal!

print("="*60)
print("CREATING A NEW PROJECT WITH uv")
print("="*60)

print("""
# Step 1: Create a new project
uv init my-awesome-project
cd my-awesome-project

# What uv creates:
# my-awesome-project/
#   pyproject.toml    <- Project config and dependencies
#   README.md         <- Documentation
#   hello.py          <- Starter script
#   .python-version   <- Python version (e.g., "3.12")
""")

print("\n" + "="*60)
print("EXAMINING THE GENERATED pyproject.toml")
print("="*60)

pyproject_example = '''[project]
name = "my-awesome-project"
version = "0.1.0"
description = "Add your description here"
readme = "README.md"
requires-python = ">=3.12"
dependencies = []

[build-system]
requires = ["hatchling"]
build-backend = "hatchling.build"
'''
print(pyproject_example)

print("\n" + "="*60)
print("ADDING DEPENDENCIES")
print("="*60)

print("""
# Add a single package
uv add requests

# Add multiple packages at once
uv add fastapi pydantic httpx

# Add with version constraints
uv add "pandas>=2.0.0"
uv add "numpy>=1.24,<2.0"

# Add development dependencies (testing, linting)
uv add --dev pytest ruff mypy

# What happens when you run 'uv add':
# 1. Downloads the package (cached globally)
# 2. Updates pyproject.toml with the dependency
# 3. Updates uv.lock with exact versions
# 4. Installs into project's virtual environment
""")

print("\n" + "="*60)
print("RUNNING YOUR CODE")
print("="*60)

print("""
# The magic command: uv run
# No need to manually activate virtual environments!

# Run a Python script
uv run python hello.py

# Run a module
uv run python -m pytest

# Run an installed tool
uv run ruff check .

# Start a REPL with project dependencies
uv run python

# What 'uv run' does:
# 1. Creates virtual environment if needed
# 2. Installs dependencies if needed
# 3. Runs your command with correct Python
# All automatically, instantly!
""")
```
