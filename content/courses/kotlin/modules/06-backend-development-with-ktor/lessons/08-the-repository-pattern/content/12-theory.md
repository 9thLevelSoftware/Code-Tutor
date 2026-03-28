---
type: "THEORY"
title: "🎯 Exercise: Implement User Repository & Service"
---


Create a complete User system with the repository pattern:

### Requirements

1. **UserRepository interface** with:
   - getAll, getById, getByUsername, getByEmail
   - insert, update, delete
   - search(query)

2. **UserRepositoryImpl** with Exposed

3. **UserService** with:
   - Business logic: username must be unique, email must be valid
   - Password requirements (min 8 chars)
   - createUser, updateUser, deleteUser, searchUsers

4. **Routes** using the service

---

