---
type: "THEORY"
title: "Layouts and UI Design"
---

**Estimated Time**: 75 minutes

**Learning Objectives**:
- Master Row, Column, Box for flexible layouts
- Use ConstraintLayout in Compose Multiplatform 1.10+
- Apply modifiers for styling, sizing, and positioning
- Create responsive designs with proper spacing and alignment

---

## Layout Fundamentals in Compose

Compose Multiplatform provides a powerful layout system built on simple, composable principles. Whether you're building a list screen, a form, or a complex dashboard, the same core layouts adapt to any screen size.

### Core Layout Composables

**Row** arranges children horizontally:
```kotlin
Row(
    horizontalArrangement = Arrangement.SpaceBetween,
    verticalAlignment = Alignment.CenterVertically
) {
    Text("Left")
    Text("Right")
}
```

**Column** arranges children vertically:
```kotlin
Column(
    verticalArrangement = Arrangement.spacedBy(16.dp),
    horizontalAlignment = Alignment.CenterHorizontally
) {
    Text("Header")
    Button(onClick = {}) { Text("Action") }
}
```

**Box** stacks children, useful for overlays:
```kotlin
Box(modifier = Modifier.fillMaxSize()) {
    Image(painter = background, contentDescription = null)
    Text("Overlay", modifier = Modifier.align(Alignment.Center))
}
```

### ConstraintLayout in CMP 1.10+

New in Compose Multiplatform 1.10, **ConstraintLayout** brings complex responsive layouts:

```kotlin
ConstraintLayout(modifier = Modifier.fillMaxSize()) {
    val (header, content, footer) = createRefs()
    
    TopAppBar(
        modifier = Modifier.constrainAs(header) {
            top.linkTo(parent.top)
            start.linkTo(parent.start)
            end.linkTo(parent.end)
        }
    )
    
    // content and footer constraints...
}
```

### The Modifier System

**Modifiers** are the Swiss Army knife of Compose styling. Chain them for precise control:

```kotlin
Text(
    text = "Styled Text",
    modifier = Modifier
        .padding(16.dp)
        .background(MaterialTheme.colorScheme.surface)
        .border(1.dp, MaterialTheme.colorScheme.outline)
        .padding(8.dp)
        .fillMaxWidth()
)
```

Real-world apps like **Cash App** use these patterns to create consistent, accessible layouts across Android and iOS. The modifier chain order matters—each modifier wraps the previous one, creating layered styling effects.
