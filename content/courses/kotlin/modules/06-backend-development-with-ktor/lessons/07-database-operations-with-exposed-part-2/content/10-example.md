---
type: "EXAMPLE"
title: "💻 Complete Example: Book Review System"
---


Let's build a complete system with relationships:

### Models


### Enhanced BookDao with Statistics


### ReviewDao


### Routes


### Testing


---



```bash
# Create a review
curl -X POST http://localhost:8080/api/books/1/reviews \
  -H "Content-Type: application/json" \
  -d '{
    "reviewerName": "Alice",
    "rating": 5,
    "comment": "Absolutely brilliant dystopian novel!"
  }'

# Get all reviews for a book
curl http://localhost:8080/api/books/1/reviews

# Get book with statistics
curl http://localhost:8080/api/books/1

# Delete a review
curl -X DELETE http://localhost:8080/api/books/1/reviews/1
```
