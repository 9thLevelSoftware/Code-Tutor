---
type: "EXAMPLE"
title: "Code Example: The uv.lock File and Reproducible Builds"
---

**The uv.lock file ensures everyone gets exactly the same dependencies.** Unlike requirements.txt, it captures the complete dependency tree with cryptographic hashes.

**Why this matters:**
- Works on your machine AND production
- No "it works on my laptop" bugs
- Security: verifies package integrity
- Speed: uv knows exactly what to install

```python
print("="*60)
print("UNDERSTANDING uv.lock")
print("="*60)

print("""
# ============================================
# WHAT uv.lock CONTAINS
# ============================================

# When you run 'uv add requests', uv.lock records:
# - Exact version installed (e.g., requests 2.32.3)
# - All transitive dependencies (urllib3, certifi, etc.)
# - SHA256 hashes for each package
# - Python version compatibility

# Example uv.lock snippet (don't edit manually!):
"""

lock_example = '''version = 1
requires-python = ">=3.12"

[[package]]
name = "requests"
version = "2.32.3"
source = { registry = "https://pypi.org/simple" }
dependencies = [
    { name = "certifi" },
    { name = "charset-normalizer" },
    { name = "idna" },
    { name = "urllib3" },
]
wheels = [
    { url = "https://files.pythonhosted.org/...", hash = "sha256:..." },
]
'''
print(lock_example)

print("""
# ============================================
# KEY COMMANDS FOR REPRODUCIBILITY
# ============================================

# Update the lock file (after manual pyproject.toml edits)
uv lock

# Install exactly what's in uv.lock (CI/CD, production)
uv sync

# Update all dependencies to latest compatible versions
uv lock --upgrade

# Update a specific package
uv lock --upgrade-package requests

# ============================================
# WORKFLOW FOR TEAMS
# ============================================

# Developer 1 adds a package:
uv add httpx
git add pyproject.toml uv.lock
git commit -m "Add httpx for async HTTP"
git push

# Developer 2 pulls and syncs:
git pull
uv sync  # Gets EXACTLY the same versions!

# CI/CD pipeline:
git clone repo
uv sync  # Reproducible every time!
uv run pytest
""")
```
