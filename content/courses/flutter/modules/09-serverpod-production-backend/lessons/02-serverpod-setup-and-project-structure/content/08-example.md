---
type: "EXAMPLE"
title: "Docker Compose Configuration"
---


Here is what the auto-generated docker-compose.yaml contains:



```yaml
# docker-compose.yaml (in my_app_server/)
version: '3'
services:
  postgres:
    image: postgres:14
    ports:
      - '5432:5432'
    environment:
      POSTGRES_USER: postgres
      POSTGRES_PASSWORD: postgres_password
      POSTGRES_DB: my_app
    volumes:
      - postgres_data:/var/lib/postgresql/data
  
  redis:
    image: redis:6
    ports:
      - '6379:6379'
    volumes:
      - redis_data:/data

volumes:
  postgres_data:
  redis_data:

# This configuration:
# - Uses PostgreSQL 14 and Redis 6 (stable versions)
# - Exposes standard ports for easy connection
# - Persists data in Docker volumes (survives restarts)
# - Uses simple passwords (fine for local dev)
```
