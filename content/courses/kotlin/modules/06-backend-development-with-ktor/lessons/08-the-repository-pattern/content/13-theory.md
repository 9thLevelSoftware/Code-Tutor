---
type: "THEORY"
title: "✅ Solution & Explanation"
---



---



```kotlin
// Repository Interface
interface UserRepository {
    fun getAll(): List<User>
    fun getById(id: Int): User?
    fun getByUsername(username: String): User?
    fun getByEmail(email: String): User?
    fun insert(user: User): Int
    fun update(id: Int, user: User): Boolean
    fun delete(id: Int): Boolean
    fun search(query: String): List<User>
}

// Repository Implementation
class UserRepositoryImpl : UserRepository {
    override fun getAll(): List<User> = transaction {
        Users.selectAll()
            .orderBy(Users.username)
            .map { rowToUser(it) }
    }

    override fun getById(id: Int): User? = transaction {
        Users.selectAll()
            .where { Users.id eq id }
            .map { rowToUser(it) }
            .singleOrNull()
    }

    override fun getByUsername(username: String): User? = transaction {
        Users.selectAll()
            .where { Users.username eq username }
            .map { rowToUser(it) }
            .singleOrNull()
    }

    override fun getByEmail(email: String): User? = transaction {
        Users.selectAll()
            .where { Users.email eq email }
            .map { rowToUser(it) }
            .singleOrNull()
    }

    override fun insert(user: User): Int = transaction {
        Users.insert {
            it[username] = user.username
            it[email] = user.email
            it[passwordHash] = user.passwordHash
            it[createdAt] = LocalDateTime.now()
        }[Users.id]
    }

    override fun update(id: Int, user: User): Boolean = transaction {
        Users.update({ Users.id eq id }) {
            it[email] = user.email
            it[passwordHash] = user.passwordHash
        } > 0
    }

    override fun delete(id: Int): Boolean = transaction {
        Users.deleteWhere { Users.id eq id } > 0
    }

    override fun search(query: String): List<User> = transaction {
        Users.selectAll()
            .where {
                (Users.username like "%$query%") or
                (Users.email like "%$query%")
            }
            .map { rowToUser(it) }
    }

    private fun rowToUser(row: ResultRow): User {
        return User(
            id = row[Users.id],
            username = row[Users.username],
            email = row[Users.email],
            passwordHash = row[Users.passwordHash],
            createdAt = row[Users.createdAt].toString()
        )
    }
}

// Service
class UserService(
    private val userRepository: UserRepository
) {

    fun createUser(request: CreateUserRequest): Result<User> {
        // Validate username
        if (request.username.length < 3) {
            return Result.failure(ValidationException("Username must be at least 3 characters"))
        }

        // Validate email
        if (!request.email.contains("@")) {
            return Result.failure(ValidationException("Invalid email address"))
        }

        // Validate password
        if (request.password.length < 8) {
            return Result.failure(ValidationException("Password must be at least 8 characters"))
        }

        // Check for duplicates
        if (userRepository.getByUsername(request.username) != null) {
            return Result.failure(DuplicateException("Username already exists"))
        }

        if (userRepository.getByEmail(request.email) != null) {
            return Result.failure(DuplicateException("Email already exists"))
        }

        // Hash password (simplified - use BCrypt in production!)
        val passwordHash = request.password.hashCode().toString()

        val user = User(
            id = 0,
            username = request.username,
            email = request.email,
            passwordHash = passwordHash,
            createdAt = ""
        )

        val id = userRepository.insert(user)
        val created = userRepository.getById(id)
            ?: return Result.failure(Exception("Failed to create user"))

        return Result.success(created)
    }

    fun getAllUsers(): List<User> {
        return userRepository.getAll()
    }

    fun getUser(id: Int): User? {
        return userRepository.getById(id)
    }

    fun deleteUser(id: Int): Result<Unit> {
        if (userRepository.getById(id) == null) {
            return Result.failure(NotFoundException("User not found"))
        }

        val deleted = userRepository.delete(id)
        return if (deleted) {
            Result.success(Unit)
        } else {
            Result.failure(Exception("Failed to delete user"))
        }
    }
}

// Routes
fun Route.userRoutes(userService: UserService) {
    route("/api/users") {
        get {
            val users = userService.getAllUsers()
            call.respond(ApiResponse(success = true, data = users))
        }

        get("/{id}") {
            val id = call.parameters["id"]?.toIntOrNull()
                ?: return@get call.respond(HttpStatusCode.BadRequest)

            val user = userService.getUser(id)
            if (user == null) {
                call.respond(HttpStatusCode.NotFound)
            } else {
                call.respond(ApiResponse(success = true, data = user))
            }
        }

        post {
            val request = call.receive<CreateUserRequest>()

            userService.createUser(request)
                .onSuccess { user ->
                    call.respond(
                        HttpStatusCode.Created,
                        ApiResponse(success = true, data = user)
                    )
                }
                .onFailure { error ->
                    when (error) {
                        is ValidationException -> call.respond(
                            HttpStatusCode.BadRequest,
                            ApiResponse<User>(success = false, message = error.message)
                        )
                        is DuplicateException -> call.respond(
                            HttpStatusCode.Conflict,
                            ApiResponse<User>(success = false, message = error.message)
                        )
                        else -> call.respond(HttpStatusCode.InternalServerError)
                    }
                }
        }

        delete("/{id}") {
            val id = call.parameters["id"]?.toIntOrNull()
                ?: return@delete call.respond(HttpStatusCode.BadRequest)

            userService.deleteUser(id)
                .onSuccess { call.respond(HttpStatusCode.OK) }
                .onFailure { call.respond(HttpStatusCode.NotFound) }
        }
    }
}
```
