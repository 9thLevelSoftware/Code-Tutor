---
type: "EXAMPLE"
title: "Code Example: Pre-commit Integration"
---

**Ruff works great with pre-commit hooks.** This ensures code quality before commits reach the repository.

**Pre-commit** is a tool that runs checks before each git commit. If checks fail, the commit is rejected until you fix the issues.

```python
print("="*60)
print("PRE-COMMIT INTEGRATION")
print("="*60)

print("""
# ============================================
# STEP 1: Install pre-commit
# ============================================

uv add --dev pre-commit

# ============================================
# STEP 2: Create .pre-commit-config.yaml
# ============================================
""")

pre_commit_config = '''
# .pre-commit-config.yaml
repos:
  - repo: https://github.com/astral-sh/ruff-pre-commit
    rev: v0.8.0  # Use latest version
    hooks:
      # Run the linter
      - id: ruff
        args: [--fix]  # Auto-fix issues
      # Run the formatter
      - id: ruff-format
'''
print(pre_commit_config)

print("""
# ============================================
# STEP 3: Install the Git Hook
# ============================================

uv run pre-commit install

# Output: pre-commit installed at .git/hooks/pre-commit

# ============================================
# STEP 4: How It Works
# ============================================

# Now when you try to commit:
$ git add app.py
$ git commit -m "Add feature"

ruff....................................Passed
ruff-format.............................Passed

# All checks passed - commit succeeds!

# If there are issues:
ruff....................................Failed
- hook id: ruff
- files were modified by this hook

# Ruff auto-fixed the issues!
# Just stage the changes and commit again:
$ git add -u
$ git commit -m "Add feature"

# ============================================
# RUN ON ALL FILES (one-time cleanup)
# ============================================

uv run pre-commit run --all-files

# This checks/fixes all files, not just staged ones
# Great for initial setup or periodic cleanup
""")

print("\n" + "="*60)
print("COMPLETE pyproject.toml WITH PRE-COMMIT")
print("="*60)

complete_config = '''
[project]
name = "my-project"
version = "0.1.0"
requires-python = ">=3.12"
dependencies = [
    "fastapi>=0.115.0",
]

[project.optional-dependencies]
dev = [
    "pytest>=8.0.0",
    "ruff>=0.8.0",
    "pre-commit>=4.0.0",
]

[tool.ruff]
line-length = 88
target-version = "py312"

[tool.ruff.lint]
select = ["E", "F", "I", "UP", "B", "SIM"]
fixable = ["ALL"]

[tool.ruff.format]
quote-style = "double"
'''
print(complete_config)
```
