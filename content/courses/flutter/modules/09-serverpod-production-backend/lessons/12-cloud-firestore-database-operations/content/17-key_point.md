---
type: "KEY_POINT"
title: "Answer Key"
---


### Answer 1: B
**Correct**: `.get()` fetches once, `.snapshots()` provides real-time updates via Stream

`.get()` returns a Future that fetches data once. `.snapshots()` returns a Stream that continuously listens for changes and automatically updates your UI via StreamBuilder.

### Answer 2: B
**Correct**: They're atomic (all-or-nothing) and more efficient

Batch writes ensure all operations succeed or fail together (atomicity), prevent partial updates, and reduce network calls by bundling multiple operations into one request.

### Answer 3: B
**Correct**: 3-4 levels

While Firestore technically allows deeper nesting, 3-4 levels is the practical recommendation. Deeper nesting makes queries complex and can impact performance. Consider denormalizing or using subcollections instead.

