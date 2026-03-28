---
type: "EXAMPLE"
title: "Code Breakdown"
---


### The Registration Flow


### Security Highlights

**1. Password Never Stored in Plaintext**:

**2. Password Hash Never Exposed**:

**3. Separate Method for Password Retrieval**:

**4. Email Case-Insensitivity**:

---



```kotlin
// "Alice@Example.COM" and "alice@example.com" are the same user
it[Users.email] = email.lowercase().trim()
```
