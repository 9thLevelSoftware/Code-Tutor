---
type: "THEORY"
title: "🎯 Exercise: Comment System"
---


Add comments on reviews (nested relationship):

### Requirements

1. Create a **Comments** table:
   - id, reviewId (foreign key to Reviews), commenterName, text, createdAt

2. Implement **CommentDao**:
   - insert, getByReviewId, delete

3. Add routes:
   - POST `/api/books/{bookId}/reviews/{reviewId}/comments`
   - GET `/api/books/{bookId}/reviews/{reviewId}/comments`
   - DELETE `/api/books/{bookId}/reviews/{reviewId}/comments/{commentId}`

---

