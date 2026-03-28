---
type: "KEY_POINT"
title: "Key Takeaways"
---

- **`typer.Typer()`** creates a multi-command application
- **`@app.command()`** decorates functions to become subcommands
- Call **`app()`** instead of `typer.run()` for applications
- **`typer.Exit(code=1)`** exits with an error code
- Function docstrings become command help text
- **Typer 0.12+:** Use `rich_markup_mode="rich"` for formatted help text