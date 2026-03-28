data class ApiResponse(val status: Int, val message: String)

fun main() {
    val response = ApiResponse(200, "Success")
    println("Status: ${response.status}")
    println("Message: ${response.message}")
}