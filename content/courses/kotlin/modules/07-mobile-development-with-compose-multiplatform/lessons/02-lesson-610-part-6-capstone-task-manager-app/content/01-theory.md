---
type: "THEORY"
title: "Capstone Project: Task Manager App"
---

**Estimated Time**: 4-6 hours

**Learning Objectives**:
- Build a complete cross-platform Task Manager application
- Apply MVVM architecture with shared ViewModels
- Implement local data persistence with MultiplatformSettings
- Create responsive UI with Material 3 components
- Deploy to both Android and iOS

---

## Welcome to Your Capstone Project

This capstone brings together everything you've learned in the Compose Multiplatform module. You'll build a **Task Manager App**—a production-ready application that runs natively on both Android and iOS from a single Kotlin codebase.

### What You'll Build

The Task Manager App is a fully-featured productivity application with:

- **Task Management**: Create, edit, complete, and delete tasks
- **Categories**: Organize tasks into Work, Personal, and custom categories
- **Due Dates**: Set reminders with date/time pickers
- **Filtering & Sorting**: View tasks by status, priority, or due date
- **Data Persistence**: Local storage using MultiplatformSettings with SQLDelight
- **Offline Support**: Full functionality without internet connectivity

### Architecture Highlights

The app demonstrates modern KMP architecture patterns:

```
┌─────────────────────────────────────────────────────┐
│                    UI Layer                          │
│  ┌──────────┐ ┌──────────┐ ┌──────────────────┐    │
│  │ TaskList │ │ TaskForm │ │ CategoryPicker   │    │
│  └────┬─────┘ └────┬─────┘ └────────┬─────────┘    │
└───────┼────────────┼────────────────┼──────────────┘
        │            │                │
┌───────▼────────────▼────────────────▼──────────────┐
│                 ViewModel Layer                    │
│         (Shared between Android & iOS)            │
└───────┬────────────┬────────────────┬──────────────┘
        │            │                │
┌───────▼────────────▼────────────────▼──────────────┐
│               Repository Layer                     │
│    (expect/actual for platform-specific storage)  │
└────────────────────────────────────────────────────┘
```

### Real-World Foundation

Apps like **Todoist**, **Microsoft To Do**, and **Google Tasks** follow similar architectures. Your Task Manager will showcase:

- **Shared UI** with platform-specific navigation patterns
- **Offline-first data** with local persistence
- **Material 3 design** with dynamic theming
- **Smooth animations** using Compose Animation APIs

By the end of this capstone, you'll have a portfolio-worthy application demonstrating your mastery of cross-platform mobile development with Kotlin.
