---
type: "THEORY"
title: "Introduction to Compose Multiplatform UI"
---

**Estimated Time**: 70 minutes

**Learning Objectives**:
- Master the @Composable annotation and its purpose
- Understand declarative UI principles and state-driven updates
- Learn how recomposition works in Compose 1.10
- Build reactive UI that responds to data changes

---

## Building UI with Composable Functions

**Compose Multiplatform** brings the declarative UI paradigm to all platforms. Instead of manipulating UI elements imperatively, you describe what your UI should look like based on the current state—and Compose handles the rest.

### The @Composable Annotation

Every UI component in CMP is a function marked with `@Composable`:

```kotlin
@Composable
fun Greeting(name: String) {
    Text(text = "Hello, $name!")
}
```

This annotation tells the Compose compiler to treat this function specially—it can emit UI elements, manage state, and participate in the composition tree. Unlike regular functions, composables can only be called from other composables or composable contexts.

### Declarative vs Imperative UI

Traditional UI frameworks use imperative approaches—you manually create views and update them:

```kotlin
// Imperative (old way)
val textView = findViewById<TextView>(R.id.text)
textView.text = "Updated"
textView.visibility = View.VISIBLE
```

Compose uses a **declarative** approach—you describe the UI based on state:

```kotlin
// Declarative (Compose)
@Composable
fun Counter(count: Int) {
    Text(text = "Count: $count")
}
```

The UI automatically updates when `count` changes. No manual view manipulation required!

### State-Driven Recomposition

**Recomposition** is Compose's secret sauce. When state changes, Compose intelligently re-executes only the composables affected by that change—not the entire UI tree. In Compose 1.10, recomposition is optimized to skip composables whose inputs haven't changed.

```kotlin
@Composable
fun StatefulCounter() {
    var count by remember { mutableStateOf(0) }
    
    Button(onClick = { count++ }) {
        Text("Clicked $count times")
    }
}
```

Real-world apps like **KotlinConf** use this pattern extensively—every button, list, and form reacts instantly to data changes. The result? Smooth 60fps interactions with minimal code complexity.