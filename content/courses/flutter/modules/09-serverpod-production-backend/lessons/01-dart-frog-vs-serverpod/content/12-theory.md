---
type: "THEORY"
title: "The Decision Framework"
---


Ask yourself these questions:

**Question 1: How complex is your data model?**
- Simple key-value or flat data -> Dart Frog
- Relational data with joins and transactions -> Serverpod

**Question 2: Do you need authentication?**
- No auth or simple API keys -> Dart Frog
- User accounts, sessions, OAuth -> Serverpod

**Question 3: Do you need real-time updates?**
- Request-response only -> Either works
- WebSockets, streams, live data -> Serverpod

**Question 4: How important is type safety?**
- Acceptable to have runtime API errors -> Dart Frog
- Need compile-time guarantees -> Serverpod

**Question 5: What is your infrastructure?**
- Serverless, edge, shared hosting -> Dart Frog
- VPS, containers, managed databases -> Either works

**Question 6: Team size and longevity?**
- Solo developer, short project -> Dart Frog
- Team, long-term maintenance -> Serverpod

**Scoring:**
- Mostly first options: Choose Dart Frog
- Mostly second options: Choose Serverpod
- Mixed: Consider starting with Dart Frog, plan migration path

