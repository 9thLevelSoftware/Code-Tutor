---
type: "THEORY"
title: "Why Consider Supabase?"
---


### Avoiding Vendor Lock-In

In Module 8 Lesson 1, we introduced Firebase as our primary backend. It's excellent for rapid development. However, **senior developers should understand alternatives** for these reasons:

1. **Cost at Scale**: Firebase pricing can surprise you as you grow
2. **Data Ownership**: Your data lives on Google's servers
3. **Flexibility**: Sometimes you need raw SQL power
4. **Self-Hosting**: Some projects require on-premise deployment
5. **Open Source**: Community-driven development and transparency

### What is Supabase?

**Supabase = Open Source Firebase Alternative**

| Feature | Firebase | Supabase |
|---------|----------|----------|
| Database | Firestore (NoSQL) | PostgreSQL (SQL) |
| Auth | Firebase Auth | GoTrue (compatible) |
| Storage | Cloud Storage | S3-compatible |
| Real-time | Firestore listeners | Postgres Changes |
| Self-host | No | Yes |
| Open Source | No | Yes |
| Pricing | Pay per operation | Pay per resource |

**When to choose Supabase:**
- You need complex SQL queries (joins, aggregations)
- You want to self-host or own your infrastructure
- You prefer open source solutions
- You're coming from a SQL background

**When to stick with Firebase:**
- Rapid prototyping (slightly faster setup)
- Deep Google ecosystem integration
- Offline-first with automatic sync

