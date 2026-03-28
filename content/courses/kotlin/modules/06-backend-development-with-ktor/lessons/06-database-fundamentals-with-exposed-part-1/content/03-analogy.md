---
type: "ANALOGY"
title: "💡 The Concept: Why Databases?"
---


### The Filing Cabinet Analogy

**In-Memory Storage** = Writing notes on sticky notes and leaving them on your desk
- ❌ Disappears when you clean your desk (restart server)
- ❌ Can't handle millions of notes (runs out of RAM)
- ❌ Lost forever if the desk catches fire (server crash)

**Database Storage** = Filing cabinet with organized folders
- ✅ Survives desk cleaning (persists across restarts)
- ✅ Can store millions of documents (scales beyond RAM)
- ✅ Can be backed up (disaster recovery)
- ✅ Multiple people can access simultaneously (concurrent access)

### What Is a Database?

A **database** is software designed specifically for storing, organizing, and retrieving data efficiently.

**Types of databases:**
1. **Relational (SQL)**: PostgreSQL, MySQL, SQLite, H2
   - Data stored in tables with rows and columns
   - Relationships between tables
   - Strong consistency guarantees

2. **NoSQL**: MongoDB, Redis, Cassandra
   - Various data models (documents, key-value, etc.)
   - Often more flexible but less structured

For this course, we'll use **H2** (a lightweight SQL database perfect for learning).

---

