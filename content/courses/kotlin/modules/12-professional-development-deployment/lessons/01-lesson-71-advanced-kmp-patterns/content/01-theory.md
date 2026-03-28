---
type: "THEORY"
title: "Introduction"
---

**Estimated Time**: 60 minutes

Professional KMP applications require robust architecture. Clean Architecture and MVI (Model-View-Intent) provide clear separation of concerns and predictable state management across platforms.

**Clean Architecture Layers**

```
Presentation Layer (Compose UI)
         ↓
    ViewModel (MVI)
         ↓
    Domain Layer (Use Cases)
         ↓
   Data Layer (Repository)
         ↓
Platform Layer (API/Database)
```

**Domain Layer: Pure Business Logic**

```kotlin
// domain/model/Task.kt - Platform-agnostic models
data class Task(
    val id: String,
    val title: String,
    val completed: Boolean
)

// domain/repository/TaskRepository.kt
interface TaskRepository {
    suspend fun getTasks(): List<Task>
    suspend fun addTask(task: Task)
    suspend fun completeTask(id: String)
}

// domain/usecase/GetTasksUseCase.kt
class GetTasksUseCase(private val repository: TaskRepository) {
    suspend operator fun invoke(): Result<List<Task>> {
        return try {
            Result.success(repository.getTasks())
        } catch (e: Exception) {
            Result.failure(e)
        }
    }
}
```

**MVI Pattern Implementation**

```kotlin
// Contract: State + Event + Effect
sealed interface TaskUiState {
    data object Loading : TaskUiState
    data class Success(val tasks: List<Task>) : TaskUiState
    data class Error(val message: String) : TaskUiState
}

sealed interface TaskEvent {
    data class LoadTasks(val forceRefresh: Boolean = false) : TaskEvent
    data class CompleteTask(val taskId: String) : TaskEvent
    data class AddTask(val title: String) : TaskEvent
}

sealed interface TaskEffect {
    data class ShowError(val message: String) : TaskEffect
    data object NavigateToTaskDetail : TaskEffect
}

// ViewModel with MVI
class TaskViewModel(
    private val getTasks: GetTasksUseCase,
    private val completeTask: CompleteTaskUseCase
) : ViewModel() {
    
    private val _state = MutableStateFlow<TaskUiState>(TaskUiState.Loading)
    val state: StateFlow<TaskUiState> = _state.asStateFlow()
    
    private val _effects = Channel<TaskEffect>()
    val effects = _effects.receiveAsFlow()
    
    fun onEvent(event: TaskEvent) {
        when (event) {
            is TaskEvent.LoadTasks -> loadTasks(event.forceRefresh)
            is TaskEvent.CompleteTask -> completeTask(event.taskId)
            is TaskEvent.AddTask -> addTask(event.title)
        }
    }
    
    private fun loadTasks(forceRefresh: Boolean) {
        viewModelScope.launch {
            _state.value = TaskUiState.Loading
            getTasks()
                .onSuccess { _state.value = TaskUiState.Success(it) }
                .onFailure { _effects.send(TaskEffect.ShowError(it.message ?: "Unknown")) }
        }
    }
}
```

**Platform-Specific UI**

```kotlin
@Composable
fun TaskScreen(viewModel: TaskViewModel = koinViewModel()) {
    val state by viewModel.state.collectAsState()
    
    // Handle side effects
    LaunchedEffect(Unit) {
        viewModel.effects.collect { effect ->
            when (effect) {
                is TaskEffect.ShowError -> showSnackbar(effect.message)
                TaskEffect.NavigateToTaskDetail -> navigator.navigate("/detail")
            }
        }
    }
    
    // Render UI based on state
    when (val s = state) {
        is TaskUiState.Loading -> LoadingIndicator()
        is TaskUiState.Success -> TaskList(
            tasks = s.tasks,
            onComplete = { viewModel.onEvent(TaskEvent.CompleteTask(it)) }
        )
        is TaskUiState.Error -> ErrorMessage(s.message)
    }
}
```

**Benefits of Clean Architecture + MVI**

1. **Testability**: Domain layer has no dependencies - easy to unit test
2. **Predictability**: Unidirectional data flow eliminates race conditions
3. **Separation**: UI, business logic, and data access are clearly separated
4. **KMP Friendly**: Domain layer is pure Kotlin - shared across all platforms

This architecture scales from small apps to enterprise systems while maintaining code clarity.
