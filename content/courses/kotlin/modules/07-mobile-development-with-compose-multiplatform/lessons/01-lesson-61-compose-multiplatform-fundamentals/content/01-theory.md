---
type: "THEORY"
title: "Introduction to Compose Multiplatform"
---

**Estimated Time**: 75 minutes

**Learning Objectives**:
- Understand the difference between Compose Multiplatform and Jetpack Compose
- Learn the cross-platform promise and architecture of CMP
- Explore real-world apps using Compose Multiplatform
- Set up a basic CMP project structure

---

## Compose Multiplatform: One UI, Everywhere

**Compose Multiplatform (CMP)** is JetBrains' revolutionary UI framework that brings the power of Jetpack Compose beyond Android to iOS, Desktop, and Web. Built on top of the same declarative UI principles, CMP allows you to share up to 100% of your UI code across platforms while maintaining native performance and appearance.

### CMP vs Jetpack Compose

While **Jetpack Compose** is Android-only and tightly integrated with the Android ecosystem, **Compose Multiplatform** extends these concepts to multiple platforms:

| Feature | Jetpack Compose | Compose Multiplatform |
|---------|-----------------|----------------------|
| Platforms | Android only | Android, iOS, Desktop, Web |
| Rendering | Android Canvas | Skia (cross-platform) |
| Version | Part of AndroidX | JetBrains distribution |
| Status | Stable, production-ready | Stable (1.10.x) |

The beauty of CMP is that if you already know Jetpack Compose, you already know 90% of Compose Multiplatform. The `@Composable` annotation, state management, and layout system are identical.

### The Cross-Platform Promise

Compose Multiplatform uses **Skia** (the same rendering engine used by Flutter and Chrome) to draw UI consistently across all platforms. Unlike web-based solutions, CMP compiles to native code and renders natively—giving you true native performance without JavaScript bridges.

### Architecture Overview

A typical CMP project follows this structure:
- **commonMain/**: Shared UI code, ViewModels, business logic
- **androidMain/**: Android-specific implementations (if needed)
- **iosMain/**: iOS-specific bindings and platform code

Real-world apps built with CMP include **Cash App** (by Square), **JetBrains Toolbox**, and **KotlinConf** app. These apps demonstrate that CMP is production-ready for complex, high-performance applications.

In this lesson, you'll set up your first CMP project and understand how the shared UI layer communicates with platform-specific code through the `expect/actual` mechanism. Let's build something that runs everywhere!
