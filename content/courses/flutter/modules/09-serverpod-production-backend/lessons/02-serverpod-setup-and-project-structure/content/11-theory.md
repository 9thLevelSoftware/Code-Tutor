---
type: "THEORY"
title: "Hot Reload Workflow"
---


Serverpod supports hot reload during development, making iteration fast.

**Automatic Reload:**

When you modify code and save, Serverpod automatically detects changes and reloads. You do not need to restart the server for most changes.

**When Hot Reload Works:**
- Changing endpoint method logic
- Modifying existing protocol classes
- Updating middleware
- Changing configuration values

**When You Need to Restart:**
- Adding new endpoint files
- Adding new protocol files
- Changing database migrations
- Modifying main.dart entry point

**The Development Loop:**

```
1. Edit code
2. Save file (Ctrl+S)
3. Serverpod reloads automatically
4. Test your changes
5. Repeat
```

**For code generation changes:**

When you modify protocol definitions, run:

```bash
serverpod generate
```

This regenerates the client package and server code. The running server will detect the changes.

