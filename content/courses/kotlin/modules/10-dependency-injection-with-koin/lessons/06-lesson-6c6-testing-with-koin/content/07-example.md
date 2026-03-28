---
type: "EXAMPLE"
title: "Complete Test Suite Example"
---

A well-organized test structure:

```kotlin
// ========== Test Utilities ==========

// commonTest/kotlin/test/TestModules.kt
object TestModules {
    
    val fakeDataModule = module {
        single { createInMemoryDatabase() }
        single<NotesRepository> { FakeNotesRepository() }
    }
    
    val fakePresentationModule = module {
        viewModel { params ->
            NotesListViewModel(
                repository = get(),
                dispatcher = StandardTestDispatcher()
            )
        }
    }
    
    fun allTestModules() = listOf(
        fakeDataModule,
        fakePresentationModule
    )
}

// ========== Base Test Class ==========

abstract class BaseKoinTest : KoinTest, AutoCloseKoinTest {
    
    @BeforeTest
    fun baseSetUp() {
        startKoin {
            modules(TestModules.allTestModules())
        }
        additionalSetUp()
    }
    
    open fun additionalSetUp() {}
    open fun additionalTearDown() {}
}

// ========== Repository Tests ==========

class NotesRepositoryTest : BaseKoinTest() {
    
    private val repository: NotesRepository by inject()
    
    @Test
    fun `createNote returns note with generated id`() = runTest {
        val note = repository.createNote("Title", "Content")
        
        assertNotNull(note.id)
        assertEquals("Title", note.title)
    }
    
    @Test
    fun `deleteNote removes note from storage`() = runTest {
        val note = repository.createNote("To Delete", "Content")
        
        repository.deleteNote(note.id)
        
        val notes = repository.getAllNotes()
        assertFalse(notes.any { it.id == note.id })
    }
}

// ========== ViewModel Tests ==========

class NotesListViewModelTest : BaseKoinTest() {
    
    private val viewModel: NotesListViewModel by inject()
    private val testDispatcher = StandardTestDispatcher()
    
    @Test
    fun `initial state is loading`() {
        assertTrue(viewModel.state.value.isLoading)
    }
    
    @Test
    fun `loadNotes updates state with notes`() = runTest(testDispatcher) {
        val fakeRepo = get<NotesRepository>() as FakeNotesRepository
        fakeRepo.addTestNote("Test", "Content")
        
        viewModel.loadNotes()
        advanceUntilIdle()
        
        assertFalse(viewModel.state.value.isLoading)
        assertEquals(1, viewModel.state.value.notes.size)
    }
    
    @Test
    fun `error state when repository fails`() = runTest(testDispatcher) {
        // Override with failing repository for this test
        declare<NotesRepository> {
            object : NotesRepository {
                override suspend fun getAllNotes() = 
                    throw Exception("Network error")
                override suspend fun createNote(t: String, c: String) = 
                    throw Exception("Network error")
                override suspend fun deleteNote(id: String) {}
            }
        }
        
        val failingViewModel = NotesListViewModel(get(), testDispatcher)
        failingViewModel.loadNotes()
        advanceUntilIdle()
        
        assertNotNull(failingViewModel.state.value.error)
    }
}

// ========== Module Verification ==========

class KoinModuleTest {
    
    @Test
    fun `production modules are complete`() {
        commonModule.verify(
            extraTypes = listOf(
                SqlDriver::class,
                Context::class
            )
        )
    }
    
    @Test  
    fun `test modules are complete`() {
        TestModules.allTestModules().forEach { it.verify() }
    }
}
```

**Note:** `AutoCloseKoinTest` automatically stops Koin after each test. No manual `stopKoin()` needed!
