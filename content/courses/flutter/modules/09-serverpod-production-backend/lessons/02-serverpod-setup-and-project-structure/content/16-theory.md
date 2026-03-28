---
type: "THEORY"
title: "Development Environment Best Practices"
---


**Terminal Layout:**

For productive Serverpod development, use multiple terminals:

1. **Terminal 1**: Serverpod server (`dart run bin/main.dart`)
2. **Terminal 2**: Code generation (`serverpod generate --watch`)
3. **Terminal 3**: Flutter app (`flutter run`)
4. **Terminal 4**: Git and other commands

VS Code's integrated terminal with split panes works great for this.

**File Watching:**

Run `serverpod generate --watch` in a separate terminal. It will:
- Watch for changes to protocol YAML files
- Automatically regenerate code when you save
- Show errors if your definitions are invalid

**Database Inspection:**

Connect to your local PostgreSQL to inspect data:
- Host: localhost
- Port: 5432
- Database: my_app (your project name)
- Username: postgres
- Password: postgres_password

Use pgAdmin, DBeaver, or TablePlus to browse tables and run queries.

