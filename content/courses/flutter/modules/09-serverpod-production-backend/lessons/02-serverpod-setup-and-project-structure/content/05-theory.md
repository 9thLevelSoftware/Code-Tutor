---
type: "THEORY"
title: "Understanding the Project Structure"
---


Serverpod creates three interconnected packages. Understanding this structure is crucial:

**1. my_app_server/** - The Backend Server

This is where your server code lives:

```
my_app_server/
  lib/
    src/
      endpoints/       # Your API endpoints go here
      protocol/        # Data models (YAML definitions)
      generated/       # Auto-generated code (DO NOT EDIT)
    server.dart        # Server entry point
  config/
    development.yaml   # Dev environment settings
    production.yaml    # Production settings
  migrations/          # Database migration files
```

**2. my_app_client/** - The Generated Client

This package is automatically generated from your server code:

```
my_app_client/
  lib/
    src/
      protocol/        # Generated data classes
    my_app_client.dart # Client class to call your API
```

Your Flutter app imports this package to communicate with the server. You never edit files here - they are regenerated when you run `serverpod generate`.

**3. my_app_flutter/** - Sample Flutter App

A starter Flutter app that demonstrates how to use the client:

```
my_app_flutter/
  lib/
    main.dart          # App entry point with client setup
  pubspec.yaml         # Depends on my_app_client
```

This is optional - you can use the client package in any Flutter project.

