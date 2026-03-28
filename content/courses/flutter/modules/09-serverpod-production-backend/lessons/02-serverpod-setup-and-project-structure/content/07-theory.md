---
type: "THEORY"
title: "Starting Docker Services"
---


Serverpod needs PostgreSQL (database) and Redis (caching/sessions) running. Docker makes this simple.

**Start the services:**

```bash
# From your project root directory (my_app/)
cd my_app_server
docker compose up -d
```

The `-d` flag runs containers in the background (detached mode).

**Verify services are running:**

```bash
docker compose ps
```

You should see two containers running:
- `postgres` - Database on port 5432
- `redis` - Cache on port 6379

**First-time database setup:**

The first time you run Serverpod, you need to create the database tables:

```bash
# Still in my_app_server directory
dart run bin/main.dart --apply-migrations
```

This applies all database migrations to create the required tables.

