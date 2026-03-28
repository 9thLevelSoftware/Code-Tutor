---
type: "KEY_POINT"
title: "Key Takeaways"
---

- **Ruff replaces 5+ tools** - Flake8, Black, isort, pyupgrade, autoflake in one 10-100x faster tool
- **Two commands to remember** - `ruff format .` (formatting) and `ruff check --fix .` (linting)
- **Configure in pyproject.toml** - All settings in one file under `[tool.ruff]`
- **Start with these rules** - `select = ["E", "F", "I", "UP", "B", "SIM"]` covers most needs
- **target-version = "py312"** - Enables Python 3.12-specific rules and upgrades
- **VS Code integration** - Install Ruff extension for real-time feedback
- **Pre-commit hooks** - Add ruff and ruff-format to catch issues before commit
- **Auto-fix is safe** - `--fix` only applies safe fixes; unsafe ones require `--unsafe-fixes`
- **Same as Black** - Ruff format is compatible with Black's style (88 chars, double quotes)