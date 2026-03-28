---
type: "THEORY"
title: "Project Overview"
---

Let's build a complete **Personal Finance Tracker CLI** that combines everything we've learned:

**Features:**
- Add expenses and income with categories
- List transactions in a Rich table
- Show summary with totals
- Persistent storage with JSON file
- Category-based filtering
- Beautiful Rich output with Console and Table

**Project structure:**
```
finance/
  __init__.py
  cli.py       # Typer commands
  storage.py   # JSON file handling
  models.py    # Transaction dataclass
```

**Typer 0.12+ features used:**
- `rich_markup_mode="rich"` for formatted help
- Rich Console and Table integration
- typer[all] includes everything needed