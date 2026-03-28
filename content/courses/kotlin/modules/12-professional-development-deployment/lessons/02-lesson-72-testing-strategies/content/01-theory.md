---
type: "THEORY"
title: "Introduction"
---

**Estimated Time**: 50 minutes

A comprehensive testing strategy ensures your KMP application works reliably across all platforms. This lesson covers organizing test suites, test doubles, and coverage goals for production applications.

**Test Organization Strategy**

```
commonTest/
├── unit/
│   ├── domain/          # Use case tests
│   ├── model/           # Data class tests
│   └── utils/           # Helper function tests
├── integration/
│   ├── repository/      # Repository + DB tests
│   └── api/             # API client tests
└── fakes/               # Shared test doubles

androidUnitTest/         # Android-specific logic
androidInstrumented/     # UI and integration tests
iosTest/                 # iOS-specific tests
```

**Test Doubles Strategy**

Use the right double for each scenario:

```kotlin
// Fake: Lightweight in-memory implementation
class FakeTaskRepository : TaskRepository {
    private val tasks = mutableListOf<Task>()
    
    override suspend fun getTasks() = tasks.toList()
    override suspend fun addTask(task: Task) { tasks.add(task) }
}

// Mock: Verify interactions (use sparingly)
val mockApi = mockk<ApiService>()
every { mockApi.fetchTasks() } returns listOf(task)

// Stub: Predefined responses
class StubAnalytics : Analytics {
    val trackedEvents = mutableListOf<String>()
    override fun track(event: String, params: Map<String, String>) {
        trackedEvents.add(event)
    }
}
```

**Coverage Goals by Layer**

| Layer | Target Coverage | Test Types |
|-------|-------------------|------------|
| Domain | 90%+ | Unit tests |
| Repository | 80%+ | Integration tests |
| ViewModel | 85%+ | Unit + Coroutine tests |
| UI | 60%+ | Screenshot + E2E |

**Testing Strategy for Use Cases**

```kotlin
class GetTasksUseCaseTest {
    private lateinit var repository: FakeTaskRepository
    private lateinit var useCase: GetTasksUseCase
    
    @BeforeTest
    fun setup() {
        repository = FakeTaskRepository()
        useCase = GetTasksUseCase(repository)
    }
    
    @Test
    fun returnsTasksFromRepository() = runTest {
        val expected = listOf(Task("1", "Test"))
        repository.addTask(expected[0])
        
        val result = useCase()
        
        assertTrue(result.isSuccess)
        assertEquals(expected, result.getOrNull())
    }
    
    @Test
    fun returnsFailureWhenRepositoryThrows() = runTest {
        repository.shouldThrow = true
        
        val result = useCase()
        
        assertTrue(result.isFailure)
    }
}
```

**CI Test Strategy**

```yaml
# Run tests in order of speed
- name: Fast feedback
  run: ./gradlew :shared:commonTest  # ~30 seconds

- name: Android unit tests
  run: ./gradlew :shared:androidUnitTest  # ~1 minute

- name: Full suite
  run: ./gradlew allTests  # ~5 minutes
```

**Best Practices**

1. Fakes over mocks - more maintainable, test behavior not implementation
2. One assertion per test concept (can have multiple asserts)
3. Test sad paths: network errors, empty states, invalid input
4. Keep test code as clean as production code
5. Document why tests exist, not what they do

A well-designed testing strategy catches bugs early and gives confidence to refactor fearlessly.
