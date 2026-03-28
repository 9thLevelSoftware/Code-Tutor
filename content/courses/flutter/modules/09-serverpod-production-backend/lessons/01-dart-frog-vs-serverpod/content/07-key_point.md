---
type: "KEY_POINT"
title: "The Code Generation Advantage"
---


Serverpod's code generation is its killer feature. When you define an endpoint method on the server, Serverpod automatically creates:

1. **Server-side method** with proper routing
2. **Client-side method** with matching signature
3. **Serialization code** for all parameters and return types
4. **Type definitions** shared between server and client

This means:
- **No API documentation drift** - client and server always match
- **Compile-time errors** instead of runtime errors when APIs change
- **IntelliSense/autocomplete** for all API calls
- **Refactoring safety** - rename a method, and both sides update

With Dart Frog, you manually maintain API clients, JSON serialization, and documentation. With Serverpod, it is all automatic.

