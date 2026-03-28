---
type: "THEORY"
title: "Dart Frog: The Minimalist Approach"
---


**Philosophy**: Do one thing well. Keep it simple.

Dart Frog embraces minimalism. It gives you routing and middleware, then gets out of your way. You choose your own database library, authentication solution, and architecture patterns.

**Core Features:**

1. **File-Based Routing**: Create a file, get a route. No configuration needed.
   - `routes/users.dart` becomes `/users`
   - `routes/users/[id].dart` becomes `/users/:id`

2. **Middleware**: Simple request/response pipeline
   - Add `_middleware.dart` files to intercept requests
   - Clean, predictable execution order

3. **Hot Reload**: Fast development cycle
   - Changes reflect immediately
   - No server restart needed

4. **Minimal Dependencies**: Small footprint
   - Quick to install and deploy
   - Easy to understand the entire codebase

**What Dart Frog Does NOT Include:**
- No built-in database ORM
- No authentication system
- No real-time WebSocket support
- No code generation
- No built-in file storage
- No caching layer

This is intentional. Dart Frog believes you should choose your own tools.

