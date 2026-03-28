---
type: "THEORY"
title: "Migration Path: Dart Frog to Serverpod"
---


Starting with Dart Frog does not lock you in forever. Here is a migration strategy:

**Phase 1: Parallel Development**
1. Create a new Serverpod project alongside your Dart Frog API
2. Define your data models in Serverpod
3. Migrate data to PostgreSQL

**Phase 2: Endpoint Migration**
1. Recreate endpoints in Serverpod one by one
2. Update Flutter client to use generated Serverpod client
3. Run both APIs in parallel during transition

**Phase 3: Cutover**
1. Route all traffic to Serverpod
2. Deprecate Dart Frog endpoints
3. Remove old code after confidence period

**Key Insight**: Your business logic (validation, calculations, rules) transfers directly. Only the framework integration code changes. This is why understanding both frameworks is valuable.

