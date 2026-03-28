---
type: "KEY_POINT"
title: "Answer Key"
---


### Answer 1: B
**Correct**: It provides automatic real-time updates when posts change

StreamBuilder listens to Firestore's `snapshots()` stream and automatically rebuilds the UI whenever data changes. When someone creates/deletes a post, all users see the update instantly without manual refresh.

### Answer 2: B
**Correct**: To avoid querying all posts to count them (performance)

Storing an aggregated count prevents expensive queries. Without it, you'd need to fetch all user posts just to count them (slow and costly). Firestore charges per document read, so fewer reads = lower costs.

### Answer 3: B
**Correct**: Security rules

Without proper security rules, your database is wide open - anyone can read/write/delete anything. Beautiful UI doesn't matter if hackers steal all user data. Security rules are the foundation of production apps.

