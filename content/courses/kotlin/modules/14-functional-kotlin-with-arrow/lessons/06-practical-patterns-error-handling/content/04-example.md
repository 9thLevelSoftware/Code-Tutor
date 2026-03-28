---
type: "EXAMPLE"
title: "Safe API Calls Pattern"
---


Convert HTTP exceptions to typed errors:



```kotlin
import arrow.core.*
import arrow.core.raise.*
import io.ktor.client.*
import io.ktor.client.call.*
import io.ktor.client.request.*
import java.io.IOException
import java.net.SocketTimeoutException

sealed interface ApiError {
    data class NetworkError(val cause: Throwable) : ApiError
    data class ServerError(val code: Int, val message: String) : ApiError
    data class DeserializationError(val body: String) : ApiError
    data object Timeout : ApiError
}

// Generic safe API call wrapper
context(raise: Raise<ApiError>)
suspend inline fun <reified T> HttpClient.safeGet(url: String): T {
    val response = catch(
        block = { get(url) },
        catch = { e ->
            when (e) {
                is SocketTimeoutException -> raise.raise(ApiError.Timeout)
                is IOException -> raise.raise(ApiError.NetworkError(e))
                else -> throw e
            }
        }
    )

    raise.ensure(response.status.value in 200..299) {
        ApiError.ServerError(response.status.value, response.bodyAsText())
    }

    return catch(
        block = { response.body<T>() },
        catch = { raise.raise(ApiError.DeserializationError(response.bodyAsText())) }
    )
}

// Usage in repository
class UserRepository(private val client: HttpClient) {
    
    suspend fun getUser(id: Long): Either<ApiError, User> = either {
        client.safeGet("https://api.example.com/users/$id")
    }
    
    suspend fun getUsers(): Either<ApiError, List<User>> = either {
        client.safeGet("https://api.example.com/users")
    }
}
```
