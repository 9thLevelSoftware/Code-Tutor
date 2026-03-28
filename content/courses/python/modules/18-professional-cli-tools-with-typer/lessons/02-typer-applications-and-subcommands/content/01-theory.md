---
type: "THEORY"
title: "Understanding the Concept"
---

Real-world CLIs have multiple commands. Think of `git`: you run `git commit`, `git push`, `git log` - these are **subcommands** under the main `git` command.

Typer makes this easy with `typer.Typer()` - an application object that groups commands together.

**When to use subcommands:**
- Your tool does multiple distinct things
- You want organized, discoverable commands
- You're building something like: `finance add`, `finance list`, `finance summary`

**Typer 0.12+ Feature:** Use `rich_markup_mode="rich"` to enable Rich formatting in help text!