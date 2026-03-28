fun main() {
    val url = "/users/123"
    val userId = url.substringAfterLast("/")
    
    println("User ID: $userId")
}