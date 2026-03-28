---
type: "EXAMPLE"
title: "Installing uv"
---

**Installing uv is a one-line command.** Once installed, it manages itself - no Python required for installation!

**Note:** Python 3.12 is the current stable version. Python 3.14 is scheduled for release in October 2026.

```python
# This code demonstrates uv installation commands
# Run these in your terminal, not in Python!

print("="*60)
print("INSTALLING uv - Choose your platform:")
print("="*60)

print("""
# ============================================
# WINDOWS (PowerShell) - Recommended
# ============================================
powershell -ExecutionPolicy ByPass -c "irm https://astral.sh/uv/install.ps1 | iex"

# Or using winget:
winget install astral-sh.uv

# ============================================
# MAC/LINUX (Terminal)
# ============================================
curl -LsSf https://astral.sh/uv/install.sh | sh

# Or using Homebrew (Mac):
brew install uv

# ============================================
# VERIFY INSTALLATION
# ============================================
uv --version
# Output: uv 0.5.x (or newer)

# ============================================
# UPDATE uv (it updates itself!)
# ============================================
uv self update
""")

print("\n" + "="*60)
print("AFTER INSTALLATION - Test it works:")
print("="*60)

print("""
# Check uv version
uv --version

# See all available commands
uv help

# uv is now ready to use!
""")
```
