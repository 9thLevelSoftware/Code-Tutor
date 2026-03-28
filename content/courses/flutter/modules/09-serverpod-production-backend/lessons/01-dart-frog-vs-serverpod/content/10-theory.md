---
type: "THEORY"
title: "When to Choose Serverpod"
---


**Choose Serverpod when:**

1. **Building a production application**
   - You know you will need auth, database, real-time eventually
   - Long-term maintainability matters
   - Multiple developers will work on the codebase

2. **You need real-time features**
   - Chat applications
   - Live notifications
   - Collaborative editing
   - Live dashboards

3. **Complex data relationships**
   - Users, posts, comments, likes (social apps)
   - Orders, products, inventory (e-commerce)
   - Nested entities with transactions

4. **Team productivity is critical**
   - Type-safe APIs prevent bugs
   - Generated code reduces boilerplate
   - Consistent patterns across the codebase

5. **You value the Dart ecosystem**
   - Same language on frontend and backend
   - Shared models between client and server
   - Single team can handle full stack

6. **Enterprise or funded startups**
   - Can handle Docker/PostgreSQL infrastructure
   - Need production-grade logging and monitoring
   - Security and authentication are requirements

**Real Example**: A funded startup building a social fitness app. They need user accounts, post feeds, real-time chat, image uploads, push notifications. Serverpod provides all of this out of the box.

