---
type: "THEORY"
title: "Introduction"
---

**Estimated Time**: 45 minutes
**Difficulty**: Intermediate
**Prerequisites**: Lessons 5.1-5.5 (HTTP, Ktor, routing, parameters, JSON)

## Introduction

Most production APIs persist data to databases. Exposed is JetBrains' Kotlin-native SQL library that provides type-safe database operations without the complexity of ORMs. In this lesson, you'll set up Exposed, define table schemas, and execute your first queries—all with the elegance of Kotlin DSLs.

In this lesson, you'll learn:

1. **Exposed Setup**: Adding dependencies and configuring database connections
2. **Table Definitions**: Creating type-safe table objects with columns and constraints
3. **Basic Queries**: Inserting and selecting data using Exposed's fluent DSL
4. **Database Connections**: Managing connection pools for production performance

## Why It Matters

Type-safe SQL prevents runtime errors from typos in table names or column types. Exposed's DSL feels natural to Kotlin developers while generating efficient SQL. Understanding database fundamentals—connections, transactions, indexing—is essential for any backend developer building data-driven applications.

## Real-World Context

Exposed powers database layers at JetBrains and in many Kotlin backends. Whether you're building a small service or a high-throughput API, proper database setup is critical. The patterns you'll learn—connection pooling, schema migrations, query optimization—translate to any SQL database in production use.

Let's connect to a database!

