---
type: "THEORY"
title: "✅ Solution & Explanation"
---



---



```kotlin
// Table definition
object Comments : Table() {
    val id = integer("id").autoIncrement()
    val reviewId = integer("review_id").references(Reviews.id)
    val commenterName = varchar("commenter_name", 100)
    val text = text("text")
    val createdAt = datetime("created_at")

    override val primaryKey = PrimaryKey(id)
}

// Model
@Serializable
data class Comment(
    val id: Int,
    val reviewId: Int,
    val commenterName: String,
    val text: String,
    val createdAt: String
)

@Serializable
data class CreateCommentRequest(
    val commenterName: String,
    val text: String
)

// DAO
object CommentDao {
    fun insert(reviewId: Int, request: CreateCommentRequest): Int = transaction {
        Comments.insert {
            it[Comments.reviewId] = reviewId
            it[commenterName] = request.commenterName
            it[text] = request.text
            it[createdAt] = LocalDateTime.now()
        }[Comments.id]
    }

    fun getByReviewId(reviewId: Int): List<Comment> = transaction {
        Comments.selectAll()
            .where { Comments.reviewId eq reviewId }
            .orderBy(Comments.createdAt)
            .map { rowToComment(it) }
    }

    fun delete(id: Int, reviewId: Int): Boolean = transaction {
        Comments.deleteWhere {
            (Comments.id eq id) and (Comments.reviewId eq reviewId)
        } > 0
    }

    private fun rowToComment(row: ResultRow): Comment {
        return Comment(
            id = row[Comments.id],
            reviewId = row[Comments.reviewId],
            commenterName = row[Comments.commenterName],
            text = row[Comments.text],
            createdAt = row[Comments.createdAt].toString()
        )
    }
}

// Routes
fun Route.commentRoutes() {
    route("/api/books/{bookId}/reviews/{reviewId}/comments") {
        get {
            val reviewId = call.parameters["reviewId"]?.toIntOrNull()
                ?: return@get call.respond(HttpStatusCode.BadRequest)

            val comments = CommentDao.getByReviewId(reviewId)
            call.respond(ApiResponse(success = true, data = comments))
        }

        post {
            val reviewId = call.parameters["reviewId"]?.toIntOrNull()
                ?: return@post call.respond(HttpStatusCode.BadRequest)

            val request = call.receive<CreateCommentRequest>()
            val commentId = CommentDao.insert(reviewId, request)

            call.respond(HttpStatusCode.Created)
        }

        delete("/{commentId}") {
            val reviewId = call.parameters["reviewId"]?.toIntOrNull()
                ?: return@delete call.respond(HttpStatusCode.BadRequest)
            val commentId = call.parameters["commentId"]?.toIntOrNull()
                ?: return@delete call.respond(HttpStatusCode.BadRequest)

            val deleted = CommentDao.delete(commentId, reviewId)
            if (deleted) {
                call.respond(HttpStatusCode.OK)
            } else {
                call.respond(HttpStatusCode.NotFound)
            }
        }
    }
}

// Update DatabaseFactory
transaction(database) {
    SchemaUtils.create(Books, Reviews, Comments)
}
```
