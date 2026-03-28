---
type: "KEY_POINT"
title: "Key Patterns Used"
---

- **`Enum` for choices** - `Category(str, Enum)` gives autocomplete for categories
- **`Path.home()`** - Store data in user's home directory
- **JSON persistence** - Simple file-based storage
- **Rich Console + Table** - Professional output formatting
- **`rich_markup_mode="rich"`** - Enable Rich formatting in help (Typer 0.12+)
- **Exit codes** - `typer.Exit(code=1)` for errors
- **`uv add typer[all]`** - Includes Rich, no separate install needed