---
type: "KEY_POINT"
title: "Key Takeaways"
---

- **uv is the modern Python toolchain** - Replaces pip, venv, pyenv, poetry in one 10-100x faster tool
- **Install with one command** - `curl -LsSf https://astral.sh/uv/install.sh | sh` (Mac/Linux) or PowerShell installer (Windows)
- **`uv init project-name`** - Creates new project with pyproject.toml
- **`uv add package`** - Adds dependency to pyproject.toml and installs
- **`uv run python script.py`** - Runs with correct Python and dependencies (no activation needed!)
- **`uv sync`** - Installs exactly what's in uv.lock (reproducible builds)
- **`uv python install 3.12`** - Installs Python versions (replaces pyenv)
- **pyproject.toml** - Modern project config, replaces requirements.txt
- **uv.lock** - Ensures reproducible builds across all machines
- **Always use `uv run`** - Never worry about activating virtual environments again