---
type: "THEORY"
title: "Introduction to Compose Multiplatform UI"
---

**Estimated Time**: 60 minutes
**Difficulty**: Beginner
**Prerequisites**: Kotlin basics, understanding of functions and lambdas

## Introduction

Compose Multiplatform is JetBrains' declarative UI framework that lets you build native user interfaces for Android, iOS, desktop (Windows, macOS, Linux), and web using a single Kotlin codebase. Unlike traditional UI frameworks that use XML or Storyboards, Compose lets you define your entire UI in Kotlin code using a declarative approach.

## Learning Objectives

By the end of this lesson, you will:
- Understand the declarative UI paradigm
- Set up a Compose Multiplatform project
- Create basic composable functions
- Use built-in UI components like Text, Button, and Column
- Understand the recomposition mechanism

## Why Compose Multiplatform?

**Code Sharing**: Write UI code once, run on multiple platforms. Business logic can be 100% shared; UI code is typically 70-90% shared with platform-specific tweaks for native look-and-feel.

**Declarative UI**: Describe what your UI should look like for any given state, and Compose handles the updates automatically. No more manual view updates or finding views by ID.

**Kotlin-First**: Leverage Kotlin's features—coroutines, DSLs, type safety—throughout your UI layer.

**Native Performance**: Compose renders to native widgets on each platform, not a WebView or custom drawing layer.

## Real-World Context

Companies like VMware, Cash App, and Target use Compose Multiplatform to share code between mobile platforms while maintaining native performance. The framework is production-ready for Android and desktop, with iOS support rapidly maturing.

Let's begin your journey into modern, declarative UI development!