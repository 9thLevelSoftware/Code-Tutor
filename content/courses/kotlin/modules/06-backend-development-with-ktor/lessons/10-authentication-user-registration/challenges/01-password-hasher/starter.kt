fun hashPassword(password: String): String {
    // Shift each character by +3 in ASCII
    TODO("Implement password hashing")
}

fun verifyPassword(input: String, storedHash: String): Boolean {
    // Hash the input and compare with stored hash
    TODO("Implement verification")
}

fun main() {
    val hash = hashPassword("secret123")
    println("Hash: $hash")
    println("Verify 'secret123': ${verifyPassword("secret123", hash)}")
    println("Verify 'wrong': ${verifyPassword("wrong", hash)}")
}
