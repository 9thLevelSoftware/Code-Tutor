---
type: "EXAMPLE"
title: "Ktor Server Pattern"
---


Error handling in Ktor routes:



```kotlin
import arrow.core.*
import io.ktor.server.application.*
import io.ktor.server.response.*
import io.ktor.server.routing.*
import io.ktor.http.*

sealed interface DomainError {
    data class NotFound(val resource: String, val id: String) : DomainError
    data class Validation(val errors: List<String>) : DomainError
    data class Unauthorized(val reason: String) : DomainError
    data class Conflict(val message: String) : DomainError
}

// Extension function to respond with Either
suspend fun <E : DomainError, A : Any> ApplicationCall.respondEither(
    result: Either<E, A>
) {
    result.fold(
        ifLeft = { error ->
            when (error) {
                is DomainError.NotFound -> {
                    respond(HttpStatusCode.NotFound, 
                        mapOf("error" to "${error.resource} not found", "id" to error.id))
                }
                is DomainError.Validation -> {
                    respond(HttpStatusCode.BadRequest,
                        mapOf("errors" to error.errors))
                }
                is DomainError.Unauthorized -> {
                    respond(HttpStatusCode.Unauthorized,
                        mapOf("error" to error.reason))
                }
                is DomainError.Conflict -> {
                    respond(HttpStatusCode.Conflict,
                        mapOf("error" to error.message))
                }
            }
        },
        ifRight = { value ->
            respond(HttpStatusCode.OK, value)
        }
    )
}

// Usage in routes
fun Route.userRoutes(userService: UserService) {
    route("/users") {
        get("/{id}") {
            val id = call.parameters["id"]?.toLongOrNull()
                ?: return@get call.respond(HttpStatusCode.BadRequest, "Invalid ID")
            
            call.respondEither(userService.getUser(id))
        }
        
        post {
            val request = call.receive<CreateUserRequest>()
            call.respondEither(userService.createUser(request))
        }
    }
}
```
