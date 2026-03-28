data class RegisterRequest(val username: String, val email: String, val password: String)

fun validateRegistration(request: RegisterRequest): List<String> {
    val errors = mutableListOf<String>()
    // Add validation logic
    
    return errors
}

fun main() {
    val request1 = RegisterRequest("ab", "invalidemail", "pass")
    val errors = validateRegistration(request1)
    println("Errors: $errors")
    
    val request2 = RegisterRequest("alice", "alice@example.com", "password123")
    val errors2 = validateRegistration(request2)
    println("Valid: ${errors2.isEmpty()}")
}