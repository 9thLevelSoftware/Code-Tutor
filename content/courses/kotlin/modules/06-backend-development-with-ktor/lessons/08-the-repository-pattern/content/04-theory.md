---
type: "THEORY"
title: "🏗️ Clean Architecture Layers"
---


### The Three-Layer Architecture


### Dependency Flow

**Key principle**: Outer layers depend on inner layers, never the reverse.


---



```kotlin
Routes → Services → Repositories → Database
  ↓         ↓            ↓
HTTP    Business      Data
        Logic       Storage
```
