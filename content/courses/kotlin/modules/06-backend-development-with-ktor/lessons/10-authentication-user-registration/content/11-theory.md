---
type: "THEORY"
title: "Checkpoint Quiz"
---


### Question 1
What's the critical difference between hashing and encryption?

A) Hashing is faster than encryption
B) Hashing is one-way (irreversible), encryption is two-way (reversible)
C) Hashing uses more CPU than encryption
D) They're the same thing

### Question 2
What is a "salt" in password hashing?

A) Random data added to each password before hashing
B) A type of encryption algorithm
C) The cost factor in bcrypt
D) The password strength requirement

### Question 3
Why should you NEVER expose password hashes in API responses?

A) They take up too much bandwidth
B) They're ugly and users don't need them
C) Attackers can use them for offline brute-force attacks
D) It violates JSON formatting standards

### Question 4
What is the recommended bcrypt cost factor for 2025?

A) 4 (fast)
B) 8 (balanced)
C) 12 (secure, recommended default)
D) 20 (maximum security)

### Question 5
Why do we check email uniqueness BEFORE hashing the password?

A) It's required by the database
B) It saves CPU cycles (hashing is expensive, no point if email is duplicate)
C) It makes the code run faster
D) bcrypt doesn't work with duplicate emails

---

