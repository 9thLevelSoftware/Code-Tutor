---
type: "THEORY"
title: "When to Choose Dart Frog"
---


**Choose Dart Frog when:**

1. **Building a prototype or MVP**
   - You need something working in hours, not days
   - Requirements are not yet clear
   - You might throw it away and rebuild

2. **Simple APIs with minimal backend logic**
   - Proxy APIs that aggregate other services
   - Webhooks receivers
   - Simple CRUD without complex relations

3. **You have specific technology preferences**
   - You want MongoDB instead of PostgreSQL
   - You prefer a specific auth library
   - You need integration with legacy systems

4. **Learning backend development**
   - Understanding fundamentals before using abstractions
   - Seeing exactly what happens in each request
   - Building mental models of HTTP, routing, middleware

5. **Microservices that do one thing**
   - Image processing service
   - Notification dispatcher
   - Webhook handler

6. **Serverless deployment**
   - Deploying to edge functions
   - Minimal cold start time matters
   - Pay-per-request pricing

**Real Example**: A startup building an MVP to test market fit. They need a simple API for their Flutter app to save user preferences and fetch content. Dart Frog lets them ship in a weekend.

