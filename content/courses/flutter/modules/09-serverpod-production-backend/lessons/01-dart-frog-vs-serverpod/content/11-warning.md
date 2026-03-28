---
type: "WARNING"
title: "Common Decision Mistakes"
---


**Mistake 1: Starting with Serverpod for a simple API**

If you just need 3 endpoints and no auth, Serverpod's setup overhead is not worth it. You will spend more time configuring Docker and PostgreSQL than writing code.

**Mistake 2: Starting with Dart Frog for a complex app**

If you know you need auth, real-time, file storage, and complex queries, you will eventually rebuild everything Serverpod provides. Start with Serverpod and save months of work.

**Mistake 3: Choosing based on current needs only**

Ask yourself: "What will this app need in 6 months?" If the answer includes features Serverpod provides, consider starting with Serverpod even if you do not need everything today.

**Mistake 4: Ignoring infrastructure requirements**

Serverpod requires Docker and PostgreSQL. If you cannot run these (shared hosting, serverless-only), Dart Frog is your only option.

**Mistake 5: Underestimating code generation value**

Manually maintaining API clients, keeping documentation in sync, and debugging serialization bugs is tedious. Serverpod's code generation eliminates entire categories of bugs.

