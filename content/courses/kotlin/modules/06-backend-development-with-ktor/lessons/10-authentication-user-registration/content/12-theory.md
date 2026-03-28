---
type: "THEORY"
title: "Quiz Answers"
---


**Question 1: B) Hashing is one-way (irreversible), encryption is two-way (reversible)**

This is the fundamental difference:
- **Hashing**: password → hash (no reverse operation possible)
- **Encryption**: password → encrypted → decrypt → password

For passwords, you want one-way hashing so even you can't retrieve the original password.

---

**Question 2: A) Random data added to each password before hashing**

Salt prevents rainbow table attacks:


bcrypt generates and stores the salt automatically in the hash output.

---

**Question 3: C) Attackers can use them for offline brute-force attacks**

If an attacker gets the hash, they can:
1. Try millions of passwords offline
2. Hash each attempt with bcrypt
3. Compare to the stolen hash
4. Eventually crack weak passwords

This is why strong passwords and high cost factors matter—they make this attack impractically slow.

---

**Question 4: C) 12 (secure, recommended default)**

Cost factor guidelines:
- **10**: Fast but less secure, ok for low-security applications
- **12**: Recommended default (takes ~250-350ms per hash)
- **14**: Very secure but slower (~1-1.5s per hash)
- **16+**: Overkill for most applications, may hurt UX

Cost=12 balances security with user experience.

---

**Question 5: B) It saves CPU cycles (hashing is expensive, no point if email is duplicate)**

Order of operations matters:


Fail fast on cheap operations before expensive ones.

---



```kotlin
// ✅ Efficient: Check uniqueness first
if (userRepository.emailExists(request.email)) {
    throw ConflictException(...)  // Fast database lookup
}
val hash = PasswordHasher.hashPassword(request.password)  // Expensive bcrypt

// ❌ Wasteful: Hash first, then check
val hash = PasswordHasher.hashPassword(request.password)  // Wasted CPU if email is duplicate
if (userRepository.emailExists(request.email)) {
    throw ConflictException(...)
}
```
