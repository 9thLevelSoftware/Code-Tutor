---
type: "KEY_POINT"
title: "The Three-Package Architecture"
---


Serverpod's three-package structure enables full-stack type safety:

```
+------------------+     +------------------+     +------------------+
|                  |     |                  |     |                  |
|  my_app_server   | --> |  my_app_client   | <-- |  my_app_flutter  |
|                  |     |                  |     |                  |
|  (Your backend)  |     |  (Generated)     |     |  (Your app)      |
+------------------+     +------------------+     +------------------+
        |                         ^                        |
        |                         |                        |
        +---- serverpod generate --+                       |
                                                           |
                         depends on -----------------------+
```

**The Flow:**
1. You write endpoints and models in `my_app_server`
2. Run `serverpod generate` to update `my_app_client`
3. Your Flutter app imports `my_app_client` and calls your API

**The Magic:**
- Change a method signature on the server
- Run generate
- Flutter app gets compile error if it uses the old signature
- Fix the Flutter code
- Both sides are always in sync!

