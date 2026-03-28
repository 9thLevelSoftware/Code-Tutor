---
type: "WARNING"
title: "Common Pitfalls"
---

### Watch Out For These Common Mistakes:

**1. Using Black AND Ruff Format Together**
```bash
# WRONG - Redundant, may conflict
uv add --dev black ruff
# Running both formatters
black . && ruff format .  # Unnecessary!

# CORRECT - Use only Ruff (it includes Black's formatting)
uv add --dev ruff
ruff format .
```

**2. Ignoring All Rules in a Category**
```toml
# WRONG - Disabling too much
[tool.ruff.lint]
ignore = ["E", "W"]  # Ignores ALL style rules!

# CORRECT - Ignore specific rules only
[tool.ruff.lint]
ignore = ["E501"]  # Just line length
```

**3. Not Running Format AND Check**
```bash
# WRONG - Only formatting
ruff format .
# Code has style errors!

# CORRECT - Format first, then lint
ruff format .
ruff check --fix .
```

**4. Using Unsafe Fixes Without Review**
```bash
# DANGEROUS - May break code
ruff check --fix --unsafe-fixes .
# Auto-applied without review!

# SAFE - Review unsafe fixes first
ruff check .  # See all issues
ruff check --fix .  # Apply safe fixes only
# Manually review and fix unsafe issues
```

**5. Forgetting to Configure target-version**
```toml
# WRONG - Defaults to old Python
[tool.ruff]
line-length = 88
# Missing target-version!

# CORRECT - Specify your Python version
[tool.ruff]
line-length = 88
target-version = "py312"  # Enables Python 3.12 rules
```

**6. Not Updating Ruff Regularly**
```bash
# WRONG - Using old version with missing rules
# ruff 0.1.x from a year ago

# CORRECT - Keep Ruff updated
uv add --dev ruff@latest
# Or in pre-commit, update rev to latest version
```