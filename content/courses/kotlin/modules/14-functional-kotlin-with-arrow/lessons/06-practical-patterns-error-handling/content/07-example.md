---
type: "EXAMPLE"
title: "Testing Functional Code"
---


Test patterns for Either-returning functions:



```kotlin
import arrow.core.*
import kotlin.test.*

class UserServiceTest {
    private val repository = FakeUserRepository()
    private val service = UserService(repository)
    
    @Test
    fun `getUser returns user when found`() = runTest {
        // Arrange
        val user = User(1, "John", "john@example.com")
        repository.save(user)
        
        // Act
        val result = service.getUser(1)
        
        // Assert
        assertTrue(result.isRight())
        assertEquals(user, result.getOrNull())
    }
    
    @Test
    fun `getUser returns NotFound when user doesn't exist`() = runTest {
        // Act
        val result = service.getUser(999)
        
        // Assert
        assertTrue(result.isLeft())
        val error = result.leftOrNull()
        assertIs<UserError.NotFound>(error)
        assertEquals(999, error.id)
    }
    
    @Test
    fun `createUser validates email format`() = runTest {
        // Act
        val result = service.createUser("John", "invalid-email")
        
        // Assert
        assertTrue(result.isLeft())
        val error = result.leftOrNull()
        assertIs<UserError.ValidationFailed>(error)
        assertTrue(error.message.contains("email"))
    }
    
    @Test
    fun `createUser returns Conflict when email exists`() = runTest {
        // Arrange
        repository.save(User(1, "Existing", "taken@example.com"))
        
        // Act
        val result = service.createUser("New", "taken@example.com")
        
        // Assert
        assertTrue(result.isLeft())
        assertIs<UserError.EmailConflict>(result.leftOrNull())
    }
}
```
