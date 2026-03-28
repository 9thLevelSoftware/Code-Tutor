---
type: "ANALOGY"
title: "💡 The Concept: What Is a Backend?"
---


### The Restaurant Analogy

Imagine you're at a restaurant:

**Frontend** = The dining room, menu, and waitstaff
- This is what you see and interact with
- Beautiful presentation
- Easy to understand and navigate

**Backend** = The kitchen
- Hidden from customers
- Where the real work happens
- Processes orders, prepares food, manages inventory
- Follows strict recipes and procedures

When you order food (make a request), the waiter takes your order to the kitchen (sends it to the backend). The kitchen prepares it (processes the request), and the waiter brings it back to you (returns the response).

### What Does a Backend Actually Do?

A backend server is a program running on a computer (usually in a data center) that:

1. **Listens** for requests from clients (web browsers, mobile apps, etc.)
2. **Processes** those requests (validates data, performs calculations, queries databases)
3. **Responds** with data or confirmation
4. **Stores** data in databases for long-term persistence
5. **Enforces** business rules and security

### Client-Server Architecture


- **Client**: Your web browser, mobile app, or any program that makes requests
- **Server**: The backend program that handles requests and sends responses

---



```kotlin
┌─────────────┐                    ┌─────────────┐
│   Client    │  ---- Request ---> │   Server    │
│ (Frontend)  │                    │  (Backend)  │
│             │ <--- Response ---- │             │
└─────────────┘                    └─────────────┘
```
