---
type: "EXAMPLE"
title: "Testing Error Accumulation"
---


Test patterns for validation with zipOrAccumulate:



```kotlin
import arrow.core.*
import kotlin.test.*

class RegistrationValidatorTest {

    @Test
    fun `valid input returns Right`() {
        val result = validateRegistration(
            username = "johndoe",
            email = "john@example.com",
            password = "password123",
            age = 25
        )

        assertTrue(result.isRight())
        assertEquals("johndoe", result.getOrNull()!!.username)
    }

    @Test
    fun `collects all validation errors`() {
        val result = validateRegistration(
            username = "ab",           // Too short
            email = "invalid",         // No @
            password = "123",          // Too short
            age = 16                   // Too young
        )

        assertTrue(result.isLeft())
        val errors = result.leftOrNull()!!

        assertEquals(4, errors.size)
        assertTrue(errors.any { "username" in it.lowercase() || "3 characters" in it })
        assertTrue(errors.any { "email" in it.lowercase() })
        assertTrue(errors.any { "password" in it.lowercase() || "8 characters" in it })
        assertTrue(errors.any { "18" in it || "older" in it.lowercase() })
    }

    @Test
    fun `single error returns NonEmptyList with one element`() {
        val result = validateRegistration(
            username = "validuser",
            email = "valid@email.com",
            password = "validpassword",
            age = 16  // Only this fails
        )

        assertTrue(result.isLeft())
        assertEquals(1, result.leftOrNull()!!.size)
    }
}
```
