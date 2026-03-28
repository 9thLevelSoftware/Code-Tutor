---
type: "THEORY"
title: "What's Next?"
---

In **Lesson 10.5: API Contract Testing**, you'll learn how to ensure your Serverpod endpoints maintain consistent contracts with Flutter clients. API contracts define the expected request and response formats, enabling both frontend and backend teams to work independently while maintaining compatibility.

You'll explore tools like Pact for consumer-driven contract testing, where the Flutter client defines expectations that the Serverpod backend must fulfill. This approach catches breaking changes early in the development cycle, before they reach production.

Key topics include defining contract tests for your endpoints, validating JSON schemas, testing endpoint versions for backward compatibility, and integrating contract tests into your CI/CD pipeline. By implementing API contract testing, you'll create a safety net that prevents integration failures and reduces debugging time when deploying backend changes.

Contract testing complements your endpoint tests by focusing on the integration boundary rather than internal implementation. This ensures that even as your Serverpod backend evolves, your Flutter app continues to receive the data structures it expects.
