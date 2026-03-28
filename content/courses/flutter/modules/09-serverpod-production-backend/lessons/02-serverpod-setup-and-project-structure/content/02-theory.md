---
type: "THEORY"
title: "Prerequisites Check"
---


Before starting, verify you have these installed:

**1. Dart SDK** (comes with Flutter)
```bash
dart --version
```
You should see Dart 3.10 or higher (included with Flutter 3.38+).

**2. Docker Desktop**
Serverpod uses Docker to run PostgreSQL and Redis locally. This is essential for development.

- **Windows**: Download from https://www.docker.com/products/docker-desktop/
- **macOS**: Download from Docker website or `brew install --cask docker`
- **Linux**: Follow Docker's official installation guide for your distribution

After installing, verify Docker is running:
```bash
docker --version
docker compose version
```

**3. PostgreSQL Client (Optional but Recommended)**
A GUI tool to inspect your database:
- **pgAdmin**: Free, cross-platform
- **DBeaver**: Free, supports many databases
- **TablePlus**: Paid with free tier, excellent UI

You can also use command-line `psql` if you prefer.

