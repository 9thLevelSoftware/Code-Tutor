---
type: "THEORY"
title: "Testing Serverpod Endpoints"
---

Serverpod is a comprehensive backend framework for Dart that provides powerful testing utilities to ensure your API endpoints work correctly. Unlike simple HTTP endpoint testing, Serverpod tests run against the actual framework, enabling you to test the full request lifecycle including authentication, database interactions, and business logic.

The foundation of Serverpod testing is the `TestSession` class, which creates a simulated session environment for your endpoints. This session mimics real user interactions, allowing you to test authenticated endpoints, database operations, and streaming responses without deploying to a production server. By using TestSession, your tests execute within the same runtime as your server code, providing fast feedback and access to internal APIs.

Integration testing is where Serverpod truly shines. Rather than mocking database calls, you test against a real PostgreSQL database configured specifically for testing. This approach catches ORM mapping issues, query errors, and transaction problems that mocks would silently ignore. Serverpod's test utilities handle database setup, migration, and cleanup automatically between test runs.

For authenticated endpoints, Serverpod allows you to create sessions with predefined user contexts. You can test role-based access control by creating sessions for different user types—administrators, regular users, or anonymous visitors—and verifying that each receives appropriate responses. This eliminates the complexity of managing JWT tokens or session cookies in your test code.

Streaming endpoints require special testing approaches. Serverpod provides utilities to capture and verify streamed data chunks, test connection lifecycle events, and validate WebSocket behavior. These tests ensure your real-time features work correctly under various network conditions and client behaviors.

A complete Serverpod testing strategy combines unit tests for business logic with integration tests for endpoints. This layered approach provides confidence that your backend is production-ready, catching issues early in development before they reach staging or production environments.