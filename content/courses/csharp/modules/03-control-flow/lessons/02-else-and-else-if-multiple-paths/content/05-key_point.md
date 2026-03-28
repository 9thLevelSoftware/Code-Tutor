---
type: "KEY_POINT"
title: "Branching with Else and Else-If"
---

## Key Takeaways

- **else-if chains are checked top to bottom** — once a condition is true, its block executes and all remaining conditions are skipped. Order your conditions from most specific to least specific.

- **Use guard clauses to reduce nesting** — instead of deeply nested if-else, return early for invalid cases: `if (user is null) return;`. The remaining code handles the valid path without indentation.

- **The final `else` is your safety net** — it catches everything not matched above. Include it when you need to handle "none of the above" cases, like a default error message.

- **Only ONE block executes** — Even if multiple conditions would be true, only the first matching one runs. This is different from using multiple separate `if` statements.

- **Order matters!** — Put your most specific conditions first. `if (score >= 90)` should come before `if (score >= 80)`.

- **When to use else if vs switch:**
  - Use `if-else if` for checking ranges (`score >= 80`) or complex conditions
  - Use `switch` when checking exact values against a single variable
