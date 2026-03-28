---
type: "THEORY"
title: "🛠️ Setting Up Exposed"
---


### Step 1: Add Dependencies

Update your `build.gradle.kts`:


**What each dependency does:**
- **exposed-core**: Core Exposed functionality
- **exposed-jdbc**: JDBC integration (standard Java database API)
- **exposed-dao**: DAO (Data Access Object) pattern support
- **h2**: The actual database engine
- **HikariCP**: Manages database connection pool (reuses connections efficiently)

### Step 2: Create Database Configuration

Create `src/main/kotlin/com/example/database/DatabaseFactory.kt`:


**Understanding the configuration:**

- **jdbc:h2**: Using H2 database
- **mem:test**: In-memory database named "test"
- **DB_CLOSE_DELAY=-1**: Keep database open even when no connections

- Connection pool: Reuses up to 3 database connections
- More efficient than creating a new connection for every request

---



```kotlin
maximumPoolSize = 3
```
