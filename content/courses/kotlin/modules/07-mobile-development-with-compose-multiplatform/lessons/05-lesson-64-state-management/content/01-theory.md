---
type: "THEORY"
title: "State Management in Compose"
---

**Estimated Time**: 70 minutes

**Learning Objectives**:
- Use remember and mutableStateOf for local state
- Implement ViewModel with StateFlow for shared state
- Apply derivedStateOf for computed values
- Understand state hoisting patterns

---

## Managing State in Compose Multiplatform

State management is the heart of every Compose application. Whether it's a simple counter or a complex data-driven UI, understanding how to handle state properly ensures your app remains responsive and maintainable.

### Local State with remember

For UI-specific state that doesn't need to survive configuration changes, use `remember` with `mutableStateOf`:

```kotlin
@Composable
fun ExpandingCard() {
    var expanded by remember { mutableStateOf(false) }
    
    Card(onClick = { expanded = !expanded }) {
        Text("Header")
        if (expanded) {
            Text("Expanded content here...")
        }
    }
}
```

The `remember` function stores the state across recompositions, while `mutableStateOf` creates observable state that triggers recomposition when changed.

### Shared State with ViewModel

For state shared across multiple composables or surviving configuration changes, use a **ViewModel** with **StateFlow**:

```kotlin
class TaskViewModel : ViewModel() {
    private val _tasks = MutableStateFlow<List<Task>>(emptyList())
    val tasks: StateFlow<List<Task>> = _tasks.asStateFlow()
    
    fun addTask(task: Task) {
        _tasks.update { it + task }
    }
}

// In Composable
@Composable
fun TaskList(viewModel: TaskViewModel = viewModel()) {
    val tasks by viewModel.tasks.collectAsState()
    LazyColumn {
        items(tasks) { task -> TaskItem(task) }
    }
}
```

### Derived State

When you need computed values based on other state, use `derivedStateOf` for performance:

```kotlin
val completedCount by remember {
    derivedStateOf { tasks.count { it.isCompleted } }
}
```

This only recalculates when `tasks` actually changes, not on every recomposition.

### State Hoisting

The key pattern for reusable composables: **state hoisting** moves state up to the caller:

```kotlin
// Stateless - reusable
@Composable
fun SearchBar(
    query: String,
    onQueryChange: (String) -> Unit
) { /* ... */ }
```

Real-world apps like **JetBrains Toolbox** leverage these patterns to manage complex UI state across platforms while maintaining smooth performance and testability.
