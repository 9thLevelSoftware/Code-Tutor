---
type: "WARNING"
title: "Common Setup Problems and Solutions"
---


**Problem: serverpod command not found**

Solution: Add Dart's global bin to your PATH:
```bash
# Check where pub cache is
dart pub global list

# Add to PATH (macOS/Linux)
export PATH="$PATH":"$HOME/.pub-cache/bin"

# Windows: Add %LOCALAPPDATA%\Pub\Cache\bin to PATH
```

**Problem: Docker containers won't start**

Solutions:
- Make sure Docker Desktop is running (check system tray/menu bar)
- Check if ports 5432 or 6379 are already in use: `lsof -i :5432`
- Try restarting Docker Desktop
- On Windows, ensure WSL 2 is properly configured

**Problem: Cannot connect to database**

Solutions:
- Verify containers are running: `docker compose ps`
- Check container logs: `docker compose logs postgres`
- Ensure you ran migrations: `dart run bin/main.dart --apply-migrations`
- Verify config/development.yaml has correct database settings

**Problem: Code generation fails**

Solutions:
- Check protocol YAML syntax (indentation matters!)
- Ensure you are in the correct directory (my_app_server)
- Run `dart pub get` to update dependencies
- Check for circular dependencies in models

**Problem: Flutter app cannot connect to server**

Solutions:
- Is the server running? Check terminal output
- Check client initialization uses correct host/port
- For emulator, use 10.0.2.2 instead of localhost (Android)
- For iOS simulator, localhost works fine

