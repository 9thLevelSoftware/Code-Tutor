// Create ApiResponse data class

fun main() {
    val response = ApiResponse(200, "Success")
    println("Status: ${response.status}")
    println("Message: ${response.message}")
}