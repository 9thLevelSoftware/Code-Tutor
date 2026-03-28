---
type: "THEORY"
title: "Serverpod: The Batteries-Included Approach"
---


**Philosophy**: Everything you need for production, out of the box.

Serverpod provides a complete backend solution. It includes an ORM, authentication, real-time communication, file storage, caching, logging, and automatic client code generation. You trade some flexibility for massive productivity gains.

**Core Features:**

1. **Built-in ORM**: Type-safe database operations
   - Define models in YAML, get Dart classes automatically
   - Migrations, relations, transactions included
   - PostgreSQL-first design

2. **Code Generation**: Automatic client libraries
   - Define an endpoint once, call it from Flutter with full type safety
   - No manual API client code needed
   - Serialization handled automatically

3. **Authentication**: Complete auth system
   - Email/password, Google, Apple, Firebase Auth
   - Session management included
   - Secure by default

4. **Real-time**: WebSocket streams built-in
   - Broadcast messages to clients
   - Subscribe to data changes
   - Chat, notifications, live updates

5. **File Storage**: Upload and serve files
   - S3-compatible storage
   - Cloud or local development
   - Automatic URL generation

6. **Caching**: Redis integration
   - Cache expensive operations
   - Session storage
   - Rate limiting

7. **Logging and Monitoring**: Production-ready observability
   - Structured logging
   - Error tracking
   - Performance metrics

