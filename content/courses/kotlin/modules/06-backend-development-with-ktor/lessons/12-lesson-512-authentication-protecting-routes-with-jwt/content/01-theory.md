---
type: "THEORY"
title: "Introduction"
---

**Estimated Time**: 70 minutes
**Difficulty**: Intermediate
**Prerequisites**: Lesson 5.11 (JWT tokens, login implementation)

## Introduction

Issuing JWTs is only half the battle—you must also protect your API endpoints by validating those tokens on every request. In this lesson, you'll learn how to secure routes, extract user information from tokens, and implement role-based access control in Ktor.

In this lesson, you'll learn:

1. **Token Validation**: Configuring Ktor to verify JWT signatures and claims on incoming requests
2. **Protected Routes**: Using `authenticate` blocks to guard sensitive endpoints
3. **Principal Extraction**: Accessing user information from validated tokens within route handlers
4. **Role-Based Access**: Implementing authorization checks beyond simple authentication

## Why It Matters

Not all authenticated users should have equal access. An admin can delete accounts; a regular user cannot. Proper authorization prevents privilege escalation attacks and ensures users only access resources they're entitled to. Ktor's authentication pipeline makes it easy to layer these security checks without cluttering your business logic.

## Real-World Context

Every production API uses some form of route protection. Whether it's a payment endpoint verifying a user's identity, an admin dashboard restricting access to specific roles, or a user profile ensuring people only see their own data—these patterns are universal. The authorization strategies you'll implement protect sensitive operations while maintaining clean, testable code.

Let's secure your API endpoints!
