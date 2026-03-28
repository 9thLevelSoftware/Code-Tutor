---
type: "WARNING"
title: "Password Storage Anti-Patterns"
---

**Never store plain-text passwords**, even temporarily or in logs. If your database is compromised, attackers gain access to all user accounts on your site—and likely others, since users reuse passwords.

**MD5 and SHA-1 are not suitable for passwords**:
```kotlin
// WRONG - Fast hashing algorithms are vulnerable to brute force
val hash = MessageDigest.getInstance("MD5")
    .digest(password.toByteArray())
```

**Use bcrypt, Argon2, or PBKDF2** with high work factors:
```kotlin
// Correct - bcrypt with cost factor 12
val hashedPassword = BCrypt.hashpw(password, BCrypt.gensalt(12))
```

**Never implement your own crypto**—use established libraries like bcrypt, jBCrypt, or Spring Security's password encoders. Home-grown crypto is almost always vulnerable.

**Common mistakes:**
- Using fast hash functions (MD5, SHA-256) without salts
- Using the same salt for all passwords
- Low work factor/iteration counts (makes brute force faster)
- Logging passwords or password hashes
- Transmitting passwords over HTTP (always use HTTPS)

Password security is critical—get it wrong and you're liable for user account compromises across the internet.
