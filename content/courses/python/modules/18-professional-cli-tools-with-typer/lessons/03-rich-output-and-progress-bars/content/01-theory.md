---
type: "THEORY"
title: "Understanding the Concept"
---

**Rich** is a Python library for beautiful terminal output. When you install `typer[all]`, Rich is included - no separate install needed!

**Rich gives you:**
- **Console** - Central object for all Rich output
- **Tables** - Display data in formatted columns
- **Progress bars** - Show operation progress
- **Panels and trees** - Organize complex output
- **Colored text** - Make important info stand out

**Typer 0.12+ integration:**
- Use `rich.console.Console` for direct Rich output
- Use `rich.table.Table` for beautiful data display
- Rich markup works in docstrings with `rich_markup_mode="rich"`