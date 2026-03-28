---
type: "THEORY"
title: "Introduction"
---

**Estimated Time**: 50 minutes

Integration tests verify that multiple components work together correctly. This lesson covers testing repositories with real databases, API clients with mock servers, and full feature flows.

**What to Integration Test**

Integration tests cover:
- Repository + Database interactions
- API client + Network layer
- ViewModel + Repository coordination
- Full feature workflows

**Testing Repositories with Real Database**

Use SQLDelight's in-memory driver for tests:

```kotlin
class TaskRepositoryIntegrationTest {
    private lateinit var database: AppDatabase
    private lateinit var repository: TaskRepository
    
    @BeforeTest
    fun setup() {
        // In-memory database for tests
        val driver = JdbcSqliteDriver(JdbcSqliteDriver.IN_MEMORY)
        AppDatabase.Schema.create(driver)
        database = AppDatabase(driver)
        repository = TaskRepository(database.taskQueries)
    }
    
    @Test
    fun addTaskPersistsToDatabase() = runTest {
        val task = Task("Test Task", "Description")
        repository.addTask(task)
        
        val retrieved = repository.getTask(task.id)
        assertEquals(task.title, retrieved?.title)
    }
}
```

**Testing API Clients with MockWebServer**

```kotlin
class ApiIntegrationTest {
    private val mockServer = MockWebServer()
    private lateinit var apiClient: ApiClient
    
    @BeforeTest
    fun setup() {
        mockServer.start()
        apiClient = ApiClient(baseUrl = mockServer.url("/"))
    }
    
    @Test
    fun fetchUsersReturnsParsedData() = runTest {
        // Enqueue mock response
        mockServer.enqueue(
            MockResponse()
                .setBody("""[{\"id\": 1, \"name\": \"John\"}]""")
                .setResponseCode(200)
        )
        
        val users = apiClient.getUsers()
        
        assertEquals(1, users.size)
        assertEquals("John", users[0].name)
        
        // Verify request
        val request = mockServer.takeRequest()
        assertEquals("/users", request.path)
    }
}
```

**End-to-End Feature Testing**

Test complete user workflows:

```kotlin
@Test
fun completeLoginFlow() = runTest {
    // Given: Setup components
    val authApi = FakeAuthApi()
    val repository = AuthRepository(authApi)
    val viewModel = LoginViewModel(repository)
    
    // When: Simulate user actions
    viewModel.onEmailChanged("user@test.com")
    viewModel.onPasswordChanged("password123")
    viewModel.onLoginClick()
    
    // Then: Verify final state
    advanceUntilIdle()
    assertEquals(AuthState.Authenticated, viewModel.authState.value)
}
```

**Best Practices**

1. Use real dependencies where lightweight (in-memory DBs)
2. Mock external systems (APIs) to avoid flakiness
3. Clean state between tests - reset databases/servers
4. Keep integration tests in `commonTest` when possible
5. Run integration tests in CI but not on every file change

Integration tests bridge the gap between unit tests and full E2E tests, catching issues that only appear when components interact.