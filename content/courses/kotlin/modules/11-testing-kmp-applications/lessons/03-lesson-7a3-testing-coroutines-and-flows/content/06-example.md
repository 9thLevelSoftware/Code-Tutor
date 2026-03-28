---
type: "EXAMPLE"
title: "Testing Repository Flows"
---

Testing a repository that exposes a reactive Flow:

### Required Dependencies

```toml
# gradle/libs.versions.toml
[versions]
turbine = "1.2.0"

[libraries]
turbine = { module = "app.cash.turbine:turbine", version.ref = "turbine" }
```

```kotlin
// shared/build.gradle.kts
kotlin {
    sourceSets {
        commonTest.dependencies {
            implementation(libs.turbine)
        }
    }
}
```

### Test Implementation

```kotlin
// Production code
class NoteRepositoryImpl(private val database: AppDatabase) : NoteRepository {
    override fun observeAll(): Flow<List<Note>> {
        return database.noteQueries
            .selectAll()
            .asFlow()
            .mapToList(Dispatchers.Default)
    }
}

// Test with Turbine
class NoteRepositoryTest {
    private lateinit var driver: SqlDriver
    private lateinit var database: AppDatabase
    private lateinit var repository: NoteRepositoryImpl
    
    @BeforeTest
    fun setup() {
        driver = JdbcSqliteDriver(JdbcSqliteDriver.IN_MEMORY)
        AppDatabase.Schema.create(driver)
        database = AppDatabase(driver)
        repository = NoteRepositoryImpl(database)
    }
    
    @AfterTest
    fun teardown() {
        driver.close()
    }
    
    @Test
    fun `observeAll emits updates when notes change`() = runTest {
        repository.observeAll().test {
            // Initial empty state
            assertEquals(emptyList(), awaitItem())
            
            // Add a note
            repository.add("Title 1", "Content 1")
            val afterFirst = awaitItem()
            assertEquals(1, afterFirst.size)
            assertEquals("Title 1", afterFirst[0].title)
            
            // Add another note
            repository.add("Title 2", "Content 2")
            val afterSecond = awaitItem()
            assertEquals(2, afterSecond.size)
            
            // Delete a note
            repository.delete(afterSecond[0].id)
            val afterDelete = awaitItem()
            assertEquals(1, afterDelete.size)
            assertEquals("Title 2", afterDelete[0].title)
            
            cancelAndIgnoreRemainingEvents()
        }
    }
    
    @Test
    fun `observeById emits null when note deleted`() = runTest {
        val noteId = repository.add("Test", "Content")
        
        repository.observeById(noteId).test {
            // Note exists
            assertNotNull(awaitItem())
            
            // Delete it
            repository.delete(noteId)
            
            // Now emits null
            assertNull(awaitItem())
            
            cancelAndIgnoreRemainingEvents()
        }
    }
}
```
