---
type: "THEORY"
title: "Exercise: Enhanced User Profile"
---


Extend the user registration system with additional features.

### Requirements

1. **Add Profile Fields**:
   - Username (required, unique, 3-20 chars, alphanumeric + underscore only)
   - Bio (optional, max 500 chars)
   - Date of birth (required, must be 13+ years old)
   - Phone number (optional, if provided must match pattern: +1-XXX-XXX-XXXX)

2. **Update User Model**:
   - Include new fields in User and RegisterRequest
   - Add database columns

3. **Create Username Validator**:
   - Length: 3-20 characters
   - Pattern: Only letters, numbers, underscore
   - Must not start with underscore or number
   - Check uniqueness

4. **Create Age Validator**:
   - Parse date of birth
   - Calculate age
   - Ensure user is at least 13 years old (COPPA compliance)

5. **Create Phone Validator**:
   - Optional but must match pattern if provided
   - Format: +1-XXX-XXX-XXXX (US phone numbers)

### Starter Code


---



```kotlin
@Serializable
data class User(
    val id: Int,
    val email: String,
    val username: String,
    val fullName: String,
    val bio: String?,
    val dateOfBirth: String,
    val phoneNumber: String?,
    val createdAt: String
)

@Serializable
data class RegisterRequest(
    val email: String,
    val username: String,
    val password: String,
    val fullName: String,
    val bio: String? = null,
    val dateOfBirth: String,  // Format: YYYY-MM-DD
    val phoneNumber: String? = null
)

// TODO: Update Users table definition
// TODO: Implement enhanced UserValidator
// TODO: Update UserRepository
// TODO: Test all validation rules
```
