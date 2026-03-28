---
type: "THEORY"
title: "🔀 JOIN Queries"
---


### Inner Join

Get books with their reviews:


**SQL equivalent:**

### Left Join

Get all books, even those without reviews:


### Simplified: Get Reviews for Specific Book


---



```kotlin
fun getReviewsForBook(bookId: Int): List<Review> = transaction {
    Reviews.selectAll()
        .where { Reviews.bookId eq bookId }
        .map { rowToReview(it) }
}

// Or with aggregation
fun getAverageRating(bookId: Int): Double? = transaction {
    Reviews.select(Reviews.rating.avg())
        .where { Reviews.bookId eq bookId }
        .singleOrNull()
        ?.get(Reviews.rating.avg())
}
```
