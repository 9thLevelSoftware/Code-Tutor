---
type: "THEORY"
title: "Navigation in Compose Multiplatform"
---

**Estimated Time**: 75 minutes

**Learning Objectives**:
- Set up navigation in CMP using Voyager or PreCompose
- Implement type-safe navigation with sealed classes
- Pass arguments between screens
- Handle back stack and deep linking

---

## Navigation Architecture in CMP

Navigation is a critical aspect of any mobile application. In Compose Multiplatform, we use specialized libraries to achieve consistent navigation patterns across Android and iOS.

### Navigation Libraries

Compose Multiplatform doesn't include a built-in navigation system like Jetpack Navigation. Instead, we use third-party libraries:

**Voyager** - The most popular choice:
```kotlin
// Build.gradle
implementation("cafe.adriel.voyager:voyager-navigator:1.1.0-beta02")
```

**PreCompose** - Alternative with ViewModel support:
```kotlin
implementation("moe.tlaster:precompose:1.6.2")
```

### Type-Safe Navigation with Voyager

Voyager uses a screen-based approach with sealed classes:

```kotlin
sealed class Screen : Parcelable {
    @Parcelize
    data object Home : Screen()
    
    @Parcelize
    data class Detail(val id: String) : Screen()
}

class HomeScreen : Screen {
    @Composable
    override fun Content() {
        val navigator = LocalNavigator.current
        
        Button(onClick = { navigator.push(DetailScreen("123")) }) {
            Text("Go to Detail")
        }
    }
}
```

### Navigation with Arguments

Passing data between screens:

```kotlin
class TaskDetailScreen(val taskId: String) : Screen {
    @Composable
    override fun Content() {
        // Access taskId directly
        Text("Task ID: $taskId")
    }
}
```

### Platform-Specific Navigation

While the navigation logic is shared, platform-specific behaviors (like iOS swipe-back gestures) are handled automatically by the underlying platform integrations.

Real-world apps like **KotlinConf** use Voyager for seamless navigation across platforms, maintaining a single navigation graph while respecting platform conventions like Android's back button and iOS's navigation gestures.
