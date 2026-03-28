---
type: "THEORY"
title: "Networking and APIs in Compose Multiplatform"
---

**Estimated Time**: 75 minutes

**Learning Objectives**:
- Configure Ktor client for cross-platform networking
- Handle HTTP requests with proper error handling
- Integrate API calls with Compose UI using ViewModels
- Implement loading states and error UI

---

## Networking in CMP with Ktor

Modern mobile apps live and breathe data from APIs. Compose Multiplatform pairs perfectly with **Ktor Client**—Kotlin's native networking solution that works seamlessly across all platforms.

### Setting Up Ktor Client

Add the Ktor client dependencies:

```kotlin
// Common module
implementation("io.ktor:ktor-client-core:3.4.1")
implementation("io.ktor:ktor-client-content-negotiation:3.4.1")
implementation("io.ktor:ktor-serialization-kotlinx-json:3.4.1")

// Platform-specific engines
// Android: io.ktor:ktor-client-android
// iOS: io.ktor:ktor-client-darwin
```

### Making HTTP Requests

Create a reusable API client:

```kotlin
class ApiClient {
    private val client = HttpClient {
        install(ContentNegotiation) {
            json(Json { ignoreUnknownKeys = true })
        }
        defaultRequest {
            url("https://api.example.com/")
            header(HttpHeaders.ContentType, ContentType.Application.Json)
        }
    }
    
    suspend fun fetchTasks(): List<Task> = 
        client.get("tasks").body()
}
```

### Integrating with Compose

Expose API data through ViewModels:

```kotlin
class TaskViewModel(private val api: ApiClient) : ViewModel() {
    private val _uiState = MutableStateFlow<UiState<List<Task>>>(UiState.Loading)
    val uiState: StateFlow<UiState<List<Task>>> = _uiState.asStateFlow()
    
    fun loadTasks() {
        viewModelScope.launch {
            _uiState.value = try {
                UiState.Success(api.fetchTasks())
            } catch (e: Exception) {
                UiState.Error(e.message ?: "Unknown error")
            }
        }
    }
}
```

### Handling States in UI

```kotlin
@Composable
fun TaskScreen(viewModel: TaskViewModel) {
    val state by viewModel.uiState.collectAsState()
    
    when (val s = state) {
        is UiState.Loading -> CircularProgressIndicator()
        is UiState.Success -> TaskList(s.data)
        is UiState.Error -> ErrorMessage(s.message)
    }
}
```

Real-world apps like **Cash App** and **Robinhood** use similar patterns—Ktor for networking, sealed classes for state management, and Compose for reactive UI updates. The result is robust, maintainable networking code that works identically on Android and iOS.
