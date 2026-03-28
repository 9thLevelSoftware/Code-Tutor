---
type: "THEORY"
title: "Advanced UI & Animations"
---

**Estimated Time**: 75 minutes

**Learning Objectives**:
- Use animate*AsState for smooth value animations
- Implement AnimatedVisibility for enter/exit effects
- Create custom transitions between screens
- Handle gestures with pointer input

---

## Bringing Life to Your UI with Animations

Animations transform static interfaces into delightful experiences. Compose Multiplatform provides powerful, declarative animation APIs that work identically across Android and iOS, using the same syntax you'd use in Jetpack Compose.

### animate*AsState for Value Animations

Animate numeric values smoothly:

```kotlin
@Composable
fun AnimatedCounter(target: Int) {
    val animatedValue by animateIntAsState(
        targetValue = target,
        animationSpec = tween(durationMillis = 300)
    )
    
    Text(
        text = "$animatedValue",
        fontSize = 48.sp
    )
}
```

Available variants: `animateFloatAsState`, `animateDpAsState`, `animateColorAsState`, `animateOffsetAsState`.

### AnimatedVisibility

Add and remove content with smooth transitions:

```kotlin
@Composable
fun ExpandableCard(expanded: Boolean) {
    Card {
        Text("Header")
        
        AnimatedVisibility(
            visible = expanded,
            enter = expandVertically() + fadeIn(),
            exit = shrinkVertically() + fadeOut()
        ) {
            Column {
                Text("Expanded line 1")
                Text("Expanded line 2")
            }
        }
    }
}
```

### Custom Transitions

Create shared element transitions between screens:

```kotlin
@Composable
fun TaskDetailTransition(task: Task) {
    var selected by remember { mutableStateOf(false) }
    
    val color by animateColorAsState(
        if (selected) MaterialTheme.colorScheme.primary
        else MaterialTheme.colorScheme.surface
    )
    
    val elevation by animateDpAsState(
        if (selected) 8.dp else 2.dp
    )
    
    Card(
        backgroundColor = color,
        elevation = elevation,
        onClick = { selected = !selected }
    ) {
        Text(task.title)
    }
}
```

### Gesture Handling

Implement custom gestures with pointer input:

```kotlin
@Composable
fun DraggableCard() {
    val offsetX = remember { Animatable(0f) }
    
    Card(
        modifier = Modifier
            .offset { IntOffset(offsetX.value.toInt(), 0) }
            .pointerInput(Unit) {
                detectHorizontalDragGestures { change, dragAmount ->
                    change.consume()
                    scope.launch {
                        offsetX.snapTo(offsetX.value + dragAmount)
                    }
                }
            }
    ) { /* content */ }
}
```

### Pull to Refresh (Compose 1.10 Pattern)

Use the modern `PullToRefreshBox` (not deprecated `SwipeRefresh`):

```kotlin
import androidx.compose.material3.pulltorefresh.PullToRefreshBox

PullToRefreshBox(
    isRefreshing = isRefreshing,
    onRefresh = { viewModel.refresh() }
) {
    LazyColumn { /* content */ }
}
```

Real-world apps like **Trello** and **Slack** use these patterns—subtle animations guide users, gesture interactions feel natural, and smooth transitions create a polished, professional experience across both platforms.
