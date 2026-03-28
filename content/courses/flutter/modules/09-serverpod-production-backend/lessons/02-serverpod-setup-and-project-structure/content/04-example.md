---
type: "EXAMPLE"
title: "Creating Your First Serverpod Project"
---


Now for the exciting part - creating a real Serverpod project!



```bash
# Create a new Serverpod project
serverpod create my_app

# This creates a 'my_app' directory with everything you need.
# The command takes about a minute as it:
# - Creates project structure
# - Downloads dependencies
# - Generates initial code

# Navigate into your project
cd my_app

# Look at what was created
ls -la

# You should see:
# my_app_client/     - Dart package for Flutter apps to use
# my_app_flutter/    - Sample Flutter app (optional)
# my_app_server/     - The actual Serverpod server
# docker-compose.yaml - Docker configuration
```
