import kotlinx.coroutines.test.*
import org.koin.core.context.startKoin
import org.koin.dsl.module
import org.koin.test.AutoCloseKoinTest
import org.koin.test.KoinTest
import org.koin.test.inject
import kotlin.test.*

// ========== Fake Repository ==========

class FakeBookRepository : BookRepository {
    private val books = mutableListOf<Book>()
    var shouldFail = false
    var failureMessage = "Simulated error"
    
    override suspend fun getAllBooks(): List<Book> {
        if (shouldFail) throw Exception(failureMessage)
        return books.toList()
    }
    
    override suspend fun addBook(title: String, author: String): Book {
        if (shouldFail) throw Exception(failureMessage)
        val book = Book(
            id = (books.size + 1).toString(),
            title = title,
            author = author
        )
        books.add(book)
        return book
    }
    
    override suspend fun deleteBook(id: String) {
        if (shouldFail) throw Exception(failureMessage)
        books.removeAll { it.id == id }
    }
    
    // Test helpers
    fun addTestBook(title: String, author: String) {
        books.add(Book((books.size + 1).toString(), title, author))
    }
    
    fun clear() {
        books.clear()
        shouldFail = false
    }
}

// ========== Test Module ==========

val testModule = module {
    single { FakeBookRepository() }
    single<BookRepository> { get<FakeBookRepository>() }
    factory { params ->
        BookViewModel(
            repository = get(),
            dispatcher = params.getOrNull() ?: StandardTestDispatcher()
        )
    }
}

// ========== Tests ==========

class BookViewModelTest : KoinTest, AutoCloseKoinTest {
    
    private val repository: FakeBookRepository by inject()
    private val testDispatcher = StandardTestDispatcher()
    
    @BeforeTest
    fun setUp() {
        repository.clear()
        startKoin {
            modules(testModule)
        }
    }
    
    @Test
    fun `initial state has isLoading false and empty books`() {
        val viewModel = BookViewModel(repository, testDispatcher)
        
        assertFalse(viewModel.state.value.isLoading)
        assertTrue(viewModel.state.value.books.isEmpty())
        assertNull(viewModel.state.value.error)
    }
    
    @Test
    fun `loadBooks populates state with books from repository`() = runTest(testDispatcher) {
        // Given
        repository.addTestBook("Clean Code", "Robert Martin")
        repository.addTestBook("Refactoring", "Martin Fowler")
        
        val viewModel = BookViewModel(repository, testDispatcher)
        
        // When
        viewModel.loadBooks()
        advanceUntilIdle()
        
        // Then
        val state = viewModel.state.value
        assertFalse(state.isLoading)
        assertEquals(2, state.books.size)
        assertEquals("Clean Code", state.books[0].title)
        assertNull(state.error)
    }
    
    @Test
    fun `loadBooks sets error state when repository fails`() = runTest(testDispatcher) {
        // Given
        repository.shouldFail = true
        repository.failureMessage = "Network error"
        
        val viewModel = BookViewModel(repository, testDispatcher)
        
        // When
        viewModel.loadBooks()
        advanceUntilIdle()
        
        // Then
        val state = viewModel.state.value
        assertFalse(state.isLoading)
        assertTrue(state.books.isEmpty())
        assertEquals("Network error", state.error)
    }
    
    @Test
    fun `loadBooks sets isLoading true while loading`() = runTest(testDispatcher) {
        val viewModel = BookViewModel(repository, testDispatcher)
        
        // When - don't advance dispatcher
        viewModel.loadBooks()
        
        // Then - should be loading
        assertTrue(viewModel.state.value.isLoading)
        
        // Complete the coroutine
        advanceUntilIdle()
        assertFalse(viewModel.state.value.isLoading)
    }
}