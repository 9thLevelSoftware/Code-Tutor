---
type: "THEORY"
title: "Understanding the Concept"
---

**Typer** is the modern way to build command-line interfaces (CLIs) in Python. Created by the same developer as FastAPI, it uses Python type hints to create beautiful, self-documenting CLIs with minimal code.

**Why Typer over argparse?**
- **Type hints = auto-validation** - No more manual type checking
- **Auto-generated help** - Your docstrings become `--help` text
- **Shell completion** - Tab completion for free
- **Less boilerplate** - A Typer CLI is often 70% less code than argparse
- **Rich markup in help** - Use `[bold]`, `[green]` in docstrings (0.12+)

**Installation (Typer 0.12+):**
```bash
uv add typer[all]  # Includes Rich - typer-cli is now integrated!
# or: pip install typer[all]
```

**Key insight:** If you know type hints, you already know 80% of Typer. Your function signatures *are* your CLI definitions.

**Throughout this module**, we'll build a **Personal Finance Tracker CLI** - a practical tool for managing expenses, income, and budgets from the terminal.