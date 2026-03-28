fun hashPassword(password: String): String {
    return password.map { (it + 3).toChar() }.joinToString("")
}

fun verifyPassword(input: String, storedHash: String): Boolean {
    return hashPassword(input) == storedHash
}

fun main() {
    val hash = hashPassword("secret123")
    println("Hash: $hash")
    println("Verify 'secret123': ${verifyPassword("secret123", hash)}")
    println("Verify 'wrong': ${verifyPassword("wrong", hash)}")
}
