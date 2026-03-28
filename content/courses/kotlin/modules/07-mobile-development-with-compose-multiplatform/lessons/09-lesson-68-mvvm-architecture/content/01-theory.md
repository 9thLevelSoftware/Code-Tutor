---
type: "THEORY"
title: "MVVM Architecture in Compose Multiplatform"
---

**Estimated Time**: 70 minutes

**Learning Objectives**:
- Implement MVVM pattern in KMP projects
- Use ViewModel from lifecycle-viewmodel-compose
- Manage StateFlow for reactive state
- Separate business logic from UI

---

## MVVM in Compose Multiplatform

The **Model-View-ViewModel (MVVM)** pattern is the gold standard for organizing mobile applications. In Compose Multiplatform, we adapt this pattern to share business logic across Android and iOS while keeping platform-specific UI implementations separate.

### MVVM Components

```
┌─────────────────────────────────────────────────────────┐
│  UI Layer (Composable Screens)                          │
│  - Displays data from ViewModel                         │
│  - Sends user actions to ViewModel                      │
└──────────────────────┬──────────────────────────────────┘
                       │ observes
┌──────────────────────▼──────────────────────────────────┐
│  ViewModel Layer (Shared)                               │
│  - Holds UI state in StateFlow                          │
│  - Contains business logic                              │
│  - Survives configuration changes                     │
└──────────────────────┬──────────────────────────────────┘
                       │ uses
┌──────────────────────▼──────────────────────────────────┐
│  Repository Layer (Shared with expect/actual)           │
│  - Data operations                                      │
│  - API calls, database access                           │
└─────────────────────────────────────────────────────────┘
```

### ViewModel in KMP

Use the official `lifecycle-viewmodel-compose` library:

```kotlin
// Dependency
implementation("org.jetbrains.androidx.lifecycle:lifecycle-viewmodel-compose:2.8.0")

class TaskViewModel(
    private val repository: TaskRepository
) : ViewModel() {
    
    private val _uiState = MutableStateFlow(TaskUiState())
    val uiState: StateFlow<TaskUiState> = _uiState.asStateFlow()
    
    fun loadTasks() {
        viewModelScope.launch {
            _uiState.update { it.copy(isLoading = true) }
            
            repository.getTasks()
                .catch { error -> 
                    _uiState.update { it.copy(error = error.message) }
                }
                .collect { tasks ->
                    _uiState.update { 
                        it.copy(tasks = tasks, isLoading = false) 
                    }
                }
        }
    }
    
    fun addTask(title: String) {
        viewModelScope.launch {
            repository.addTask(Task(title = title))
        }
    }
}
```

### Consuming in Composables

```kotlin
@Composable
fun TaskScreen(
    viewModel: TaskViewModel = viewModel { TaskViewModel(repository) }
) {
    val state by viewModel.uiState.collectAsState()
    
    TaskList(
        tasks = state.tasks,
        isLoading = state.isLoading,
        onAddTask = viewModel::addTask
    )
}
```

### Business Logic Separation

Keep ViewModels focused on UI state management. Move pure business logic to use cases or repository methods:

```kotlin
// Shared business logic
class ValidateTaskUseCase {
    operator fun invoke(title: String): ValidationResult {
        return when {
            title.isBlank() -> ValidationResult.Error("Title required")
            title.length > 100 -> ValidationResult.Error("Too long")
            else -> ValidationResult.Success
        }
    }
}
```

Real-world apps like **Cash App** and **Netflix** use this architecture—shared ViewModels with platform-specific UI, enabling 70-80% code sharing while maintaining native performance and platform conventions.
