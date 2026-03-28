---
type: "EXAMPLE"
title: "Unit Testing with Fake Modules"
---

Replace real dependencies with fakes for isolated testing:

```kotlin
// ========== Fake Implementations ==========

class FakeNotesRepository : NotesRepository {
    private val notes = mutableListOf<Note>()
    
    override suspend fun getAllNotes(): List<Note> = notes.toList()
    
    override suspend fun createNote(title: String, content: String): Note {
        val note = Note(
            id = (notes.size + 1).toString(),
            title = title,
            content = content,
            createdAt = Clock.System.now()
        )
        notes.add(note)
        return note
    }
    
    override suspend fun deleteNote(id: String) {
        notes.removeAll { it.id == id }
    }
    
    // Test helper
    fun addTestNote(title: String, content: String) {
        notes.add(Note(
            id = notes.size.toString(),
            title = title,
            content = content,
            createdAt = Clock.System.now()
        ))
    }
}

// ========== Test Module ==========

val testModule = module {
    single<NotesRepository> { FakeNotesRepository() }
    viewModel { NotesListViewModel(get()) }
}

// ========== Tests ==========

class NotesListViewModelTest : KoinTest, AutoCloseKoinTest {
    
    private val viewModel: NotesListViewModel by inject()
    private val repository: FakeNotesRepository by inject()
    
    @BeforeTest
    fun setUp() {
        startKoin {
            modules(testModule)
        }
    }
    
    @Test
    fun `loading notes populates state`() = runTest {
        // Given
        repository.addTestNote("Test Note", "Content")
        
        // When
        viewModel.loadNotes()
        advanceUntilIdle()
        
        // Then
        assertEquals(1, viewModel.state.value.notes.size)
        assertEquals("Test Note", viewModel.state.value.notes[0].title)
    }
    
    @Test
    fun `creating note adds to list`() = runTest {
        // When
        viewModel.createNote("New Note", "New Content")
        advanceUntilIdle()
        
        // Then
        assertEquals(1, viewModel.state.value.notes.size)
    }
}
```

**Note:** `AutoCloseKoinTest` automatically stops Koin after each test. No manual `stopKoin()` needed!
