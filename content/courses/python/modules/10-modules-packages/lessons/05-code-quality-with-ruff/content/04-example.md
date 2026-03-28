---
type: "EXAMPLE"
title: "Code Example: Configuring Ruff in pyproject.toml"
---

**All Ruff configuration goes in pyproject.toml.** This is one of Ruff's best features - no need for separate .flake8, pyproject.toml (black), isort.cfg, etc.

**Key configuration sections:**
- `[tool.ruff]` - General settings (line length, Python version)
- `[tool.ruff.lint]` - Which rules to enable/disable
- `[tool.ruff.format]` - Formatting options

```python
print("="*60)
print("RUFF CONFIGURATION IN pyproject.toml")
print("="*60)

basic_config = '''
# ============================================
# BASIC CONFIGURATION (Good starting point)
# ============================================

[tool.ruff]
# Same as Black for consistency
line-length = 88

# Target Python version (enables version-specific rules)
target-version = "py312"

# Directories to exclude
exclude = [
    ".venv",
    "__pycache__",
    "migrations",
    "*.pyi",
]

[tool.ruff.lint]
# Rule selection - start conservative, add more over time
select = [
    "E",    # pycodestyle errors (most important)
    "F",    # pyflakes (unused imports, undefined names)
    "I",    # isort (import sorting)
    "UP",   # pyupgrade (modernize syntax)
    "B",    # flake8-bugbear (common bugs)
    "SIM",  # flake8-simplify (simplify code)
]

# Rules to ignore (if needed)
ignore = [
    "E501",  # Line too long (formatter handles this)
]

# Allow autofix for all enabled rules
fixable = ["ALL"]

[tool.ruff.format]
# Use double quotes (like Black)
quote-style = "double"

# Indent with spaces (not tabs)
indent-style = "space"

# Unix-style line endings
line-ending = "lf"
'''
print(basic_config)

print("\n" + "="*60)
print("ADVANCED CONFIGURATION")
print("="*60)

advanced_config = '''
# ============================================
# ADVANCED CONFIGURATION (Strict mode)
# ============================================

[tool.ruff]
line-length = 88
target-version = "py312"

[tool.ruff.lint]
# Enable many more rules for strict checking
select = [
    "E",     # pycodestyle errors
    "W",     # pycodestyle warnings
    "F",     # pyflakes
    "I",     # isort
    "UP",    # pyupgrade
    "B",     # flake8-bugbear
    "SIM",   # flake8-simplify
    "C4",    # flake8-comprehensions
    "DTZ",   # flake8-datetimez (timezone-aware datetime)
    "T20",   # flake8-print (no print statements)
    "RET",   # flake8-return (return statement issues)
    "PTH",   # flake8-use-pathlib (prefer pathlib over os.path)
    "ERA",   # eradicate (commented-out code)
    "PL",    # pylint rules
    "RUF",   # Ruff-specific rules
]

# Per-file rule ignores
[tool.ruff.lint.per-file-ignores]
"tests/*" = ["S101"]  # Allow assert in tests
"__init__.py" = ["F401"]  # Allow unused imports in __init__
"scripts/*" = ["T201"]  # Allow print in scripts

# isort configuration
[tool.ruff.lint.isort]
known-first-party = ["myproject"]  # Your package name
combine-as-imports = true

# pylint configuration
[tool.ruff.lint.pylint]
max-args = 5  # Maximum function arguments
'''
print(advanced_config)

print("\n" + "="*60)
print("RULE CATEGORIES EXPLAINED")
print("="*60)

print("""
Most commonly used rule categories:

E/W  - pycodestyle (PEP 8 style)
F    - pyflakes (errors like undefined names)
I    - isort (import sorting)
UP   - pyupgrade (use modern Python syntax)
B    - bugbear (likely bugs)
SIM  - simplify (code simplification)
C4   - comprehensions (list/dict/set)
PTH  - pathlib (prefer Path over os.path)
T20  - print (no print() in production)
RUF  - Ruff-specific rules

See all rules: https://docs.astral.sh/ruff/rules/
""")
```
