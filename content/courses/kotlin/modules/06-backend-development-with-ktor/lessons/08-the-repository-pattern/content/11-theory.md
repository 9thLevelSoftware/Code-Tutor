---
type: "THEORY"
title: "📂 Complete Project Structure"
---



---



```kotlin
src/main/kotlin/com/example/
├── Application.kt                    # Entry point, DI setup
├── database/
│   ├── DatabaseFactory.kt           # Database initialization
│   └── tables/
│       ├── Books.kt                  # Table definitions
│       └── Reviews.kt
├── models/
│   ├── Book.kt                       # Domain models
│   ├── Review.kt
│   ├── Requests.kt                   # API request models
│   └── Responses.kt                  # API response models
├── repositories/
│   ├── BookRepository.kt             # Interface
│   ├── BookRepositoryImpl.kt         # Implementation
│   ├── ReviewRepository.kt
│   └── ReviewRepositoryImpl.kt
├── services/
│   ├── BookService.kt                # Business logic
│   ├── ReviewService.kt
│   └── Exceptions.kt                 # Custom exceptions
└── plugins/
    ├── Routing.kt                    # HTTP routes
    └── Serialization.kt              # JSON config
```
