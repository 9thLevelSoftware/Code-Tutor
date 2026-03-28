---
type: "WARNING"
title: "Common Pitfalls"
---

### Watch Out For These Common Mistakes:

**1. Using `pip install` Instead of `uv add`**
```bash
# WRONG - Installs but doesn't track in pyproject.toml
uv pip install requests  # Not tracked!

# CORRECT - Adds to pyproject.toml and installs
uv add requests  # Tracked in pyproject.toml and uv.lock
```

**2. Running Python Directly Instead of via uv**
```bash
# WRONG - Uses system Python, wrong dependencies
python app.py  # Which Python? Which packages?

# CORRECT - Uses project's Python and dependencies
uv run python app.py  # Always correct!
```

**3. Committing .venv to Git**
```bash
# WRONG - Huge, platform-specific
git add .venv/  # 100s of MB, breaks on other platforms!

# CORRECT - Only commit config files
# .gitignore:
.venv/
__pycache__/
*.pyc

# Commit these:
git add pyproject.toml uv.lock
```

**4. Manually Editing uv.lock**
```bash
# WRONG - Manual edits break consistency
# Editing uv.lock by hand

# CORRECT - Let uv manage the lock file
uv lock  # Regenerate from pyproject.toml
uv lock --upgrade  # Update all packages
```

**5. Forgetting to Sync After Git Pull**
```bash
# WRONG - Outdated dependencies
git pull
python app.py  # Missing new dependencies!

# CORRECT - Always sync after pull
git pull
uv sync  # Install any new/updated dependencies
uv run python app.py
```

**6. Not Pinning Python Version**
```toml
# WRONG - Any Python 3.x might be used
[project]
requires-python = ">=3"

# CORRECT - Specify minimum version you've tested
[project]
requires-python = ">=3.12"
```