---
type: "KEY_POINT"
title: "Essential Serverpod Commands"
---


Here are the commands you will use daily:

**Project Creation:**
```bash
serverpod create <project_name>   # Create new project
```

**Code Generation:**
```bash
serverpod generate                 # Regenerate code after model/endpoint changes
serverpod generate --watch         # Continuously watch and regenerate
```

**Running the Server:**
```bash
dart run bin/main.dart             # Start server
dart run bin/main.dart --apply-migrations  # Start and apply DB migrations
```

**Docker Commands:**
```bash
docker compose up -d               # Start PostgreSQL and Redis
docker compose down                # Stop services
docker compose logs -f             # View service logs
docker compose ps                  # Check service status
```

**Database Commands:**
```bash
serverpod create-migration         # Create migration after model changes
serverpod create-repair-migration  # Fix migration issues
```

