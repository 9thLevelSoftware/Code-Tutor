---
type: "THEORY"
title: "Syntax Breakdown"
---

**Key patterns:**

- **`app = typer.Typer()`** - Creates an application object
- **`@app.command()`** - Registers a function as a subcommand
- **`app()`** - Runs the application (replaces `typer.run()`)
- **`typer.Exit(code=1)`** - Exit with error code

**Typer 0.12+ features:**
- **`rich_markup_mode="rich"`** - Enable Rich formatting in help text
- **`[bold]`, `[green]`** in docstrings renders as formatted text in `--help`

**Command naming:**
- By default, function name = command name
- Use `@app.command(name="custom-name")` to override (e.g., `list` to avoid shadowing built-in)

**Help customization:**
- `typer.Typer(help="Main help")` - Application description
- Function docstrings - Command descriptions (with Rich markup in 0.16+)
- `typer.Argument(help="...")` - Argument descriptions