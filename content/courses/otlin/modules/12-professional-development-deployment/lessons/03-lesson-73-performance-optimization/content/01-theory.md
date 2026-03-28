---
type: "THEORY"
title: "Introduction"
---

**Estimated Time**: 45 minutes

Performance optimization in KMP requires understanding platform-specific characteristics while keeping shared code efficient. This lesson covers memory management, rendering optimization, and profiling techniques.

**Compose Performance Best Practices**

```kotlin
// BAD: Unnecessary recompositions
@Composable
fun UserList(users: List<User>) {
    Column {
        users.forEach { user ->
            UserCard(user) // Recomposes when any user changes
        }
    }
}

// GOOD: Stable keys and remembered items
@Composable
fun UserList(users: List<User>) {
    LazyColumn {
        items(
            items = users,
            key = { it.id } // Stable identity
        ) { user ->
            UserCard(
                user = user,
                modifier = Modifier.animateItem() // Smooth animations
            )
        }
    }
}
```

**Memory Management**

```kotlin
// Use WeakReference for large objects that can be GC'd
class ImageCache {
    private val cache = mutableMapOf<String, WeakReference<ImageBitmap>>()
    
    fun get(key: String): ImageBitmap? {
        return cache[key]?.get()?.also {
            // Refresh reference if still valid
            cache[key] = WeakReference(it)
        }
    }
}

// Properly scope coroutines to avoid leaks
class ViewModel : CoroutineScope by MainScope() {
    fun loadData() {
        launch {
            // Work here
        }
    }
    
    fun cleanup() {
        cancel() // Cancel all coroutines
    }
}
```

**Flow Optimization**

```kotlin
// BAD: Creates new flow on every collection
fun getUserData(): Flow<User> = flow { /* expensive */ }

// GOOD: Share and replay for multiple collectors
private val _userData = flow { /* expensive */ }
    .shareIn(
        scope = viewModelScope,
        started = SharingStarted.WhileSubscribed(5000),
        replay = 1
    )
val userData: Flow<User> = _userData

// Debounce rapid changes
searchQuery
    .debounce(300) // Wait 300ms after last keystroke
    .flatMapLatest { query ->
        searchRepository.search(query)
    }
```

**Profiling Tools by Platform**

| Platform | Tool | Use For |
|----------|------|---------|
| Android | Android Studio Profiler | CPU, Memory, Network |
| iOS | Xcode Instruments | CPU, Memory, GPU |
| Desktop | VisualVM | JVM heap analysis |
| KMP | Kotlin/Native Memory Manager | Native memory leaks |

**Build Performance**

```kotlin
// gradle.properties
org.gradle.jvmargs=-Xmx8g -XX:MaxMetaspaceSize=512m
org.gradle.caching=true
org.gradle.parallel=true
kotlin.incremental=true
kotlin.compiler.execution.strategy=in-process
```

**Image Optimization**

```kotlin
// Coil for efficient image loading
AsyncImage(
    model = ImageRequest.Builder(LocalContext.current)
        .data(imageUrl)
        .size(300, 300) // Request correct size
        .crossfade(true)
        .build(),
    contentDescription = null
)
```

**Best Practices**

1. Profile before optimizing - measure first
2. Use LazyColumn/LazyRow for large lists
3. Avoid boxing primitives in generics when possible
4. Minimize recomposition with stable keys and remember
5. Cache expensive computations with `remember` or `derivedStateOf`

Performance optimization is an iterative process - start with clean architecture, then optimize based on profiling data.
