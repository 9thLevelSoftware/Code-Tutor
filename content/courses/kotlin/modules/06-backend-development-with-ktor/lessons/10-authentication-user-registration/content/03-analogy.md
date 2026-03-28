---
type: "ANALOGY"
title: "The Concept"
---


### The Bank Vault Analogy

Think of password hashing like a bank vault combination:

**Bad Approach (Storing Plaintext Passwords)**:
- Writing the combination on a sticky note
- Anyone who sees it (hackers, rogue employees, backups) can open the vault
- If the note is stolen, every vault using that combination is compromised
- 💀 Catastrophic security failure

**Good Approach (Hashing Passwords)**:
- The combination goes through a one-way machine
- Machine outputs a unique fingerprint of the combination
- You store the fingerprint, not the combination
- To verify: run their attempt through the same machine, compare fingerprints
- Even if the fingerprint is stolen, it can't be reversed back to the combination
- ✅ Secure!

### Hashing vs Encryption: Critical Difference

| Aspect | Hashing | Encryption |
|--------|---------|------------|
| **Direction** | One-way (irreversible) | Two-way (reversible) |
| **Purpose** | Verify data without storing it | Protect data in transit/storage |
| **Can be decoded?** | ❌ No (by design!) | ✅ Yes (with key) |
| **Use for passwords?** | ✅ Always | ❌ Never |
| **Example** | bcrypt, argon2 | AES, RSA |

**Why hashing for passwords?**

If you encrypt passwords, the decryption key must exist somewhere in your system. If hackers get that key, they decrypt every password. With hashing, there's nothing to steal—the original passwords simply don't exist in your system.

### The Rainbow Table Problem

Early password systems used simple hashing (like MD5):


Hackers created "rainbow tables"—massive databases mapping common passwords to their hashes:


If your database is breached, they instantly crack every password by looking up hashes in the table.

**Solution: Salting**

A "salt" is random data added to each password before hashing:


Same password, different salts = different hashes! Rainbow tables are useless.

### Why bcrypt?

Modern password hashing needs three properties:

1. **Slow**: Takes time to compute (makes brute-force attacks impractical)
2. **Adaptive**: Can increase cost as computers get faster
3. **Salted**: Built-in random salt for each password

**bcrypt** provides all three:


As computers improve, just increase the cost factor. Your password system stays secure for years.

---



```kotlin
bcrypt(password, cost=12)
        ↓
Cost factor: 2^12 = 4,096 rounds
(Adjustable: 10=fast, 12=default, 14=very secure but slower)
```
